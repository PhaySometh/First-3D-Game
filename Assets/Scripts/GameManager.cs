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
    public GameObject enemyPrefab2; // NEW: Second enemy type
    public Transform player;

    [Header("Spawn Settings")]
    public int numberOfEnemies = 3; // Initial enemies
    public float spawnDistance = 25f; // Spawn nearby (visible)
    public float spawnHeight = 1f;
    public float spawnInterval = 3f; // Initial spawn interval (3 seconds - reasonable start)
    public float minSpawnInterval = 0.5f; // Minimum spawn interval (0.5 seconds - very hard)
    public bool spawnGradually = true;
    
    [Header("Difficulty Settings")]
    public bool increaseDifficulty = true;
    public float difficultyMultiplier = 0.92f; // Multiply spawn interval (0.92 = 8% faster each second - more aggressive!)
    // NOTE: No max enemies - they keep spawning forever!

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

    // For gradual enemy spawning
    private int enemiesSpawned = 0;
    private float spawnTimer = 0f;
    private float currentSpawnInterval; // NEW: Dynamic spawn interval
    private float difficultyTimer = 0f; // NEW: Track difficulty progression
    
    // NEW: Player health reference
    private PlayerHealth playerHealth;

    private void Start()
    {
        // IMPORTANT: Reset time scale in case scene was paused
        Time.timeScale = 1f;
        
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
                playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // NEW: Get player health component
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogWarning("⚠️ PlayerHealth component not found on player!");
            }
        }

        Debug.Log($"🎮 GameManager started. Player found: {(player != null ? "YES" : "NO")}");

        // Initialize spawn interval
        currentSpawnInterval = spawnInterval;
        spawnTimer = 2.5f; // DELAY: Wait 2.5 seconds before first enemy spawns - gives player time to prepare!
        enemiesSpawned = 0;

        // UPDATED: Don't spawn immediately - let player prepare for 2-3 seconds
        Debug.Log($"🎮 Game Started! First enemy will spawn in 2-3 seconds...");

        // Update UI
        if (objectiveText != null)
            objectiveText.text = "OBJECTIVE: SURVIVE!\nEvade the enemies as long as possible!";

        if (instructionText != null)
            instructionText.text = "WASD: Move | MOUSE: Look Around | SHIFT: Run (Stamina) | SPACE: Jump";

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
            
            // Configure button RectTransform for proper clickable area
            RectTransform buttonRect = replayButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                // Position button at bottom center of screen
                buttonRect.anchorMin = new Vector2(0.5f, 0); // Bottom center
                buttonRect.anchorMax = new Vector2(0.5f, 0); // Bottom center
                buttonRect.pivot = new Vector2(0.5f, 0); // Bottom center pivot
                buttonRect.anchoredPosition = new Vector2(0, 50); // 50 pixels up from bottom
                
                // Make sure the button is large enough to click (minimum 200x60 pixels)
                if (buttonRect.sizeDelta.x < 200 || buttonRect.sizeDelta.y < 60)
                {
                    buttonRect.sizeDelta = new Vector2(200, 60);
                    Debug.Log($"✓ Button size adjusted to: {buttonRect.sizeDelta}");
                }
                
                Debug.Log($"✓ Button positioned at bottom center. Position={buttonRect.anchoredPosition}, Size={buttonRect.sizeDelta}");
            }
            
            Debug.Log("✓ Replay button listener ADDED successfully!");
        }
        else
        {
            Debug.LogError("❌ REPLAY BUTTON IS NULL! Please assign it in the Inspector!");
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

        // Update difficulty every second
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= 1f && increaseDifficulty)
        {
            difficultyTimer = 0f;
            IncreaseDifficulty();
        }

        // Spawn enemies continuously - NO LIMIT!
        if (spawnGradually)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnSingleEnemy();
                spawnTimer = currentSpawnInterval;
            }
        }
    }

    /// <summary>
    /// NEW: Increase difficulty as player survives
    /// </summary>
    private void IncreaseDifficulty()
    {
        // Make spawn interval shorter (faster spawning)
        currentSpawnInterval *= difficultyMultiplier;
        currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);
        
        // Calculate enemies per minute for clarity
        float enemiesPerMinute = 60f / currentSpawnInterval;
        Debug.Log($"⚡ DIFFICULTY UP! Spawn interval: {currentSpawnInterval:F2}s ({enemiesPerMinute:F0} enemies/min) | Time survived: {survivalTime:F0}s");
    }

    /// <summary>
    /// Spawn all enemies immediately (old behavior)
    /// </summary>
    private void SpawnEnemiesImmediate()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnSingleEnemy();
        }
    }

    /// <summary>
    /// Spawn a single enemy at a random valid position
    /// </summary>
    private void SpawnSingleEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("❌ Enemy prefab not assigned in GameManager!");
            return;
        }

        if (player == null)
        {
            Debug.LogError("❌ Player not assigned in GameManager!");
            return;
        }

        Debug.Log($"📍 Attempting to spawn enemy #{enemiesSpawned + 1} near player at {player.position}");


        Vector3 spawnPosition = Vector3.zero;
        bool validSpawnFound = false;

        // Try to find a valid spawn position on the NavMesh
        for (int attempts = 0; attempts < 10; attempts++)
        {
            // IMPROVED: Spawn around player but AVOID the front direction
            // Get player's forward direction
            Vector3 playerForward = player.forward; // Player's forward direction
            
            // Choose spawn angle: AVOID front (0-90 degrees), prefer LEFT/RIGHT/BEHIND
            // Angles: 0° = front, 90° = right, 180° = back, 270° = left
            float angle;
            int zone = Random.Range(0, 3); // 0=left (270±45), 1=right (90±45), 2=back (180±45)
            
            if (zone == 0) // LEFT SIDE
            {
                angle = Random.Range(225f, 315f); // Left side (270 ± 45 degrees)
            }
            else if (zone == 1) // RIGHT SIDE
            {
                angle = Random.Range(45f, 135f); // Right side (90 ± 45 degrees)
            }
            else // BACK SIDE
            {
                angle = Random.Range(135f, 225f); // Behind (180 ± 45 degrees)
            }
            
            float radians = angle * Mathf.Deg2Rad;
            
            // Spawn MUCH further away (35-50 units) - gives player time to see and react
            float randomDistance = Random.Range(35f, 50f);
            
            float spawnX = player.position.x + Mathf.Cos(radians) * randomDistance;
            float spawnZ = player.position.z + Mathf.Sin(radians) * randomDistance;
            
            spawnPosition = new Vector3(spawnX, spawnHeight + 5f, spawnZ);
            
            Debug.Log($"   Attempt #{attempts + 1}: Zone={zone}, Angle={angle:F0}°, Distance={randomDistance:F1}m, Position={spawnPosition}");

            // Sample NavMesh to find valid position - IMPROVED: larger search radius
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 10f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                validSpawnFound = true;
                Debug.Log($"   ✓ Valid position found on NavMesh: {spawnPosition}");
                break;
            }
        }

        if (!validSpawnFound)
        {
            // FALLBACK: If NavMesh sampling fails, still spawn enemy but close to player
            spawnPosition = player.position + new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
            Debug.LogWarning($"⚠️ NavMesh spawn failed after 10 attempts, using fallback position: {spawnPosition}");
        }

        // Randomly choose between enemy type 1 and 2
        GameObject selectedPrefab = enemyPrefab;
        if (enemyPrefab2 != null && Random.value > 0.5f)
        {
            selectedPrefab = enemyPrefab2;
        }

        // Instantiate enemy
        GameObject enemy = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        enemy.name = $"Enemy_{enemiesSpawned + 1}";

        // Setup enemy
        EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
        if (enemyAi != null)
        {
            enemyAi.player = player;
            Debug.Log($"🔴 Spawned {enemy.name} at distance {Vector3.Distance(spawnPosition, player.position):F1} from player");
        }
        else
        {
            Debug.LogError($"Enemy prefab doesn't have EnemyAi script!");
            Destroy(enemy);
        }

        activeEnemies.Add(enemy);
        enemiesSpawned++;
    }

    /// <summary>
    /// OLD: Spawn multiple enemies around the player (mix of both types)
    /// </summary>
    private void SpawnEnemies()
    {
        if (spawnGradually)
        {
            // Spawn gradually in Update
            return;
        }
        
        // Spawn all at once
        SpawnEnemiesImmediate();
    }

    /// <summary>
    /// Call this when player is caught by enemy (or dies from health reaching 0)
    /// </summary>
    public void PlayerCaught()
    {
        gameActive = false;
        Debug.Log("Game Over! Survived for: " + survivalTime + " seconds");

        // PAUSE CAMERA - Unlock mouse and stop camera input
        if (Camera.main != null)
        {
            ThirdPersonCameraController cameraController = Camera.main.GetComponent<ThirdPersonCameraController>();
            if (cameraController != null)
            {
                cameraController.PauseCamera();
            }
        }

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
            // NEW: Show health or status
            string deathReason = "Health depleted";
            if (playerHealth != null)
            {
                deathReason = $"Health: {(int)playerHealth.GetHealth()}/100";
            }
            gameOverText.text = $"<color=red><b>GAME OVER!</b></color>\n\n<color=yellow>SURVIVED: {(int)survivalTime} seconds</color>\n<color=orange>{deathReason}</color>";
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
        Debug.Log("🔄 REPLAY BUTTON CLICKED! Starting replay...");
        
        // Resume time FIRST before anything else
        Time.timeScale = 1f;
        Debug.Log("⏱️ Time.timeScale set to 1f");
        
        // Hide UI elements before reload
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Reload scene directly without coroutine to avoid timing issues
        Debug.Log("🔄 Reloading scene now...");
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
