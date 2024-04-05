using UnityEngine;

[RequireComponent(typeof(EnemyView))]
public class EnemyController : MonoBehaviour {
    private EnemyModel _model;
    private EnemyView _view;
    private MainBuilding _mainBuilding;

    public EnemyModel Model => _model; 
    
    public void Construct(MainBuilding mainBuilding) {
        _model = new EnemyModel();
        _view = GetComponent<EnemyView>();
        _view.Construct(_model);
        _mainBuilding = mainBuilding;
    }

    public void StopAttack() {
        _mainBuilding.TakeDamage(_model.Damage);
        _model.IsAttack = false;
    }

    public void Death() {
        EnemySpawner.EnemyList.Remove(this);
        Destroy(gameObject); 
    }

    private void Update() {
        float moveDistance = _model.MoveSpeed * Time.deltaTime;
        Vector3 moveDirection = FindTarget(_mainBuilding.transform);
        bool canMove = moveDirection.x < -0.01;
        _model.IsRun = canMove;

        if (canMove) transform.position += moveDirection * moveDistance;
        else if (!_model.IsAttack) Attack();
    }

    private Vector2 FindTarget(Transform target) {
        Vector3 movementVector = new Vector3(target.position.x - transform.position.x, 0, 0).normalized;
        return movementVector;
    }
    private void Attack() {
        _model.IsAttack = true;
    }
}