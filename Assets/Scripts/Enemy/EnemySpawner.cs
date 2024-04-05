using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private Transform[] _spawnPlaces;
    [SerializeField] private GameObject[] _troops;
    [SerializeField] private MainBuilding _mainBuilding;
    [SerializeField] private float _spawnTime;
    
    private bool _isTroopsSpawned = true;
    private int _waveNumber;

    public int WaveNumber => _waveNumber;

    public static List<EnemyController> EnemyList = new();

    private void Awake() {
        EnemyList.Clear();
    }

    private void Update() {
        if(_isTroopsSpawned) { 
            for (int i = Random.Range(0, 5); i <4; i++) {
                StartCoroutine(SpawnTroops(_spawnTime, _troops[0], _spawnPlaces[i]));
            }
            
            _waveNumber += 1;
            float newSpawnTime = _waveNumber/5f;
            _spawnTime -= _spawnTime <=3 ? 0 : newSpawnTime;
        }
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
}