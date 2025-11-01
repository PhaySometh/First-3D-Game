using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
    public GameObject gameOverPanel; // NEW: Game Over Panel
    public TextMeshProUGUI gameOverText; // NEW: Game Over Text
    public Button replayButton; // NEW: Replay Button

    [Header("Coin System")]
    public int totalCoins = 0;
    public int totalExperience = 0;
    public int playerLevel = 1;
    public int expToNextLevel = 100;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

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

        // Setup survival time text at TOP-LEFT during gameplay
        if (survivalTimeText != null)
        {
            survivalTimeText.text = "SURVIVAL TIME: 00:00";
            survivalTimeText.fontSize = 64;
            survivalTimeText.alignment = TextAlignmentOptions.TopLeft;
            
            // Set to top-left anchor
            RectTransform rectTransform = survivalTimeText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.anchoredPosition = new Vector2(10, -10);
            }
        }

        // Setup Replay Button
        if (replayButton != null)
        {
            replayButton.onClick.AddListener(ReplayGame);
        }
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

        if (player == null)
        {
            Debug.LogError("Player not assigned in GameManager!");
            return;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validSpawnFound = false;

            // Try to find a valid spawn position on the NavMesh
            for (int attempts = 0; attempts < 10; attempts++)
            {
                // Calculate spawn position in a circle around player
                float angle = (360f / numberOfEnemies) * i + (attempts * 36f);
                float radians = angle * Mathf.Deg2Rad;
                float currentRadius = spawnRadius + (attempts * 5f);
                
                float spawnX = player.position.x + Mathf.Cos(radians) * currentRadius;
                float spawnZ = player.position.z + Mathf.Sin(radians) * currentRadius;
                
                spawnPosition = new Vector3(spawnX, spawnHeight + 10f, spawnZ);

                // Sample NavMesh to find valid position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPosition, out hit, 5f, NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;
                    validSpawnFound = true;
                    break;
                }
            }

            if (!validSpawnFound)
            {
                Debug.LogWarning($"Could not find valid spawn position for Enemy_{i + 1}");
                continue;
            }

            // Instantiate enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = $"Enemy_{i + 1}";

            // Setup enemy
            EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
            if (enemyAi != null)
            {
                enemyAi.player = player;
                Debug.Log($"Spawned {enemy.name} at {spawnPosition}");
            }
            else
            {
                Debug.LogError($"Enemy prefab doesn't have EnemyAi script!");
                Destroy(enemy);
            }

            activeEnemies.Add(enemy);
        }

        Debug.Log($"Successfully spawned {activeEnemies.Count} enemies!");
    }

    /// <summary>
    /// Call this when player is caught by enemy
    /// </summary>
    public void PlayerCaught()
    {
        gameActive = false;
        Debug.Log("Game Over! Survived for: " + survivalTime + " seconds");

        // FREEZE THE GAME - Set time scale to 0
        Time.timeScale = 0f;

        // SHOW THE CURSOR so player can click replay button
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Show Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Update Game Over Text
        if (gameOverText != null)
        {
            gameOverText.text = $"<color=red><b>GAME OVER!</b></color>\n\n<color=yellow>SURVIVED: {(int)survivalTime} seconds</color>";
        }

        // Hide survival timer
        if (survivalTimeText != null)
        {
            survivalTimeText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Restart the game
    /// </summary>
    public void ReplayGame()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Hide cursor again (for gameplay)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Get current survival time
    /// </summary>
    public float GetSurvivalTime()
    {
        return survivalTime;
    }

    /// <summary>
    /// Add coins when collected
    /// </summary>
    public void AddScore(int amount)
    {
        totalCoins += amount;
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins;
        }
        Debug.Log("💰 Total Coins: " + totalCoins);
    }
    
    /// <summary>
    /// Add experience and check for level up
    /// </summary>
    public void AddExperience(int amount)
    {
        totalExperience += amount;
        
        // Check for level up
        while (totalExperience >= expToNextLevel)
        {
            playerLevel++;
            totalExperience -= expToNextLevel;
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.5f);
            
            if (levelText != null)
            {
                levelText.text = "Level: " + playerLevel;
            }
            
            Debug.Log("🎉 LEVEL UP! Now level " + playerLevel);
        }
        
        if (expText != null)
        {
            expText.text = "EXP: " + totalExperience + "/" + expToNextLevel;
        }
    }
}
