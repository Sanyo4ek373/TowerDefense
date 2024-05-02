using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour {
    private bool _isActive = false;

    public string GetCurrentLocaleName() {
        return LocalizationSettings.SelectedLocale.LocaleName;
    }

    public void ChangeLanguage(int localeID) {
        if (_isActive) return;

        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int _localeID) {
        _isActive = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];

        _isActive = false;
    }
}
