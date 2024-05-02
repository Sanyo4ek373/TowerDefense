using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageDropdownController : MonoBehaviour {
    [SerializeField] private LanguageManager _languageManager;

    private TMP_Dropdown _dropdown;

    private string _language;

    private const string ENGLISH = "English (en)";
    private const string UKRAINIAN = "Ukrainian (uk)";
    private const string POLISH = "Polish (pl)";

    private Dictionary<string, int> _languages = new() {
        { ENGLISH, 0 }, { UKRAINIAN, 1 }, { POLISH, 2 }
    };

    private void Awake() {
        _dropdown = GetComponent<TMP_Dropdown>();
        _language = _languageManager.GetCurrentLocaleName();
        _dropdown.value = _languages[_language];
    }
}