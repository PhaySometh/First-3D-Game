using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Advanced coin spawner with proper terrain detection and trigger zones
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Prefab")]
    [Tooltip("The coin prefab to spawn")]
    public GameObject coinPrefab;
    
    [Header("Initial Spawn Settings")]
    [Tooltip("Spawn coins at game start?")]
    public bool spawnOnStart = true;
    
    [Tooltip("Number of coins to spawn initially")]
    public int initialCoinCount = 30;  // More coins!
    
    [Header("Spawn Area")]
    [Tooltip("Center point for spawning (leave empty to use player)")]
    public Transform spawnCenter;
    
    [Tooltip("Minimum distance from center")]
    public float minSpawnRadius = 5f;  // Closer to player!
    
    [Tooltip("Maximum distance from center")]
    public float maxSpawnRadius = 20f;  // Much closer range!
    
    [Tooltip("Height to start raycast from (above terrain)")]
    public float raycastHeight = 100f;
    
    [Header("Terrain Detection")]
    [Tooltip("Layer mask for terrain/ground")]
    public LayerMask terrainLayer = ~0; // Everything by default
    
    [Tooltip("Height above terrain to spawn coins")]
    public float spawnHeightOffset = 0.5f;
    
    [Tooltip("Should coins apply spawn force?")]
    public bool coinsApplySpawnForce = false;
    
    [Header("Trigger Zone Spawning")]
    [Tooltip("Enable trigger-based spawning?")]
    public bool enableTriggerSpawning = true;
    
    [Tooltip("Number of coins to spawn when trigger activated")]
    public int coinsPerTrigger = 3;
    
    [Tooltip("Radius around player to spawn trigger coins")]
    public float triggerSpawnRadius = 10f;
    
    [Tooltip("Cooldown between trigger spawns")]
    public float triggerCooldown = 5f;
    
    [Header("Auto Respawn")]
    [Tooltip("Enable automatic coin respawning?")]
    public bool enableAutoRespawn = true;  // Enable auto-spawn!
    
    [Tooltip("Respawn interval in seconds")]
    public float respawnInterval = 10f;  // Spawn every 10 seconds!
    
    [Tooltip("Maximum coins in scene at once")]
    public int maxCoins = 50;  // Allow more coins!
    
    [Tooltip("Spawn coins near player when they move?")]
    public bool spawnNearPlayerMovement = true;
    
    [Tooltip("Distance player must move to trigger new coin spawn")]
    public float movementThreshold = 15f;
    
    [Header("Debug")]
    [Tooltip("Show spawn area gizmos in editor?")]
    public bool showGizmos = true;
    
    private List<GameObject> activeCoins = new List<GameObject>();
    private float nextRespawnTime = 0f;
    private float lastTriggerTime = -999f;
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;
    
    void Start()
    {
        // Find player
        if (spawnCenter == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                spawnCenter = player.transform;
                playerTransform = player.transform;
            }
            else
            {
                spawnCenter = transform;
                Debug.LogWarning("CoinSpawner: Player not found! Using spawner position.");
            }
        }
        
        // Spawn initial coins
        if (spawnOnStart)
        {
            SpawnInitialCoins();
        }
        
        // Track initial player position
        if (playerTransform != null)
        {
            lastPlayerPosition = playerTransform.position;
        }
    }
    
    void Update()
    {
        // Clean up destroyed coins from list
        activeCoins.RemoveAll(coin => coin == null);
        
        // Spawn coins as player moves
        if (spawnNearPlayerMovement && playerTransform != null)
        {
            float distanceMoved = Vector3.Distance(playerTransform.position, lastPlayerPosition);
            
            if (distanceMoved >= movementThreshold && activeCoins.Count < maxCoins)
            {
                // Spawn 3-5 coins near player's new position
                int coinsToSpawn = Random.Range(3, 6);
                for (int i = 0; i < coinsToSpawn; i++)
                {
                    Vector3 spawnPos = GetRandomPositionNearPlayer(playerTransform.position, 15f);
                    if (spawnPos != Vector3.zero)
                    {
                        SpawnCoin(spawnPos);
                    }
                }
                
                lastPlayerPosition = playerTransform.position;
                Debug.Log($"🎯 Player moved! Spawned {coinsToSpawn} new coins nearby!");
            }
        }
        
        // Auto respawn
        if (enableAutoRespawn && Time.time >= nextRespawnTime)
        {
            if (activeCoins.Count < maxCoins)
            {
                // Spawn near player, not randomly far away!
                Vector3 spawnPos = playerTransform != null 
                    ? GetRandomPositionNearPlayer(playerTransform.position, 20f)
                    : GetRandomTerrainPosition();
                    
                if (spawnPos != Vector3.zero)
                {
                    SpawnCoin(spawnPos);
                }
                nextRespawnTime = Time.time + respawnInterval;
            }
        }
    }
    
    /// <summary>
    /// Spawn initial batch of coins
    /// </summary>
    void SpawnInitialCoins()
    {
        Debug.Log($"🪙 Spawning {initialCoinCount} initial coins...");
        
        int successCount = 0;
        int attempts = 0;
        int maxAttempts = initialCoinCount * 3; // Safety limit
        
        while (successCount < initialCoinCount && attempts < maxAttempts)
        {
            attempts++;
            Vector3 spawnPos = GetRandomTerrainPosition();
            
            if (spawnPos != Vector3.zero)
            {
                SpawnCoin(spawnPos);
                successCount++;
            }
        }
        
        Debug.Log($"✅ Successfully spawned {successCount} coins!");
    }
    
    /// <summary>
    /// Spawn coins near player (trigger-based)
    /// </summary>
    public void SpawnCoinsNearPlayer()
    {
        if (!enableTriggerSpawning) return;
        
        // Check cooldown
        if (Time.time - lastTriggerTime < triggerCooldown)
        {
            return;
        }
        
        lastTriggerTime = Time.time;
        
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                return;
        }
        
        Debug.Log($"🎯 Spawning {coinsPerTrigger} coins near player!");
        
        for (int i = 0; i < coinsPerTrigger; i++)
        {
            Vector3 spawnPos = GetRandomPositionNearPlayer(playerTransform.position, triggerSpawnRadius);
            
            if (spawnPos != Vector3.zero)
            {
                SpawnCoin(spawnPos);
            }
        }
    }
    
    /// <summary>
    /// Spawn a single coin at specified position
    /// </summary>
    void SpawnCoin(Vector3 position)
    {
        if (coinPrefab == null)
        {
            Debug.LogError("CoinSpawner: No coin prefab assigned!");
            return;
        }
        
        if (activeCoins.Count >= maxCoins)
        {
            return;
        }
        
        // Instantiate coin - use prefab's original rotation, then add random Y spin
        GameObject coin = Instantiate(coinPrefab, position, coinPrefab.transform.rotation);
        coin.transform.Rotate(0, Random.Range(0f, 360f), 0, Space.World);
        coin.transform.parent = transform;
        
        // Configure coin
        CoinCollectible coinScript = coin.GetComponent<CoinCollectible>();
        if (coinScript != null)
        {
            coinScript.applySpawnForce = coinsApplySpawnForce;
        }
        
        activeCoins.Add(coin);
    }
    
    /// <summary>
    /// Get random position on terrain within spawn radius
    /// </summary>
    Vector3 GetRandomTerrainPosition()
    {
        if (spawnCenter == null) return Vector3.zero;
        
        // Random angle and radius
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
        
        // Calculate horizontal position
        Vector3 randomOffset = new Vector3(
            Mathf.Cos(angle) * radius,
            0,
            Mathf.Sin(angle) * radius
        );
        
        Vector3 targetPos = spawnCenter.position + randomOffset;
        
        // Raycast down to find terrain
        return GetTerrainHeightAt(targetPos);
    }
    
    /// <summary>
    /// Get random position near a specific point
    /// </summary>
    Vector3 GetRandomPositionNearPlayer(Vector3 center, float radius)
    {
        // Random position in circle
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 targetPos = center + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        // Get terrain height at that position
        return GetTerrainHeightAt(targetPos);
    }
    
    /// <summary>
    /// Get terrain height at specific XZ position using raycast
    /// </summary>
    Vector3 GetTerrainHeightAt(Vector3 position)
    {
        // Start raycast from high above
        Vector3 rayStart = new Vector3(position.x, position.y + raycastHeight, position.z);
        
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastHeight * 2f, terrainLayer))
        {
            // Found terrain! Return position slightly above it
            return hit.point + Vector3.up * spawnHeightOffset;
        }
        
        // No terrain found
        Debug.LogWarning($"CoinSpawner: No terrain found at position {position}");
        return Vector3.zero;
    }
    
    /// <summary>
    /// Get current coin count
    /// </summary>
    public int GetActiveCoinCount()
    {
        activeCoins.RemoveAll(coin => coin == null);
        return activeCoins.Count;
    }
    
    /// <summary>
    /// Draw gizmos for spawn area
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!showGizmos || spawnCenter == null) return;
        
        // Draw spawn radius circles
        Gizmos.color = Color.yellow;
        DrawCircle(spawnCenter.position, minSpawnRadius, Color.yellow);
        
        Gizmos.color = Color.green;
        DrawCircle(spawnCenter.position, maxSpawnRadius, Color.green);
        
        // Draw trigger radius
        if (enableTriggerSpawning && playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            DrawCircle(playerTransform.position, triggerSpawnRadius, Color.cyan);
        }
    }
    
    void DrawCircle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;
        int segments = 32;
        float angleStep = 360f / segments;
        
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
