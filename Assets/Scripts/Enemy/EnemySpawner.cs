using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public Action<int, int> OnWaveChange;

    [SerializeField] private Transform[] _spawnPlaces;
    [SerializeField] private GameObject[] _troops;
    [SerializeField] private MainBuilding _mainBuilding;
    [SerializeField] private float _spawnTime;
    
    private bool _isTroopsSpawned = true;

    private int _waveNumber;
    private int _waveLevel = 1;

    private int _minEnemiesInWave = 2;
    private int _maxEnemiesInWave = 3;

    public int WaveLevel => _waveLevel;

    public static List<EnemyController> EnemyList = new();

    private void Awake() {
        EnemyList.Clear();
    }

    private void Update() {
        if(_isTroopsSpawned && _waveLevel < 6) { 
            for (int i = UnityEngine.Random.Range(0, _minEnemiesInWave + 1); i <_maxEnemiesInWave; i++) {
                StartCoroutine(SpawnTroops(_spawnTime, _troops[0], _spawnPlaces[i]));
            }
            
            _waveNumber += 1;
            OnWaveChange(_waveLevel, _waveNumber);
        }

        ChangeWaveLevel();
    }

    private IEnumerator SpawnTroops(float waitTime, GameObject troop, Transform spawnPlace) {
        _isTroopsSpawned = false;
    
        EnemyList.Add(
            Instantiate(troop, spawnPlace.position, Quaternion.identity)
            .GetComponent<EnemyController>()
        );
        
        EnemyList.Last().Construct(_mainBuilding);
        yield return new WaitForSeconds(waitTime);

        _isTroopsSpawned = true;
    }

    private void ChangeWaveLevel() {
        if(_waveNumber == 8) {
            _waveNumber = 0;
            _waveLevel += 1;

            IncreaseEnemies();
        }
    }

    private void IncreaseEnemies() {
        _maxEnemiesInWave += 1;
    }
}