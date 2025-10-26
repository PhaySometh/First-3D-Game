using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Game Manager - Controls the chase game
/// Spawns enemies and manages game state
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int numberOfEnemies = 3;
    public float spawnRadius = 50f;
    public float spawnHeight = 1f;

    [Header("UI")]
    public TextMeshProUGUI survivalTimeText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI instructionText;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float survivalTime = 0f;
    private bool gameActive = true;

    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
                playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Spawn enemies
        SpawnEnemies();

        // Update UI
        if (objectiveText != null)
            objectiveText.text = "OBJECTIVE: SURVIVE!\nEvade the enemies as long as possible!";

        if (instructionText != null)
            instructionText.text = "WASD: Move | MOUSE: Look Around | SHIFT: Run | SPACE: Jump";
    }

    private void Update()
    {
        if (!gameActive)
            return;

        // Update survival time
        survivalTime += Time.deltaTime;

        if (survivalTimeText != null)
        {
            int minutes = (int)(survivalTime / 60f);
            int seconds = (int)(survivalTime % 60f);
            survivalTimeText.text = $"SURVIVAL TIME: {minutes:00}:{seconds:00}";
        }
    }

    /// <summary>
    /// Spawn multiple enemies around the player
    /// </summary>
    private void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned in GameManager!");
            return;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Calculate random spawn position
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;
            randomDirection = randomDirection.normalized;

            Vector3 spawnPosition = player.position + randomDirection * spawnRadius;
            spawnPosition.y = spawnHeight;

            // Instantiate enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = $"Enemy_{i + 1}";

            // Setup enemy
            EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
            if (enemyAi != null)
            {
                enemyAi.player = player;
            }

            activeEnemies.Add(enemy);
            Debug.Log($"Spawned {enemy.name} at {spawnPosition}");
        }

        Debug.Log($"Spawned {numberOfEnemies} enemies!");
    }

    /// <summary>
    /// Call this when player is caught by enemy
    /// </summary>
    public void PlayerCaught()
    {
        gameActive = false;
        Debug.Log("Game Over! Survived for: " + survivalTime + " seconds");

        if (survivalTimeText != null)
            survivalTimeText.text = $"GAME OVER! SURVIVED: {(int)survivalTime} seconds";
    }

    /// <summary>
    /// Get current survival time
    /// </summary>
    public float GetSurvivalTime()
    {
        return survivalTime;
    }
}
