using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Player Health System - Manages player health and death
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Damage")]
    public float damagePerHit = 25f; // Damage taken per enemy collision
    public float damageCooldown = 1f; // Cooldown between damage hits
    private float lastDamageTime = 0f;

    private bool isAlive = true;
    private GameManager gameManager;

    private void Start()
    {
        currentHealth = maxHealth;
        gameManager = FindObjectOfType<GameManager>();
        
        Debug.Log($"🏥 PlayerHealth initialized. Max Health: {maxHealth}");
    }

    /// <summary>
    /// Take damage from enemy
    /// </summary>
    public void TakeDamage(float damage)
    {
        // Cooldown to prevent multiple hits in same frame
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;
        currentHealth -= damage;
        
        Debug.Log($"❤️ Player took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"💚 Player healed! Health: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Player death logic
    /// </summary>
    private void Die()
    {
        isAlive = false;
        Debug.Log("💀 Player is dead!");

        if (gameManager != null)
        {
            gameManager.PlayerCaught(); // Use existing game over system
        }
    }

    /// <summary>
    /// Get current health
    /// </summary>
    public float GetHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Get health percentage (0-1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// Check if player is alive
    /// </summary>
    public bool IsAlive()
    {
        return isAlive;
    }
}
