using System.Collections;
using UnityEngine;

public class DefenseTower : MonoBehaviour {
    [SerializeField] private ArrowController _arrow;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _reloadingTime;
    [SerializeField] private float _attackRange;
    
    private bool _isShotting = true;

    public void ChangeReloadingTime(float time) {
        _reloadingTime -= time;
    }

    private void Update() {
        if (!_isShotting) return;

        foreach (EnemyController enemy in EnemySpawner.EnemyList) {
            if (Vector3.Distance(transform.position, enemy.transform.position) < _attackRange) {
                StartCoroutine(Shoot(_reloadingTime, _arrow, enemy.transform.position));
                return;
            }
        } 
    }

    private IEnumerator Shoot(float waitTime, ArrowController arrow, Vector3 targetPosition) {
        _isShotting = false;

        Instantiate(arrow.gameObject, _shootPosition.position, Quaternion.identity)
            .GetComponent<ArrowController>()
            .Construct(targetPosition);

        yield return new WaitForSeconds(waitTime);

        _isShotting = true;
    }
}