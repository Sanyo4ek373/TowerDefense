using System;
using UnityEngine;
using UnityEngine.Diagnostics;

public class ArrowController : MonoBehaviour {
    private ArrowModel _model;
    private bool _isHit = false;

    private void Update() {
        if (_isHit) return;

        transform.position += _model.MoveDirection * _model.MoveSpeed * Time.deltaTime;
        
        float angle = GetAngle(_model.MoveDirection);
        transform.eulerAngles = new Vector3(0, 0, angle);

        foreach (EnemyController enemy in EnemySpawner.EnemyList) {
            if (Vector3.Distance(transform.position, enemy.transform.position) < _model.DestroyDistance ) {
                enemy.Model.TakeDamage(_model.Damage);
                _isHit = true;
                ArrowDestroy();
                break;
            }
        }

        if (Math.Abs(transform.position.x) > 8 || Math.Abs(transform.position.y) > 3) Destroy(gameObject);
    }

    private float GetAngle(Vector3 direction) {
        direction = direction.normalized;
        float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        return n - 90;
    }

    public void Construct(Vector3 targetPosition) {
        Vector3 moveDirection = targetPosition - transform.position;
        _model = new ArrowModel(moveDirection.normalized);
    }

    private void ArrowDestroy() {
        Destroy(gameObject);
    }
}
