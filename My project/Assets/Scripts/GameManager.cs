using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private ResourcesManager _resources;
    [SerializeField] private MenuManager _menu;
    [SerializeField] private GameObject _enemySpawner;

    private float _waitTime = 3f;

    private void Update() {
        if (_enemySpawner.GetComponent<EnemySpawner>().VaweNumber == 3) {
            _enemySpawner.SetActive(false);

            if (EnemySpawner.EnemyList.Count == 1) StartCoroutine(WinGame(_waitTime));
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

    private IEnumerator WinGame(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        _menu.ShowWinMenu();
    }
}