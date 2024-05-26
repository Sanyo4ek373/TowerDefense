using System;
using System.Collections.Generic;
using MachinationsUP.Engines.Unity;
using MachinationsUP.Integration.Binder;
using MachinationsUP.Integration.Elements;
using MachinationsUP.Integration.Inventory;
using UnityEngine;

namespace MachinationsUP.GeneratedCode
{

  [CreateAssetMenu(menuName = "MachinationsOut/GeneratedSO_20220228_095419")]
  public class GeneratedSO_20220228_095419 : ScriptableObject, IMnScriptableObject
  {

    //Constants for imported Machinations Elements.
    private const string M_TOKENLIMITS = "TokenLimits [Token Limits]";


    //Variables used to value-transfer with Machinations Elements.
    public ElementBase TokenLimits;


    public event EventHandler OnUpdatedFromMachinations;

    public void OnEnable()
    {
      Debug.Log("Scriptable Object GeneratedSO_20220228_095419 OnEnable.");
      //Manifest that defines what the SO uses from Machinations.
      Manifest = new MnObjectManifest
      {
        Name = "Some Inspiring Name for This Set of Values",
        DiagramMappings = new List<DiagramMapping>
      {
        new DiagramMapping
        {
          PropertyName = M_TOKENLIMITS,
          DiagramElementID = 676,
          EditorElementBase = TokenLimits
        }
      }
      };

      //Register this SO with the MDL.
      MnDataLayer.EnrollScriptableObject(this, Manifest);
    }

    #region IMnScriptableObject

    public MnObjectManifest Manifest { get; private set; }

    public ScriptableObject SO => this;

    /// <summary>
    /// Called when Machinations initialization has been completed.
    /// </summary>
    /// <param name="binders">The Binders for this Object.</param>
    public void MDLInitCompleteSO(Dictionary<string, ElementBinder> binders)
    {
      TokenLimits = binders[M_TOKENLIMITS].CurrentElement;

    }

    /// <summary>
    /// Called by the <see cref="MnDataLayer"/> when an element has been updated in the Machinations back-end.
    /// </summary>
    /// <param name="diagramMapping">The <see cref="DiagramMapping"/> of the modified element.</param>
    /// <param name="elementBase">The <see cref="ElementBase"/> that was sent from the backend.</param>
    public void MDLUpdateSO(DiagramMapping diagramMapping = null, ElementBase elementBase = null)
    {

    }

    #endregion

  }

}