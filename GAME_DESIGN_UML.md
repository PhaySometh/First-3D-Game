# Class UML Diagram - Game Implementation

## Class Diagram with Details

```mermaid
classDiagram
    class GameManager {
        +GameObject enemyPrefab
        +GameObject enemyPrefab2
        +Transform player
        +int numberOfEnemies
        +float spawnDistance
        +float spawnInterval
        +bool increaseDifficulty
        +float difficultyMultiplier
        +TextMeshProUGUI survivalTimeText
        +GameObject gameOverPanel
        +int totalCoins
        +int totalExperience
        +int playerLevel
        -List~GameObject~ activeEnemies
        -float survivalTime
        -bool gameActive
        +Start()
        +Update()
        +SpawnSingleEnemy()
        +PlayerCaught()
        +ReplayGame()
        +AddScore(int amount)
        +AddExperience(int amount)
        +IncreaseDifficulty()
    }

    class PlayerMovementScript {
        +Camera playerCamera
        +GameObject characterModel
        +float walkSpeed
        +float runSpeed
        +float jumpPower
        +float gravity
        +float lookSpeed
        -Vector3 moveDirection
        -float rotationX
        -CharacterController characterController
        -bool canMove
        +Start()
        +Update()
    }

    class EnemyAi {
        +NavMeshAgent agent
        +Transform player
        +float sightRange
        +float chaseStopDistance
        +float walkPointRange
        -Vector3 walkPoint
        -bool walkPointSet
        -EnemyState currentState
        +Awake()
        +Update()
        +Patrol()
        +SearchWalkPoint()
        +ChasePlayer()
        +OnDrawGizmosSelected()
    }

    class CoinCollectible {
        +int coinValue
        +int expValue
        +float spawnForce
        +float bounceForce
        +bool applySpawnForce
        +float rotationSpeed
        +bool enableBobbing
        +float bobbingHeight
        +float bobbingSpeed
        +GameObject collectParticlePrefab
        +AudioClip collectSound
        -Rigidbody rb
        -bool isCollected
        -Vector3 startPosition
        -float bobbingTimer
        -bool hasLanded
        -AudioSource audioSource
        +Start()
        +Update()
        +ApplySpawnForce()
        +OnCollisionEnter(Collision collision)
        +StopPhysics()
        +OnTriggerEnter(Collider other)
        +CollectCoin()
    }

    class CoinSpawner {
        +GameObject coinPrefab
        +bool spawnOnStart
        +int initialCoinCount
        +Transform spawnCenter
        +float minSpawnRadius
        +float maxSpawnRadius
        +float raycastHeight
        +LayerMask terrainLayer
        +bool enableTriggerSpawning
        +int coinsPerTrigger
        +bool enableAutoRespawn
        +float respawnInterval
        +int maxCoins
        +bool spawnNearPlayerMovement
        +float movementThreshold
        -List~GameObject~ activeCoins
        -float nextRespawnTime
        -Transform playerTransform
        -Vector3 lastPlayerPosition
        +Start()
        +Update()
        +SpawnInitialCoins()
        +SpawnCoinsNearPlayer()
        +SpawnCoin(Vector3 position)
        +GetRandomTerrainPosition()
        +GetRandomPositionNearPlayer()
        +GetTerrainHeightAt(Vector3 position)
        +GetActiveCoinCount()
        +OnDrawGizmosSelected()
    }

    class PlayerAnimationController {
        -Animator animator
        -CharacterController characterController
        -int speedHash
        -int isMovingHash
        -int isGroundedHash
        -int jumpHash
        -int isCrouchingHash
        -float walkThreshold
        -float runThreshold
        +Start()
        +Update()
        -UpdateAnimationState()
    }

    class EnemyCollisionDetector {
        -bool hasCollided
        +OnTriggerEnter(Collider collision)
        +OnTriggerStay(Collider collision)
    }

    class CoinTriggerZone {
        -CoinSpawner coinSpawner
        +Start()
        +OnTriggerEnter(Collider collision)
    }

    class CoinGlow {
        +Material glowMaterial
        +float glowSpeed
        +float minIntensity
        +float maxIntensity
        -float glowTimer
        +Start()
        +Update()
    }

    %% Relationships
    GameManager --> PlayerMovementScript : controls
    GameManager --> EnemyAi : spawns
    GameManager --> CoinCollectible : spawns
    GameManager --> EnemyCollisionDetector : detects collisions

    EnemyAi --> PlayerMovementScript : chases

    CoinCollectible --> GameManager : notifies
    CoinCollectible --> PlayerAnimationController : interacts

    CoinSpawner --> CoinCollectible : creates
    CoinSpawner --> CoinTriggerZone : manages

    CoinTriggerZone --> CoinSpawner : communicates

    PlayerAnimationController --> PlayerMovementScript : animates

    CoinGlow --> CoinCollectible : effects

    EnemyCollisionDetector --> GameManager : reports catch

```

---

## Inheritance & Relationships

```mermaid
graph TD
    MonoBehaviour["MonoBehaviour<br/>(Unity Base)"]

    GameManager_C["GameManager"]
    PlayerMovement["PlayerMovementScript"]
    EnemyAI_C["EnemyAi"]
    CoinCollectible_C["CoinCollectible"]
    CoinSpawner_C["CoinSpawner"]
    PlayerAnimation["PlayerAnimationController"]
    EnemyCollision["EnemyCollisionDetector"]
    CoinTrigger["CoinTriggerZone"]
    CoinGlow_C["CoinGlow"]

    MonoBehaviour --> GameManager_C
    MonoBehaviour --> PlayerMovement
    MonoBehaviour --> EnemyAI_C
    MonoBehaviour --> CoinCollectible_C
    MonoBehaviour --> CoinSpawner_C
    MonoBehaviour --> PlayerAnimation
    MonoBehaviour --> EnemyCollision
    MonoBehaviour --> CoinTrigger
    MonoBehaviour --> CoinGlow_C

    style MonoBehaviour fill:#FFE4B5
    style GameManager_C fill:#FF6B6B
    style PlayerMovement fill:#4ECDC4
    style EnemyAI_C fill:#95E1D3
    style CoinCollectible_C fill:#FFD93D
    style CoinSpawner_C fill:#FFD93D
    style PlayerAnimation fill:#C7B7D4
    style EnemyCollision fill:#A8DADC
    style CoinTrigger fill:#A8DADC
    style CoinGlow_C fill:#FFE66D
```

---

## Script Descriptions

### **GameManager** (Core Game Controller)

- Manages overall game state
- Spawns enemies with progressive difficulty
- Tracks player survival time
- Handles coin and XP system
- Detects game over condition
- Manages UI updates

### **PlayerMovementScript** (Player Control)

- Handles WASD movement input
- Camera look control with mouse
- Jump and crouch mechanics
- CharacterController integration

### **EnemyAi** (Enemy Behavior)

- NavMesh-based AI movement
- Constant player chasing (no sight limit)
- Patrol pattern when not chasing
- Collision detection for catch

### **CoinCollectible** (Coin Object)

- Physics-based coin spawning
- Rotation and bobbing animations
- Collision and trigger detection
- Score and XP awarding
- Particle effects on collection

### **CoinSpawner** (Coin Management)

- Initial coin spawning
- Dynamic respawning system
- Player-following spawn logic
- Trigger-based spawning
- Terrain detection via raycasting

### **PlayerAnimationController** (Animation)

- Animation parameter management
- Movement state detection
- Speed-based animation blending
- Jump animation triggering

### **EnemyCollisionDetector** (Collision Handler)

- Detects player capture
- Triggers game over
- Notifies GameManager

### **CoinTriggerZone** (Bonus Spawning)

- Triggers bonus coin spawns
- Zone-based activation

### **CoinGlow** (Visual Effect)

- Coin glowing animation
- Pulsing intensity effect
