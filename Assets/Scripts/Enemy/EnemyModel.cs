using System;

public class EnemyModel {
    public Action OnDied;

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
            _isAttack = value;
        } 
    }

    public void TakeDamage(int damage) {
        _health -= damage;
        if (_health <= 0) OnDied();
    }
}