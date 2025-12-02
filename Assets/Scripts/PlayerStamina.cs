using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Player Stamina System - Manages stamina drain during sprint and regeneration
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    private float currentStamina;

    [Header("Drain Settings")]
    public float staminaDrainRate = 30f; // Stamina lost per second while sprinting

    [Header("Regeneration Settings")]
    public float staminaRegenRate = 20f; // Stamina gained per second while not sprinting
    public float regenDelayAfterSprint = 0.5f; // Delay before regen starts after sprinting stops
    private float timeSinceStoppedSprinting = 0f;

    private bool canSprint = true;
    private float lastSprintTime = 0f;

    private void Start()
    {
        currentStamina = maxStamina;
        Debug.Log($"⚡ PlayerStamina initialized. Max Stamina: {maxStamina}");
    }

    private void Update()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isSprinting && canSprint)
        {
            DrainStamina();
            lastSprintTime = Time.time;
        }
        else
        {
            RegenerateStamina();
        }
    }

    /// <summary>
    /// Drain stamina while sprinting
    /// </summary>
    private void DrainStamina()
    {
        currentStamina -= staminaDrainRate * Time.deltaTime;

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            canSprint = false; // Prevent sprinting when stamina is depleted
            Debug.Log("⚠️ Stamina depleted! Cannot sprint.");
        }
    }

    /// <summary>
    /// Regenerate stamina while not sprinting
    /// </summary>
    private void RegenerateStamina()
    {
        // Wait for delay before regenerating
        timeSinceStoppedSprinting = Time.time - lastSprintTime;

        if (timeSinceStoppedSprinting >= regenDelayAfterSprint)
        {
            currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);

            // Allow sprinting again when stamina is high enough
            if (currentStamina >= maxStamina * 0.2f) // Can sprint at 20% stamina
            {
                canSprint = true;
            }
        }
    }

    /// <summary>
    /// Check if player can sprint
    /// </summary>
    public bool CanSprint()
    {
        return canSprint && currentStamina > 0;
    }

    /// <summary>
    /// Get current stamina
    /// </summary>
    public float GetStamina()
    {
        return currentStamina;
    }

    /// <summary>
    /// Get stamina percentage (0-1)
    /// </summary>
    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }

    /// <summary>
    /// Manually add stamina (for potions, etc)
    /// </summary>
    public void AddStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        Debug.Log($"⚡ Stamina added! Current: {currentStamina}/{maxStamina}");
    }
}
