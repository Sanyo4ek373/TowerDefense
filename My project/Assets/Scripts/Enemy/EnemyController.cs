using UnityEngine;

[RequireComponent(typeof(EnemyView))]
public class EnemyController : MonoBehaviour {
    [SerializeField] private Transform _target;
    private EnemyModel _model;
    private EnemyView _view;

    public EnemyModel Model => _model; 

    private void Awake() {
        _model = new EnemyModel();
        _view = GetComponent<EnemyView>();
        _view.Construct(_model);
    }

    private void Update() {
        float moveDistance = _model.MoveSpeed * Time.deltaTime;
        Vector3 moveDirection = findTarget(_target);
        bool canMove = moveDirection.x < -0.01;

        _model.IsRun = canMove;

        if (canMove) transform.position += moveDirection * moveDistance;
        else if (!_model.IsAttack) Attack();
    }

    private Vector2 findTarget(Transform target) {
        Vector3 movementVector = new Vector3(target.position.x - transform.position.x, 0, 0).normalized;
        return movementVector;
    }

    private void Attack() {
        _model.IsAttack = true;
    }

    public void StopAttack() {
        MainBuilding.Instance.TakeDamage(_model.Damage);
        _model.IsAttack = false;
    }

    public void Death() {
        EnemySpawner.EnemyList.Remove(this);
        Destroy(gameObject); 
    }
}
