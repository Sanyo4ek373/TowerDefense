using System;
using System.Collections.Generic;
using MachinationsUP.Config;
using MachinationsUP.Integration.Elements;
using System.Timers;
using MachinationsUP.Engines.Unity.BackendConnection;
using MachinationsUP.Logger;
using MachinationsUP.ThirdParty.SocketIO.JSONObject;

namespace MachinationsUP.Engines.Unity.GameComms
{
  /// <summary>
  /// Service States. Will soon be a part of a State Machine implemented here.
  /// </summary>
  public enum State
  {

    Idling,
    WaitingForSocketReady,
    AuthRequested,
    AuthSuccess,
    PreparingForInitRequest,
    InitRequested,
    InitComplete,

  }

  /// <summary>
  /// Handles communication with the Machinations back-end.
  /// TODO: improve state machine.
  /// TODO: when implementing multiple diagrams support, beware of how full init request is used [EmitFullDiagramInitRequest].
  /// </summary>
  public class MnService
  {

    /// <summary>
    /// The current way of communicating with the Machinations back-end.
    /// </summary>
    private MnSocketIOClient _socketClient;

    /// <summary>
    /// Timer that handles service State changes.
    /// </summary>
    readonly private Timer _scheduler;

    /// <summary>
    /// Current State of the Service.
    /// </summary>
    private State _currentState;

    /// <summary>
    /// Diagram elements received from the Back-end. Sent from SocketIOClient.
    /// </summary>
    private List<JSONObject> _diagramElementsFromBackEnd;

    /// <summary>
    /// Returns whether or not the Diagram has been fully initialized.
    /// TODO: must be redesigned when implementing support for multiple diagrams.
    /// </summary>
    private bool HasPerformedFullDiagramInit { get; set; }

    /// <summary>
    /// Is the game running?
    /// </summary>
    public bool IsGameRunning { get; set; }

    /// <summary>
    /// TRUE: switch _currentState to PreparingForInitRequest state at the first available time.
    /// </summary>
    private bool _initRequested;

    /// <summary>
    /// Used to process back-end updates on the main thread. When these updates are processed directly in the Socket Thread,
    /// game object (scene) updates would crash, because they need to be processed on the main thread. 
    /// </summary>
    private List<KeyValuePair<List<JSONObject>, bool>> _updatesFromBackEnd = new List<KeyValuePair<List<JSONObject>, bool>>();


    /// <summary>
    /// Singleton instance.
    /// </summary>
    static private MnService _instance;

    /// <summary>
    /// How many seconds the Service has spent waiting for an Auth response. If none is received, a reconnection attempt will happen.
    /// </summary>
    static private int timeoutCounter;

    const int TIMEOUT_SECONDS = 10;

    /// <summary>
    /// Singleton implementation.
    /// </summary>
    static public MnService Instance
    {
      get
      {
        if (_instance != null) return _instance;
        _instance = new MnService();
        L.D("MnService created by invocation. Hash is " + _instance.GetHashCode() +
            " and User Key is " + MnConfig.Instance.UserKey);
        return _instance;
      }
    }

    public MnService()
    {
      L.D("Instantiated MnService with Hash: " + GetHashCode());
      _scheduler = new Timer(1000);
      _scheduler.Elapsed += Scheduler_OnElapsed;
      _scheduler.Start();

      _socketClient = new MnSocketIOClient(
                this,
                MnConfig.Instance.APIURL,
                MnConfig.Instance.UserKey,
                MnConfig.Instance.DiagramToken,
                MnConfig.Instance.GameName);

      _currentState = State.WaitingForSocketReady;
    }

    private void Scheduler_OnElapsed(object sender, ElapsedEventArgs e)
    {
      if (_currentState == State.WaitingForSocketReady || _currentState == State.AuthRequested || _currentState == State.InitRequested)
      {
        timeoutCounter++;
        if (timeoutCounter > TIMEOUT_SECONDS)
        {
          timeoutCounter = 0;
          Restart();
        }
      }

      //When the game is running, ProcessSchedule will be called by the MachinationsMainThreadHook.
      if (IsGameRunning) return;
      ProcessSchedule();
    }

    /// <summary>
    /// Handles Machinations tasks.
    /// </summary>
    public void ProcessSchedule()
    {
      try
      {
        //Execute any updates from the back-end.
        while (_updatesFromBackEnd.Count > 0)
        {
          MnDataLayer.UpdateSourcesWithValuesFromMachinations(_updatesFromBackEnd[0].Key, _updatesFromBackEnd[0].Value);
          _updatesFromBackEnd.RemoveAt(0);
        }

        //Rudimentary State Machine:

        //Socket not yet ready?
        if (!_socketClient.IsInitialized)
        {
          L.D("Machinations Service SocketIO Scheduler: Waiting for Socket Connection. Current State: " + _currentState);
          if (_currentState != State.WaitingForSocketReady)
          {
            L.E("Invalid state given the fact that the socket is not even initialized.");
            FreshStart();
          }

          return;
        }

        //If the socket isn't yet ready, starting the Auth process.
        if (_currentState == State.WaitingForSocketReady)
        {
          L.D("Machinations Service SocketIO Scheduler: WaitingForSocketReady -> AuthRequested.");
          _currentState = State.AuthRequested;
          _socketClient.InitSocket(); //Request Auth again.
          return;
        }

        //Wait for Auth.
        if (_currentState == State.AuthRequested)
        {
          L.D("Machinations Service SocketIO Scheduler: Waiting for Auth Response.");
          return;
        }

        //Wait for Init.
        if (_currentState == State.InitRequested)
        {
          L.D("Machinations Service SocketIO Scheduler: Waiting for Sync Init Response.");
          return;
        }

        //When everything is connected, switching through the following states:
        switch (_currentState)
        {

          case State.AuthSuccess:
            L.D("Machinations Service SocketIO Scheduler: Auth Success. Idling.");
            _currentState = State.InitRequested; //After the 2023 refactor, the Init request is immediately done after Auth.
            break;

          case State.Idling:
            if (_initRequested)
            {
              L.I("Machinations Service SocketIO Scheduler: Init Requested: Idling -> PreparingForInitRequest.");
              _currentState = State.PreparingForInitRequest;
            }
            break;

          //Wait at least 1 Timer interval before making the Sync request.
          case State.PreparingForInitRequest:
            L.D("Machinations Service SocketIO Scheduler: Init Requested.");
            _currentState = State.InitRequested;
            _initRequested = true;
            //The first time we get here, we will perform a FULL init request.
            //_socketClient.EmitDiagramInitRequest(HasPerformedFullDiagramInit);
            HasPerformedFullDiagramInit = true; //But subsequent times, we will only ask for whatever is new.
            //TOOD: implement support for "HasPerformedFullDiagramInit".
            _socketClient.EmitInitRequest();
            break;

          case State.InitComplete:
            L.D("Machinations Service SocketIO Scheduler: Init Complete. " + _currentState + " -> Idling.");
            _currentState = State.Idling;
            _initRequested = false;
            MnDataLayer.UpdateSourcesWithValuesFromMachinations(_diagramElementsFromBackEnd);
            MnDataLayer.SyncComplete();
            break;

        }
      }
      catch (Exception ex)
      {
        L.ToLogFile("MnService Scheduler Exception Caught:");
        L.ExToLogFile(ex);
      }
    }

    /// <summary>
    /// Prepares the service for a clean (re)start.
    /// </summary>
    private void FreshStart()
    {
      _currentState = State.WaitingForSocketReady;
      _initRequested = false;
      HasPerformedFullDiagramInit = false;
    }

    /// <summary>
    /// Tells the Service to perform a Sync with the Machinations Back-end.
    /// Multiple calls to this function are allowed, as the Service will always wait some time after the last request has gone in.
    /// </summary>
    public void ScheduleSync()
    {
      L.D("MnService.ScheduleSync.");
      _initRequested = true;
      //Reset wait period.
      if (_currentState == State.PreparingForInitRequest) _currentState = State.Idling;
    }

    #region Communication via Socket IO.

    //TODO: to be switched to event-based during next iteration.
    public void FailedToConnect(bool loadCache)
    {
      FreshStart();
      MnDataLayer.SyncFail(loadCache);
    }

    public void InitComplete(List<JSONObject> diagramElementsFromBackEnd)
    {
      //Nothing returned?
      if (diagramElementsFromBackEnd == null)
      {
        _currentState = State.Idling;
        return;
      }

      L.I("Switching state to INIT COMPLETE");
      _diagramElementsFromBackEnd = diagramElementsFromBackEnd;
      _currentState = State.InitComplete;
      _initRequested = false;
    }

    #endregion

    /// <summary>
    /// Called by the Socket when an answer is received from the Machinations Back-end.
    /// TODO: useless passing of values?
    /// </summary>
    /// <param name="elementsFromBackEnd"></param>
    /// <param name="updateFromDiagram"></param>
    public void UpdateWithValuesFromMachinations(List<JSONObject> elementsFromBackEnd, bool updateFromDiagram = false)
    {
      _updatesFromBackEnd.Add(new KeyValuePair<List<JSONObject>, bool>(elementsFromBackEnd, updateFromDiagram));
    }

    /// <summary>
    /// Used to update the diagram with new values from the game.
    /// </summary>
    public void EmitGameUpdateDiagramElementsRequest(JSONObject updateRequest)
    {
      L.I("EmitGameUpdateDiagramElementsRequest: " + updateRequest);
      _socketClient.EmitGameUpdateDiagramElementsRequest(updateRequest);
    }

    public void PauseSync()
    {
      throw new System.NotImplementedException();
    }

    public void AuthSuccess()
    {
      _currentState = State.AuthSuccess;
    }

    public void PlayStateChanged()
    {

    }

    /// <summary>
    /// Restarts the Service to apply eventual new connection data.
    /// <param name="socketURL">The URL where the Machinations API resides.</param>
    /// <param name="userKey">User Key (API key) to use when connecting to the back-end.</param>
    /// <param name="diagramToken">Diagram Token to make requests to.</param>
    /// </summary>
    public void Restart(string socketURL = "", string userKey = "", string diagramToken = "", string gameName = "")
    {
      L.I("Machinations Service Restarting!");
      FreshStart();
      //TODO: determine if this is really necessary.
      //MnDataLayer.ResetData();
      _socketClient.InitSocket(socketURL, userKey, diagramToken, gameName);
    }

  }
}