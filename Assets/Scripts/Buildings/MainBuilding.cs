using System;
using UnityEngine;

public class MainBuilding : MonoBehaviour {
    public Action OnBuildingDestroy;
    public Action<int> OnTakeDamage;

    private static int health = 100; 
    
    public void TakeDamage(int damage) {
        health -= damage;

        int currentHealth = Mathf.Clamp(health, 0, health);
        OnTakeDamage(currentHealth);
        
        if (health <= 0) OnBuildingDestroy();
    }
}