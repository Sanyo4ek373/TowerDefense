using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private ResourcesManager _resources;
    [SerializeField] private MenuManager _menu;
    [SerializeField] private MainBuilding _mainBuilding;
    [SerializeField] private GameObject _enemySpawner;

    private float _waitTime = 3f;

    private void Update() {
        if (_enemySpawner.GetComponent<EnemySpawner>().WaveNumber == 15) {
            _enemySpawner.SetActive(false);

            if (EnemySpawner.EnemyList.Count == 1) StartCoroutine(WinGame(_waitTime));
        }
    }

    private void Awake() {
        _resources.OnResourceLack += EndGame;
        _mainBuilding.OnBuildingDestroy += EndGame;
    }

    private void OnDestroy() {
        _resources.OnResourceLack -= EndGame;
        _mainBuilding.OnBuildingDestroy -= EndGame;
    }

    private void EndGame() {
        _menu.ShowLoseMenu();
    }

    private IEnumerator WinGame(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        _menu.ShowWinMenu();
    }
}