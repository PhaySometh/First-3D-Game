using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy collision detector - Deals damage to player on collision
/// </summary>
public class EnemyCollisionDetector : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 25f; // Damage dealt to player per hit
    public float damageCooldown = 1f; // Cooldown between damage hits
    
    private float lastDamageTime = 0f;
    private bool hasHitPlayer = false;

    private void OnTriggerEnter(Collider collision)
    {
        // Check if it's the player
        if (collision.CompareTag("Player") || collision.gameObject.name.Contains("Player"))
        {
            DealDamageToPlayer(collision.gameObject);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        // Deal continuous damage while in contact
        if (collision.CompareTag("Player") || collision.gameObject.name.Contains("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                DealDamageToPlayer(collision.gameObject);
            }
        }
    }

    /// <summary>
    /// Deal damage to the player
    /// </summary>
    private void DealDamageToPlayer(GameObject playerObject)
    {
        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsAlive())
        {
            lastDamageTime = Time.time;
            playerHealth.TakeDamage(damageAmount);
            Debug.Log($"🎯 Enemy dealt {damageAmount} damage to player!");
        }
    }
}
