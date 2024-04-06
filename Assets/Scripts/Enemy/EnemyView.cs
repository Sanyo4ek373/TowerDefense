using UnityEngine;

public class EnemyView : MonoBehaviour {
    private Animator _animator;
    private EnemyModel _model;

    private const string IS_DEATH = "Death";
    private const string IS_RUN = "IsRun";
    private const string IS_ATTACK = "IsAttack";

    public void Construct(EnemyModel model) {
        _animator = GetComponent<Animator>();
        _model = model;
        _model.OnDied += OnDied; 
    }

    private void Update() {
        _animator.SetBool(IS_RUN, _model.IsRun);
        _animator.SetBool(IS_ATTACK, _model.IsAttack);
    }

    private void OnDestroy() {
        _model.OnDied -= OnDied;
    }

    private void OnDied() {
        _animator.SetTrigger(IS_DEATH);
    }
}