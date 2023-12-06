using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private ResourcesManager _resources;
    [SerializeField] private MenuManager _menu;
    [SerializeField] private GameObject _enemySpawner;

    private void Update() {
        if (_enemySpawner.GetComponent<EnemySpawner>().VaweNumber == 10) {
            _enemySpawner.SetActive(false);

            foreach (EnemyController enemy in EnemySpawner.EnemyList) {
                if (enemy == null) WinGame();
            }
        }
    }

    private void Awake() {
        _resources.OnResourceLack.AddListener(EndGame);
    }

    private void OnDestroy() {
        _resources.OnResourceLack.RemoveListener(EndGame);
    }

    private void EndGame() {
        _menu.ShowLoseMenu();
    }

    private void WinGame() {
        _menu.ShowWinMenu();
    }
}