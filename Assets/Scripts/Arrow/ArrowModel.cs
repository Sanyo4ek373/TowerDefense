using UnityEngine;

public class ArrowModel {
    private float _moveSpeed = 15f;
    private int _damage = 1;
    private  float _destroyDistance = 0.5f;
    private Vector3 _moveDirection;

    public float MoveSpeed { get => _moveSpeed; }
    public int Damage { get => _damage; }
    public float DestroyDistance { get => _destroyDistance; }
    public Vector3 MoveDirection {get =>_moveDirection; }
    
    public ArrowModel (Vector3 moveDirection) {
        _moveDirection = moveDirection;
    }
}