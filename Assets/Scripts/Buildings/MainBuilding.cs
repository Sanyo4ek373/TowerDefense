using System;
using UnityEngine;

public class MainBuilding : MonoBehaviour {
    public Action OnBuildingDestroy;
    public Action<int> OnTakeDamage;

    private int _health = 100;
    
    public void TakeDamage(int damage) {
        OnTakeDamage(_health);
        
        _health -= damage;
        if (_health <= 0) OnBuildingDestroy();
    }
}