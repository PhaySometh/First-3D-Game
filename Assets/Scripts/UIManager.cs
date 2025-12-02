using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Manager - Manages health and stamina bars display
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Health Bar")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Color healthGoodColor = Color.green;
    public Color healthMediumColor = Color.yellow;
    public Color healthLowColor = Color.red;

    [Header("Stamina Bar")]
    public Image staminaBarFill;
    public TextMeshProUGUI staminaText;
    public Color staminaFullColor = Color.yellow;
    public Color staminaDrainingColor = new Color(1f, 0.65f, 0f); // Orange
    public Color staminaEmptyColor = Color.gray;

    private PlayerHealth playerHealth;
    private PlayerStamina playerStamina;

    private void Start()
    {
        // Find player components
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerStamina = FindObjectOfType<PlayerStamina>();

        if (playerHealth == null)
            Debug.LogError("❌ PlayerHealth component not found!");
        if (playerStamina == null)
            Debug.LogError("❌ PlayerStamina component not found!");

        Debug.Log("✓ UIManager initialized successfully!");
    }

    private void Update()
    {
        if (playerHealth != null)
        {
            UpdateHealthBar();
        }

        if (playerStamina != null)
        {
            UpdateStaminaBar();
        }
    }

    /// <summary>
    /// Update health bar fill amount and color
    /// </summary>
    private void UpdateHealthBar()
    {
        float healthPercent = playerHealth.GetHealthPercentage();

        // Update fill amount
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = healthPercent;
        }

        // Update color
        if (healthPercent > 0.5f)
        {
            healthBarFill.color = Color.Lerp(healthMediumColor, healthGoodColor, (healthPercent - 0.5f) / 0.5f);
        }
        else if (healthPercent > 0.25f)
        {
            healthBarFill.color = Color.Lerp(healthLowColor, healthMediumColor, (healthPercent - 0.25f) / 0.25f);
        }
        else
        {
            healthBarFill.color = healthLowColor;
        }
    }

    /// <summary>
    /// Update stamina bar fill amount and color
    /// </summary>
    private void UpdateStaminaBar()
    {
        float staminaPercent = playerStamina.GetStaminaPercentage();

        // Update fill amount
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = staminaPercent;
        }

        // Update color
        if (staminaPercent > 0.3f)
        {
            staminaBarFill.color = staminaFullColor;
        }
        else if (staminaPercent > 0.1f)
        {
            staminaBarFill.color = Color.Lerp(staminaDrainingColor, staminaFullColor, (staminaPercent - 0.1f) / 0.2f);
        }
        else
        {
            staminaBarFill.color = staminaEmptyColor;
        }
    }
}
