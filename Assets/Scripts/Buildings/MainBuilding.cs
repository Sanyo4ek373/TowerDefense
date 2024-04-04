using UnityEngine;
using UnityEngine.Events;

public class MainBuilding : MonoBehaviour {
    private int _health = 100;
    public static MainBuilding Instance = null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    public void TakeDamage(int damage) {
        OnTakeDamage.Invoke(_health);
        _health -= damage;

        if (_health <= 0) {
            OnBuildingDestroy.Invoke();
        }
    }

    public UnityEvent OnBuildingDestroy = new();
    
    public UnityEvent<int> OnTakeDamage =  new();
}
