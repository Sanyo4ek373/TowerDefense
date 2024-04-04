using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private Transform[] _spawnPlaces;
    [SerializeField] private GameObject[] _troops;
    [SerializeField] private float _spawnTime;
    
    private bool _isTroopsSpawned = true;
    private int _vaweNumber;

    public int VaweNumber => _vaweNumber;

    public static List<EnemyController> EnemyList = new();

    private void Awake() {
        EnemyList.Clear();
    }

    private void Update() {
        if(_isTroopsSpawned) { 
            for (int i = Random.Range(0, 5); i <4; i++) {
                StartCoroutine(SpawnTroops(_spawnTime, _troops[0], _spawnPlaces[i]));
            }
            
            _vaweNumber += 1;
            float newSpawnTime = _vaweNumber/5f;
            _spawnTime -= _spawnTime <=3 ? 0 : newSpawnTime;
        }
    }

    private IEnumerator SpawnTroops(float waitTime, GameObject troop, Transform spawnPlace) {
        _isTroopsSpawned = false;

        EnemyList.Add(
            Instantiate(troop, spawnPlace.position, Quaternion.identity)
            .GetComponent<EnemyController>()
        );

        yield return new WaitForSeconds(waitTime);

        _isTroopsSpawned = true;
    }
}