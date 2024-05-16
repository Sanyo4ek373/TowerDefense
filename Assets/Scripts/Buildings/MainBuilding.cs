using System;
using UnityEngine;

public class MainBuilding : MonoBehaviour {
    public Action OnBuildingDestroy;
    public Action<int> OnTakeDamage;

    private int _health = 100; 
    
    public void TakeDamage(int damage) {
        _health -= damage;

        int currentHealth = Mathf.Clamp(_health, 0, _health);
        OnTakeDamage(currentHealth);
        
        if (_health <= 0) OnBuildingDestroy();
    }
}