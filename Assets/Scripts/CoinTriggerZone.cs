using UnityEngine;

/// <summary>
/// Trigger zone that spawns coins when player enters
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class CoinTriggerZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("The coin spawner to trigger")]
    public CoinSpawner coinSpawner;
    
    [Tooltip("Can this zone trigger multiple times?")]
    public bool canTriggerMultipleTimes = false;
    
    [Tooltip("Cooldown between triggers (if multiple triggers enabled)")]
    public float triggerCooldown = 10f;
    
    [Header("Visual")]
    [Tooltip("Show trigger zone in editor")]
    public bool showGizmo = true;
    
    [Tooltip("Gizmo color")]
    public Color gizmoColor = new Color(0, 1, 1, 0.3f); // Cyan transparent
    
    private bool hasTriggered = false;
    private float lastTriggerTime = -999f;
    private BoxCollider triggerCollider;
    
    void Start()
    {
        // Setup trigger collider
        triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        
        // Find spawner if not assigned
        if (coinSpawner == null)
        {
            coinSpawner = FindObjectOfType<CoinSpawner>();
            if (coinSpawner == null)
            {
                Debug.LogError("CoinTriggerZone: No CoinSpawner found!");
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (other.CompareTag("Player"))
        {
            // Check if can trigger
            if (!canTriggerMultipleTimes && hasTriggered)
            {
                return;
            }
            
            // Check cooldown
            if (Time.time - lastTriggerTime < triggerCooldown)
            {
                return;
            }
            
            // Trigger coin spawn
            if (coinSpawner != null)
            {
                coinSpawner.SpawnCoinsNearPlayer();
                hasTriggered = true;
                lastTriggerTime = Time.time;
                
                Debug.Log("🎯 Coin trigger zone activated!");
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null) return;
        
        // Draw trigger zone
        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);
        
        // Draw wireframe
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(col.center, col.size);
    }
}
