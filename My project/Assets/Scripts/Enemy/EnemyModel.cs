using UnityEngine.Events;

public class EnemyModel {
    private int _health = 5;
    private float _moveSpeed = 1.5f;
    private int _damage = 1;
    private bool _isAttack = false;

    public int Health { get => _health; }
    public float MoveSpeed { get => _moveSpeed; }
    public int Damage { get => _damage; }
    public bool IsRun { get; set; }
    public bool IsAttack { get => _isAttack; 
        set {
            if (value) OnAttack.Invoke();
            _isAttack = value;
        } 
    }

    public void TakeDamage(int damage) {
        _health -= damage;
        if (_health <= 0) OnDied.Invoke();
    }

    public UnityEvent OnDied = new();

    public UnityEvent OnAttack = new();
}
