using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pauseButton;

    [SerializeField] private GameObject _winMenu;
    [SerializeField] private GameObject _loseMenu;

    [SerializeField] private GameObject _settings;

    private bool _gameIsPaused = true;
    private bool _settingsEnabled = true;

    public void OnPauseButtonPressed() {
        _pauseMenu.SetActive(_gameIsPaused);
        _settings.SetActive(false);
        Time.timeScale = Convert.ToSingle(!_gameIsPaused);

        _gameIsPaused =! _gameIsPaused;
        _settingsEnabled = true;
        _pauseButton.SetActive(true);
    }

    public void OnSettingsButtonPressed() {
        _pauseMenu.SetActive(!_settingsEnabled);
        _settings.SetActive(_settingsEnabled);

        _settingsEnabled =! _settingsEnabled;
    }

    public void ShowWinMenu() {
        _winMenu.SetActive(true);
        PauseGame();
    }

    public void ShowLoseMenu() {
        _loseMenu.SetActive(true);
        PauseGame();
    }

    public void ChangeScene(string sceneName) { 
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame() {
        Application.Quit();
    }

    private void Awake() {
        Time.timeScale = 1f;
    }

    private void PauseGame() {
        _gameIsPaused =! _gameIsPaused;
        Time.timeScale = 0f;
        _pauseButton.SetActive(false);
    }
}