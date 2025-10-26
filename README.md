# First 3D Game - Enemy Chase Game

A simple yet engaging Unity game where the player must survive and evade multiple AI-controlled enemies in a terrain-based environment.

## 📋 Project Overview

This is a **survival/chase game** built with Unity where:
- The player navigates a 3D terrain environment
- Multiple AI enemies patrol and chase the player
- The objective is to evade enemies for as long as possible
- Survival time is tracked and displayed on screen

## 🎮 Game Features

- **Player Movement**: WASD controls with mouse look, running, and jumping
- **Enemy AI**: Multiple enemies that patrol and chase the player
- **NavMesh Navigation**: Enemies navigate the terrain using Unity's NavMesh system
- **Survival Timer**: Tracks how long the player has survived
- **Collision Detection**: Game ends when an enemy catches the player
- **UI Feedback**: Clear instructions and objective display

## 🏗️ Project Structure

```
Assets/
├── Scripts/
│   ├── PlayerMovementScript.cs          # Player movement and controls
│   ├── EnemyAi.cs                       # Enemy AI patrol and chase logic
│   ├── EnemyCollisionDetector.cs        # Enemy-player collision detection
│   ├── GameManager.cs                   # Game state and enemy spawning
│   └── ...
├── Scenes/
│   ├── SampleScene.unity                # Main game scene
│   └── ...
├── Prefabs/
│   ├── Enemy.prefab                     # Enemy prefab (capsule)
│   └── ...
└── Terrain/
    └── New Terrain.asset                # Terrain data

ProjectSettings/
└── ...
```

## 🛠️ Setup Instructions

### Prerequisites
- Unity 2020.3 LTS or newer
- AI Navigation package installed

### Step 1: Install AI Navigation Package
1. Go to **Window > TextureImporter > Package Manager**
2. Click **+** button in the top left
3. Search for **"AI Navigation"**
4. Click Install

### Step 2: Setup Terrain & NavMesh

1. Select **Terrain** in the Hierarchy
2. In the Inspector, find the **NavMeshSurface** component
3. Click the **"Bake"** button
4. Wait for the blue NavMesh to appear on the terrain

### Step 3: Tag Your Player

1. Select **Player** in the Hierarchy
2. In the Inspector, click the **Tag** dropdown (top-right)
3. Select **"Player"** (create if it doesn't exist)

### Step 4: Create Enemy Prefab

1. Select one **EnemyAi capsule** in the Hierarchy
2. Add **EnemyCollisionDetector** component:
   - Click **Add Component**
   - Search for **"EnemyCollisionDetector"**
   - Click to add it
3. Ensure the capsule's **Collider** is set to **"Is Trigger"**:
   - In Inspector, find **Capsule Collider**
   - Check the **"Is Trigger"** checkbox
4. Drag the capsule to **Assets/Prefabs** folder to create a prefab
5. Delete it from the Hierarchy

### Step 5: Create UI (Optional but Recommended)

Create three TextMeshPro text objects:

1. **SurvivalTimeText** (Top-right):
   - Right-click Hierarchy → **UI > Text - TextMeshPro**
   - Position: Top-right corner
   - Anchor: Top-right

2. **ObjectiveText** (Top-center):
   - Position: Center-top
   - Anchor: Top-center

3. **InstructionText** (Bottom-center):
   - Position: Center-bottom
   - Anchor: Bottom-center

### Step 6: Setup GameManager

1. Right-click in Hierarchy → **Create Empty**
2. Rename to **"GameManager"**
3. Add **GameManager** component:
   - Click **Add Component**
   - Search for **"GameManager"**
4. In the Inspector, configure:
   - **Enemy Prefab**: Drag your enemy prefab
   - **Player**: Drag your Player object
   - **Number Of Enemies**: 3-5 (adjust difficulty)
   - **Spawn Radius**: 50 (how far enemies spawn from player)
   - Assign your UI TextMeshPro objects:
     - **Survival Time Text**: Drag SurvivalTimeText
     - **Objective Text**: Drag ObjectiveText
     - **Instruction Text**: Drag InstructionText

### Step 7: Play!

Press **Play** and test the game. You should:
- ✅ See enemies spawn around you
- ✅ Be able to move with WASD
- ✅ See enemies chase you
- ✅ See survival timer counting up
- ✅ Game ends if caught

## 🎮 Controls

| Key | Action |
|-----|--------|
| **WASD** | Move forward/back/left/right |
| **Mouse** | Look around |
| **SHIFT** | Run (faster movement) |
| **SPACE** | Jump |
| **C** | Crouch |
| **ESC** | Unlock cursor (press again to lock) |

## 📝 Script Documentation

### PlayerMovementScript.cs
Handles player movement, camera look, jumping, running, and crouching.

**Key Variables:**
- `walkSpeed`: Normal movement speed (6 units/sec)
- `runSpeed`: Sprint speed (12 units/sec)
- `jumpPower`: Jump force (7 units)
- `lookSpeed`: Mouse sensitivity (2)

### EnemyAi.cs
Controls enemy behavior - patrolling and chasing.

**Key Features:**
- Patrols randomly when player is not detected
- Chases player when within sight range
- Uses NavMesh for pathfinding
- Automatically finds player by tag or name

**Key Variables:**
- `sightRange`: How far enemy can see (30 units)
- `walkPointRange`: Patrol area size (20 units)
- `chaseStopDistance`: Distance to keep from player (2 units)

### GameManager.cs
Manages game state, spawns enemies, and tracks survival time.

**Key Features:**
- Spawns multiple enemies at start
- Tracks survival time
- Updates UI with game information
- Detects when player is caught

**Key Variables:**
- `numberOfEnemies`: How many enemies to spawn (3-5)
- `spawnRadius`: Distance enemies spawn from player (50 units)
- `spawnHeight`: Height enemies spawn at (1 unit)

### EnemyCollisionDetector.cs
Detects collision between enemy and player.

**Key Features:**
- Uses trigger collision detection
- Calls GameManager when player is caught
- Prevents multiple catch triggers

## 🐛 Troubleshooting

### Enemies not moving
- **Solution**: Check if NavMesh is baked (blue mesh visible on terrain)
- **Solution**: Verify NavMeshAgent component is enabled
- **Solution**: Check if Agent Type matches baked NavMesh type

### Enemies not chasing
- **Solution**: Verify Player is tagged as "Player"
- **Solution**: Check sightRange value (should be > 0)
- **Solution**: Verify player is within sightRange distance

### Game doesn't recognize player being caught
- **Solution**: Ensure Player has correct tag
- **Solution**: Verify Collider is set to "Is Trigger"
- **Solution**: Check EnemyCollisionDetector is added to enemy

### No UI text showing
- **Solution**: Verify TextMeshPro objects are assigned in GameManager
- **Solution**: Check Canvas is in the scene
- **Solution**: Verify text objects have proper positioning

## 🚀 Tips for Implementation

1. **Difficulty Adjustment**:
   - Increase `numberOfEnemies` for harder game
   - Increase `sightRange` to make enemies detect earlier
   - Decrease `walkPointRange` to keep enemies closer

2. **Performance**:
   - Limit enemies to 10 or fewer for smooth performance
   - Use NavMeshAgent with lower quality settings if needed

3. **Testing**:
   - Play in windowed mode to see console errors
   - Use Debug.Log to track enemy spawning
   - Enable NavMesh visualization (Window > AI > Navigation)

4. **Future Enhancements**:
   - Add collectible items (coins, power-ups)
   - Add sound effects for chasing/catching
   - Add different enemy types with varied AI
   - Add pause menu and level selection
   - Add score multiplier based on difficulty

## 📦 Required Packages

- Unity 2020.3 LTS or newer
- AI Navigation package (installed via Package Manager)
- TextMeshPro (usually included with Unity)

## 📄 Code Quality

All scripts include:
- Clear comments and documentation
- Proper naming conventions
- Error handling and validation
- Debug logging for troubleshooting
- Organized code structure

## 🤝 Collaboration Notes

When working with your team:

1. **Always pull latest changes** before starting work
2. **Use feature branches** for new features
3. **Test changes** in Unity before committing
4. **Write clear commit messages**
5. **Document any new scripts** with similar format
6. **Follow existing code style** for consistency
7. **Use .gitignore** to avoid committing Unity temporary files

## 📧 Support

For issues or questions:
1. Check the **Troubleshooting** section
2. Review the **Script Documentation**
3. Check Console for error messages
4. Verify all setup steps were completed

## 📜 License

This project is for educational purposes.

---

**Happy Developing! 🎮**
