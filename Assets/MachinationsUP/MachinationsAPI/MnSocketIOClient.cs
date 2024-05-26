using System;
using System.Collections.Generic;
using MachinationsUP.SyncAPI;
using MachinationsUP.Logger;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using MachinationsUP.Engines.Unity.GameComms;
using MachinationsUP.ThirdParty.SocketIO.JSONObject;

namespace MachinationsUP.Engines.Unity.BackendConnection
{
  /// <summary>
  /// Wraps SocketIOGlobal, adding Machinations-specific handling.
  /// </summary>
  public class MnSocketIOClient
  {

    #region Configuration Constants/Fields

    private const int MAX_RECONNECTION_ATTEMPTS = 100;
    private int _reconnectionAttempts;

    #endregion

    /// <summary>
    /// Socket used for communication.
    /// </summary>
    private SocketIOUnity _socket;

    /// <summary>
    /// Service that owns this Socket.
    /// </summary>
    private MnService _mnService;

    /// <summary>
    /// Where to connect for the Machinations API.
    /// </summary>
    private string _socketURL = "https://api.machinations.io";

    /// <summary>
    /// The User Key under which to make all API calls. This can be retrieved from
    /// the Machinations product.
    /// </summary>
    private string _userKey = "NONE_SPECIFIED";

    //TODO: Diagram Token system will soon be upgraded to support multiple diagrams.
    /// <summary>
    /// The Machinations Diagram Token will be used to identify ONE Diagram that this game is connected to.
    /// </summary>
    private string _diagramToken = "NONE_SPECIFIED";

    /// <summary>
    /// Game name is be used for associating a game with multiple diagrams.
    /// [TENTATIVE UPCOMING SUPPORT]
    /// </summary>
    private string _gameName;

    /// <summary>
    /// Connection to Machinations Backend has been aborted.
    /// </summary>
    private bool _connectionAborted;

    /// <summary>
    /// Prepares and Connects the Socket to the Machinations Backend.
    /// </summary>
    /// <param name="mnService">MachinationsService to interact with for data transactions.</param>
    /// <param name="socketURL">The URL where the Machinations API resides.</param>
    /// <param name="userKey">User Key (API key) to use when connecting to the back-end.</param>
    /// <param name="diagramToken">Diagram Token to make requests to.</param>
    /// <param name="gameName">OPTIONAL. Game name.</param>
    public MnSocketIOClient(MnService mnService, string socketURL, string userKey, string diagramToken,
        string gameName = null)
    {
      _socketURL = socketURL;
      _userKey = userKey;
      _gameName = gameName;
      _diagramToken = diagramToken;
      _mnService = mnService;

      InitSocket();
    }

    /// <summary>
    /// Initializes the Socket IO component.
    /// <param name="socketURL">The URL where the Machinations API resides.</param>
    /// <param name="userKey">User Key (API key) to use when connecting to the back-end.</param>
    /// <param name="diagramToken">Diagram Token to make requests to.</param>
    /// </summary>
    public void InitSocket(string socketURL = "", string userKey = "", string diagramToken = "", string gameName = "")
    {
      //Apply new URL if needed.
      if (socketURL != "") _socketURL = socketURL;
      if (userKey != "") _userKey = userKey;
      if (diagramToken != "") _diagramToken = diagramToken;
      if (gameName != "") _gameName = gameName;

      L.D("Socket initializing...");
      if (!CanReconnect()) return;

      try
      {
        //Close socket.
        if (_socket != null)
        {
          _socket.Disconnect();
          _socket.Dispose();
          _socket = null;
        }

        L.W(_socketURL.ToString());
        L.W(_userKey.ToString());
        L.W(_diagramToken.ToString());

        var uri = new Uri(_socketURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
          Query = new Dictionary<string, string>
                {
                    {"token", _userKey } //replace with userKey
                },
          EIO = 4,
          Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        _socket.JsonSerializer = new NewtonsoftJsonSerializer();

        _socket.OnConnected += (sender, e) =>
        {
          L.D("MnSocketIOClient: Connection Achieved!!! Attempting authentication.");
          SocketOpenReceived = true;
          _socket.EmitStringAsJSON("api-authorize", "{ \"diagramToken\": \"" + _diagramToken + "\", \"gameName\": \"" + _gameName + "\" }"); //replace with Diagram Token
        };

        //Same handler to be used for both threads.
        _socket.OnAny(OnAnyHandler);
        _socket.OnAnyInUnityThread(OnAnyHandler);

        _socket.OnPing += (sender, e) =>
        {
          L.D("Ping");
        };

        _socket.OnPong += (sender, e) =>
        {
          L.D("Pong: " + e.TotalMilliseconds);
        };

        _socket.OnDisconnected += (sender, e) =>
        {
          L.D("MnSocketIOClient Socket DISCONNECTED: " + e);
          _mnService.FailedToConnect(true);
          CanReconnect();
        };

        _socket.OnReconnectAttempt += (sender, e) =>
        {
          L.D($"{DateTime.Now} MnSocketIOClient Socket Reconnecting: attempt = {e}");
          _mnService.FailedToConnect(true);
          CanReconnect();
        };

        L.D("Instantiated SocketIO with Hash: " + _socket.GetHashCode() + " for URL: " + _socketURL);

        /*
              _socket.On(SyncMsgs.RECEIVE_AUTH_SUCCESS, OnAuthSuccess);
              _socket.On(SyncMsgs.RECEIVE_AUTH_DENY, OnAuthDeny);
              _socket.On(SyncMsgs.RECEIVE_OPEN, OnSocketOpen);
              _socket.On(SyncMsgs.RECEIVE_OPEN_START, OnSocketOpenStart);
              _socket.On(SyncMsgs.RECEIVE_GAME_INIT, OnGameInitResponse);
              _socket.On(SyncMsgs.RECEIVE_DIAGRAM_ELEMENTS_UPDATED, OnDiagramElementsUpdated);
              _socket.On(SyncMsgs.RECEIVE_ERROR, OnSocketError);
              _socket.On(SyncMsgs.RECEIVE_API_ERROR, OnSocketError);
              _socket.On(SyncMsgs.RECEIVE_CLOSE, OnSocketClose);
              _socket.On(SyncMsgs.RECEIVE_GAME_UPDATE_DIAGRAM_ELEMENTS, OnGameUpdateDiagramElementsRequest);
        */

        L.D("SocketIO: Connect Start.");
        _socket.Connect();
        L.D("SocketIO: Connect End.");
      }
      catch (Exception ex)
      {
        L.ToLogFile("MachinationsService Scheduler Exception Caught:");
        L.ExToLogFile(ex);
        L.Ex(ex);
      }
    }

    /// <summary>
    /// Emits the 'Game Init' Socket event.
    /// </summary>
    public void EmitInitRequest()
    { _socket.EmitStringAsJSON("game-init", "{ \"diagramToken\": \"" + _diagramToken + "\", \"gameName\": \"Test\" }"); }

    /// <summary>
    /// Handles events coming from Machinations.
    /// </summary>
    private void OnAnyHandler(string name, SocketIOResponse response)
    {
      L.D("MnSocketIOClient: MnSocketIOClient Received On " + name + " : " + response.ToString() + "\n");

      switch (name)
      {

        case SyncMsgs.RECEIVE_AUTH_SUCCESS:
          {
            L.D("MnSocketIOClient: AUTHENTICAITON SUCCESS!!! Now requesting Diagram data.");
            _mnService.AuthSuccess();
            IsAuthenticated = true;
            _reconnectionAttempts = 0; //Reset reconnection count on Auth Success.
            EmitInitRequest();
          }
          break;

        case SyncMsgs.RECEIVE_GAME_INIT:
          {
            L.D("MnSocketIOClient: INITIALIZATION SUCCESS!!! Diagram data received.");

            JSONObject json = new JSONObject(response.ToString());
            _mnService.InitComplete(json.list);

            foreach (JSONObject jso in json.list)
            {
              //The answer from the back-end may contain multiple payloads.
              foreach (string payloadKey in jso.keys)
                //For now, only interested in the "SyncMsgs.JK_DIAGRAM_ELEMENTS_LIST" payload.
                if (payloadKey == SyncMsgs.JK_DIAGRAM_ELEMENTS_LIST)
                  _mnService.InitComplete(jso[SyncMsgs.JK_DIAGRAM_ELEMENTS_LIST].list);
            }

          }
          break;

        case SyncMsgs.RECEIVE_DIAGRAM_ELEMENTS_UPDATED:
          {
            L.D("MnSocketIOClient: Diagram Elements Updated RECEIVED.");

            JSONObject json = new JSONObject(response.ToString());

            foreach (JSONObject jso in json.list)
            {
              //The answer from the back-end may contain multiple payloads.
              foreach (string payloadKey in jso.keys)
                //For now, only interested in the "SyncMsgs.JK_DIAGRAM_ELEMENTS_LIST" payload.
                if (payloadKey == SyncMsgs.JK_DIAGRAM_ELEMENTS_LIST)
                  _mnService.UpdateWithValuesFromMachinations(jso[SyncMsgs.JK_DIAGRAM_ELEMENTS_LIST].list, true);
            }
          }
          break;

      }
    }

    /// <summary>
    /// Emits the 'Game Update Diagram Elements' Socket event.
    /// </summary>
    public void EmitGameUpdateDiagramElementsRequest(JSONObject updateRequest)
    {
      var toEmit = updateRequest.ToString();
      L.D("MnSocketIOClient: about to Emit: " + toEmit);
      _socket.EmitStringAsJSON(SyncMsgs.SEND_GAME_UPDATE_DIAGRAM_ELEMENTS, toEmit);
    }

    /// <summary>
    /// Returns whether or not the Socket is allowed to reconnect.
    /// </summary>
    private bool CanReconnect()
    {
      _reconnectionAttempts++;
      if (_reconnectionAttempts > MAX_RECONNECTION_ATTEMPTS)
      {
        L.E("PERMANENT SOCKET CONNECTION FAILURE. Machinations UP is now giving up attempting to make a connection. To recover from this, you must restart the game.");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Used to restart the reconnection counter.
    /// </summary>
    public void ResetReconnectionCounter()
    {
      _reconnectionAttempts = 0;
    }

    /// <summary>
    /// TRUE when the Socket is open.
    /// </summary>
    private bool SocketOpenReceived { set; get; }

    /// <summary>
    /// TRUE when all Init-related tasks have been completed.
    /// </summary>
    public bool IsAuthenticated { set; get; }

    /// <summary>
    /// TRUE when all Init-related tasks have been completed.
    /// </summary>
    public bool IsInitialized => _socket != null && _socket.Connected && SocketOpenReceived;

  }
}