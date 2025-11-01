using UnityEngine;

/// <summary>
/// Enhanced Coin Collectible with physics, animations, and particle effects
/// </summary>
public class CoinCollectible : MonoBehaviour
{
    [Header("Coin Value")]
    [Tooltip("Points awarded when collected")]
    public int coinValue = 10;
    
    [Tooltip("Experience points awarded")]
    public int expValue = 5;
    
    [Header("Physics - Homework Requirements")]
    [Tooltip("Upward force when coin spawns (AddForce)")]
    public float spawnForce = 8f;
    
    [Tooltip("Bounce force when hitting ground")]
    public float bounceForce = 3f;
    
    [Tooltip("Should apply spawn force on start?")]
    public bool applySpawnForce = true;
    
    [Header("Visual Effects")]
    [Tooltip("Rotation speed (degrees per second)")]
    public float rotationSpeed = 180f;
    
    [Tooltip("Vertical bobbing/floating animation")]
    public bool enableBobbing = true;
    
    [Tooltip("Bobbing height")]
    public float bobbingHeight = 0.2f;
    
    [Tooltip("Bobbing speed")]
    public float bobbingSpeed = 2f;
    
    [Header("Particle Effect")]
    [Tooltip("Particle system to play when collected")]
    public GameObject collectParticlePrefab;
    
    [Header("Audio")]
    [Tooltip("Sound to play when collected")]
    public AudioClip collectSound;
    
    private Rigidbody rb;
    private bool isCollected = false;
    private Vector3 startPosition;
    private float bobbingTimer = 0f;
    private bool hasLanded = false;
    private AudioSource audioSource;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
        
        // Random starting rotation for variety
        bobbingTimer = Random.Range(0f, Mathf.PI * 2f);
        
        // Apply spawn force if enabled (homework requirement!)
        if (applySpawnForce && rb != null)
        {
            ApplySpawnForce();
        }
        else
        {
            // If no spawn force, coin is already placed correctly
            hasLanded = true;
            
            // Make kinematic after landing to prevent rolling away
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }
    
    void Update()
    {
        // Rotate coin continuously
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        
        // Bobbing animation (only after landed)
        if (enableBobbing && hasLanded && !isCollected)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            float newY = startPosition.y + Mathf.Sin(bobbingTimer) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    /// <summary>
    /// Apply upward spawn force (homework requirement: AddForce)
    /// </summary>
    void ApplySpawnForce()
    {
        if (rb == null) return;
        
        // Random direction with upward bias
        Vector3 randomDirection = new Vector3(
            Random.Range(-0.3f, 0.3f),
            1f, // Strong upward
            Random.Range(-0.3f, 0.3f)
        );
        
        // Apply force (homework requirement!)
        rb.AddForce(randomDirection * spawnForce, ForceMode.Impulse);
        
        // Add random spin for visual effect
        rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Detect collision with ground (homework requirement: OnCollisionEnter)
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        // Check if hit terrain or ground (more flexible - works with any ground object)
        if (!hasLanded)
        {
            hasLanded = true;
            startPosition = transform.position;
            
            // Small bounce effect
            if (rb != null && bounceForce > 0)
            {
                rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }
            
            // Stop physics after landing to prevent rolling
            Invoke("StopPhysics", 1f);
            
            Debug.Log($"Coin landed on {collision.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Stop physics movement after coin settles
    /// </summary>
    void StopPhysics()
    {
        if (rb != null && !isCollected)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            startPosition = transform.position;
        }
    }
    
    /// <summary>
    /// Detect player collection (homework requirement: OnTriggerEnter)
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        
        // Check if player collected the coin
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }
    
    /// <summary>
    /// Handle coin collection
    /// </summary>
    void CollectCoin()
    {
        isCollected = true;
        
        // Spawn particle effect
        if (collectParticlePrefab != null)
        {
            GameObject particles = Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, 2f);
        }
        
        // Play sound
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // Award coins and experience
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AddScore(coinValue);
            gm.AddExperience(expValue);
        }
        
        Debug.Log($"💰 Coin collected! +{coinValue} coins, +{expValue} EXP");
        
        // Hide visuals immediately
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        
        // Disable colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        
        // Destroy after sound finishes
        float destroyDelay = (audioSource != null && collectSound != null) ? collectSound.length : 0f;
        Destroy(gameObject, destroyDelay);
    }
}