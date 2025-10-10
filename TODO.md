# TODO
## Core Concept
The entire concept behind this project is to create a simple and highly performant 2D survivors template for the Godot Engine 4.x. The hope is to provide a solid foundation for developers to build upon and get to creating their own unique games quickly and efficiently.
## Main Feature Goals
- Player controller
- Basic mobs
- Basic items
- basic weapons
- Basic UI/Hud
- Basic treasure chest "slot machine"
- Basic menu system
- Complex performant services system
- Batched processing system for AI
- Stub saving system
- Stub achievements/ Unlocks system
- Performant map tiling system
- Performant other level systems, like audio, loading levels, etc.
- Example Level
- Documentation

### Key metric of success
- Able to run 1000+ mobs on screen at once at a solid 60fps on mid range hardware
    - The goal is this is able to run on the Steam Deck or even lower end (I.E. iGPU) hardware
- Able to run 100+ items on screen at once at a solid 60fps on mid range hardware
- Able to run 50+ weapon effects on screen at once at a solid 60fps on mid range hardware
- User is able to download the template and have a running game in under an hour.

### Need to do right now
- Rename core services to simple "Services"
- Rename Tools to simple "Systems"
- Fix issue with how pickup is named
- Create proper Level.cs
    - The level systems should not interact with the core services directly, but have relevant data passed through
    - Things like the pulse events should be ran by level.cs and it can call its child node updates.
- Replace Tool (now systems) instatiated nodes with proper node references from the loaded screne tree level
- Drop Toolbox for links to nodes inside of level.cs
- Batch mob AI processing and movement to instead use a MobSystem that processes all mobs in a single process call
    - Mobs should be data containers (clients) controlled by the MobSystem (server)
- Replace mob sprite animations with static sprites that use programmic or shader animation instead to further improve performance
- Finish up other systems
- Create proper documentation
- Create example level

## Architecture
This section is for the proposed refactoring of Architecture of the project to improve performance and maintainability.
Entire project should be refactored for a more data driven approach, as this would align better with the concept of the project; which is to present a surviors style game template. These types of games have a lot of nuanced data and would benefit from a focused approach. That is to say the game's architecture would consist of three levels of systems:
- Core "Services"
    The main controllers of the game; this would handle core functions.
- Game "Systems"
    These would be responsible for the game's logic and rules; but specifically during gameplay. Menus and other non-gameplay systems would be handled by core services.
    These would be the "servers" to the node entities "clients". They exist within the level scene tree and are instantiated when the level is loaded.
- Node "Entities"
    At the heart of the game would be custom nodes that exist to contain data; these are built in the godot editor as scenes or, primarily, *resources*. These would be very lightweight and have little to no logic. They would be controlled by the game systems.
    Examples of these would be mobs, items, weapons, heros, etc.

### Script Naming Conventions
- Core Services
    Used to manage core game functionality, such as audio, saving, loading, etc.
    - Namespace: Core
    - Folder: core/
    - Class Prefix: none
    - File Prefix: none
    - Class Suffix: Service
    - File Suffix: Service
    - Example: Core/ClockService.cs
- Game Systems
    Used to manage game logic and rules during gameplay.
    - Namespace: Game
    - Folder: game/
    - Class Prefix: none
    - File Prefix: none
    - Class Suffix: System
    - File Suffix: System
    - Example: Game/MovementSystem.cs
- Node Entities
    Used to represent in-game objects, such as mobs, items, weapons, etc. for use in data creation inside of Godot.
    - Namespace: Entities
    - Folder: entities/<type>/
    - Class Prefix: none
    - File Prefix: none
    - Class Suffix: none
    - File Suffix: none
    - Example: Entities/Mob/Mob.cs
-Interfaces
    Used to define contracts for services, systems, and entities.
    - Namespace: Core/Interfaces, Game/Interfaces, or Entities/Interfaces
    - Folder: core/interfaces/, game/interfaces/, entities/interfaces/
    - Class Prefix: I
    - File Prefix: I
    - Class Suffix: none (or Service/System if disambiguating)
    - File Suffix: none (or Service/System if disambiguating)
    - Example: Core/Interfaces/ISaveService.cs, Game/Interfaces/IMobSystem.cs
- Enums
    Used to define types, states, and other constant values. Enums of a type are grouped together. IE. all mob related enums are in one file.
    non-specific enums, like those of rarity or elements, are placed in a more general location (Entities/Enums or Game/Enums).
    - Namespace: Entities/Enums or Game/Enums or Core/Enums
    - Folder: next to relevant scripts or in a dedicated enums/ folder
    - Class Prefix: none
    - File Prefix: none
    - Class Suffix: Enums
    - File Suffix: Enums
    - Example: Entities/Enums/MobEnums.cs or Game/Enums/GameState.cs or Core/Enums/SaveSlot.cs
- Indexes
    Used to map enums to data resources or as a storage for possible unlocks.
    - Namespace: Entities/Indexes
    - Folder: entities/indexes/
    - Class Prefix: none
    - File Prefix: none
    - Class Suffix: Index
    - File Suffix: Index
    - Example: Entities/Indexes/UnlockIndex.cs, Entities/Indexes/MobIndex.cs (which maps all mob enums to their data resources)
### Data Resource Naming Conventions
Created tres files in Godot that represent game data.
- All data resources should be placed in assets/data/<type>/
- Naming convention: <Type><Clarity>Data.tres
    - Example: MobBlobData.tres, ItemHealthPotionData.tres, WeaponSwordData.tres
- Indexes are placed in assets/data/indexes/
    - Naming convention: DataIndex.tres
    - These are singular files that map enums to data resources.
    - These also contain the boolean flags for if the data is unlocked or not.

### Folder Structure
```
res://
|-- assets/
|   |-- audio/
|       |-- music/
|       |-- sfx/
|   |-- data/
|       |-- entities/
|           |-- heros/
|           |-- items/
|           |-- mobs/
|           |-- weapons/
|           |-- indexes/
|               |-- DataIndex.tres
|   |-- scenes/
|       |-- main.tscn
|       |-- levels/
|       |-- ui/
|   |-- fonts/
|   |-- shaders/
|   |-- sprites/
|       |-- icons/
|       |-- heros/
|       |-- items/
|       |-- mobs/
|       |-- weapons/
|   |-- tilesets/
|       |-- <theme>/
|-- docs/
|   |-- architecture.md
|   |-- gameplay.md
|   |-- systems.md
|   |-- entities.md
|-- scripts/
|   |-- main.cs
|   |-- core/
|       |-- interfaces/
|       |-- services/
|   |-- game/
|       |-- interfaces/
|       |-- systems/
|       |-- enums/
|   |-- entities/
|       |-- enums/
|       |-- indexes/
|       |-- hero/
|       |-- item/
|       |-- level/
|       |-- mob/
|           |-- boss.cs
|           |-- mob.cs
|           |-- mobTypeData.cs
|           |-- mobEnums.cs
|       |-- weapon/
```

# Performance Notes
These are notes on how to acheive the concept and ideas above using Godot's systems more effectively, rather than doing everything in scripts.
This makes it eaiser for designers to create content and tune values without needing to recompile code.
## This is all WIP and subject to change.
## Core Concept: Separation of Data, Behavior, and State

This pattern implements a **server-client** architecture where:
- **MobEntity nodes** = lightweight data containers (clients)
- **MobSystem** = centralized processor (server) that operates on all mob entities each frame
- **MobData Resources** = immutable configuration templates that define behavior parameters per mob type

This eliminates per-mob `_Process()` overhead and branch-heavy logic scattered across hundreds of node instances.

---

## Part 1: `MobData : Resource` — Configuration Templates

### What is it?
A `Resource` is Godot's serializable data container. Unlike nodes, Resources are pure data objects with no place in the scene tree. They can be created as `.tres` files in the editor and loaded at runtime.

### Structure
```csharp
// File: scripts/entities/mob/MobData.cs
using Godot;

[GlobalClass]
public partial class MobData : Resource
{
    [Export] public float Speed { get; set; } = 100f;
    [Export] public int Health { get; set; } = 10;
    [Export] public int Damage { get; set; } = 1;
    [Export] public float CollisionRadius { get; set; } = 16f;
    [Export] public Texture2D Sprite { get; set; }
    [Export] public MobAIPreset AIPreset { get; set; } = MobAIPreset.Chase;
    // Optional: animation timing, particle effects, sound refs, etc.
}

public enum MobAIPreset
{
    Idle,
    Chase,
    Patrol,
    Swarm,
    Ranged
}
```
^^ We should layer these with base stats, tribe stats, and type stats

### Why use this?
- **Designer-friendly**: Create `BlobMobData.tres`, `SnakeMobData.tres` in Godot editor with inspector values.
- **No code changes for tuning**: Adjust speed/health without recompiling.
- **Shared templates**: Multiple mob instances of the same type reference the same `MobData` in memory (efficient).
- **Version control**: `.tres` files are text-based and diff-friendly.

### Creating the files
In Godot editor:
1. Right-click in FileSystem → New Resource → select `MobData`
2. Set properties in inspector (speed = 120, damage = 2, etc.)
3. Save as `res://assets/data/mobs/BlobMobData.tres`

Repeat for each distinct mob type (`SnakeMobData.tres`, `TrapMobData.tres`, etc.).

---

## Part 2: DataIndex.tres — Central Registry

### What is it?
A single Resource that maps enum values to their corresponding `MobData` Resources. This acts as a lookup table loaded once at game start.

### Structure
```csharp
// File: scripts/core/DataIndex.cs
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DataIndex : Resource
{
    [Export] public Dictionary<MobTypeEnum, MobData> MobDataMap { get; set; } = new();
    // Could also include ItemData, WeaponData, etc.
}
```

### Creating the registry
In Godot editor:
1. Create a new `DataIndex` Resource → save as `res://DataIndex.tres`
2. In inspector, populate the `MobDataMap`:
   - Key: `MobTypeEnum.Blob` → Value: `BlobMobData.tres`
   - Key: `MobTypeEnum.Snake` → Value: `SnakeMobData.tres`
   - Key: `MobTypeEnum.Trap` → Value: `TrapMobData.tres`

### Why centralize?
- **Single source of truth**: All systems load the same index.
- **Hot reload**: Change mappings in editor without code changes.
- **Validation**: Easy to verify all enum values have data at load time.

---

## Part 3: `MobSystem` — Centralized Processor

### What is it?
A single node (typically child of `Level.cs`) that handles all mob logic in one `_Process()` or `_PhysicsProcess()` call. Instead of 1000 mobs each running their own process, you have **one** system processing 1000 data-only entities.

### High-level structure
```csharp
// File: scripts/game/systems/MobSystem.cs
using Godot;
using System.Collections.Generic;

public partial class MobSystem : Node
{
    private Dictionary<MobTypeEnum, MobData> _mobDataLookup;
    private Dictionary<MobTypeEnum, System.Action<MobEntity, MobData, float>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();

    public override void _Ready()
    {
        LoadDataIndex();
        RegisterAIHandlers();
        CollectMobEntities();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        
        // Single loop processes ALL mobs
        for (int i = _activeMobs.Count - 1; i >= 0; i--)
        {
            MobEntity mob = _activeMobs[i];
            if (!IsInstanceValid(mob)) {
                _activeMobs.RemoveAt(i);
                continue;
            }

            MobData data = _mobDataLookup[mob.MobType];
            
            // Delegate lookup: zero branching per mob
            _aiHandlers[data.AIPreset](mob, data, dt);
            
            // Common physics update
            mob.Position += mob.Velocity * dt;
        }
    }
}
```

### Step-by-step breakdown

#### A. `LoadDataIndex()` — Build the lookup dictionary
```csharp
private void LoadDataIndex()
{
    DataIndex index = GD.Load<DataIndex>("res://DataIndex.tres");
    _mobDataLookup = new Dictionary<MobTypeEnum, MobData>(index.MobDataMap);
    
    // Validate: ensure all enum values have data
    foreach (MobTypeEnum type in System.Enum.GetValues<MobTypeEnum>())
    {
        if (!_mobDataLookup.ContainsKey(type))
            GD.PushError($"Missing MobData for {type}");
    }
}
```

**Why?** Convert the Godot `Dictionary` to a C# `Dictionary` for fast O(1) lookups. Validation catches missing configurations early.

#### B. `RegisterAIHandlers()` — Pre-register strategy delegates
```csharp
private void RegisterAIHandlers()
{
    _aiHandlers = new()
    {
        { MobAIPreset.Idle, HandleIdleAI },
        { MobAIPreset.Chase, HandleChaseAI },
        { MobAIPreset.Patrol, HandlePatrolAI },
        { MobAIPreset.Swarm, HandleSwarmAI },
        { MobAIPreset.Ranged, HandleRangedAI }
    };
}

private void HandleChaseAI(MobEntity mob, MobData data, float delta)
{
    // Example: move toward player
    if (_player == null) return;
    
    Vector2 direction = (_player.GlobalPosition - mob.GlobalPosition).Normalized();
    mob.Velocity = direction * data.Speed;
}

private void HandleIdleAI(MobEntity mob, MobData data, float delta)
{
    mob.Velocity = Vector2.Zero;
}

// ... implement other AI behaviors
```

**Why delegates?** A delegate is a type-safe function pointer. Storing them in a dictionary replaces `switch` statements:
- **No branching per mob**: `_aiHandlers[preset](mob, data, delta)` is a single dictionary lookup + function call.
- **Extensible**: Add new AI presets without modifying the main loop.
- **Cache-friendly**: The main loop is tight; CPU can predict branches easily.

#### C. `CollectMobEntities()` — Gather all mob nodes
```csharp
private void CollectMobEntities()
{
    // Assuming mobs are children of a "Mobs" node under Level
    Node mobsContainer = GetNode("/root/Level/Mobs");
    
    foreach (Node child in mobsContainer.GetChildren())
    {
        if (child is MobEntity mob)
            _activeMobs.Add(mob);
    }
}
```

**Alternative**: Use Godot groups. Tag mobs with `AddToGroup("mobs")` in editor, then:
```csharp
_activeMobs = GetTree().GetNodesInGroup("mobs").Cast<MobEntity>().ToList();
```

**Why a list?** Sequential iteration is cache-friendly. Reverse iteration (`i--`) safely removes dead mobs during the loop.

---

## Part 4: `MobEntity` — Lightweight Data Container

### Structure
```csharp
// File: scripts/entities/mob/MobEntity.cs
using Godot;

public partial class MobEntity : CharacterBody2D // or RigidBody2D
{
    [Export] public MobTypeEnum MobType { get; set; } = MobTypeEnum.Blob;
    
    // Runtime state (managed by MobSystem)
    public Vector2 Velocity { get; set; }
    public int CurrentHealth { get; set; }
    public float StateTimer { get; set; } // for AI state machines
    
    // No _Process() or _PhysicsProcess() — system controls everything
}
```

### Key points
- **No logic methods**: No `_Process()`, no `_PhysicsProcess()`, no AI code.
- **Exports for designer**: `MobType` is set in Godot scene inspector (e.g., `BlobMob.tscn` sets `MobType = Blob`).
- **State fields only**: `CurrentHealth`, `Velocity`, `StateTimer` are updated by `MobSystem`.

### Scene setup
In Godot editor:
1. Create scene `BlobMob.tscn` with root `MobEntity` node.
2. Set `MobType = Blob` in inspector.
3. Add `Sprite2D`, `CollisionShape2D` children (visuals/physics).
4. Instantiate this scene 1000 times in level — each references `BlobMobData.tres` via the `MobType` enum.

---

## Part 5: How It All Connects

### At level load:
1. `Level.cs` instantiates `MobSystem` as a child node.
2. `MobSystem._Ready()` runs:
   - Loads DataIndex.tres → builds `_mobDataLookup` dictionary.
   - Registers AI handler delegates → builds `_aiHandlers` dictionary.
   - Collects all `MobEntity` nodes in scene → populates `_activeMobs` list.

### Every physics frame:
1. `MobSystem._PhysicsProcess(delta)` iterates `_activeMobs` list.
2. For each mob:
   - Lookup `MobData` via `mob.MobType` enum (O(1) dictionary access).
   - Lookup AI handler via `data.AIPreset` enum (O(1) dictionary access).
   - Invoke handler: `_aiHandlers[preset](mob, data, delta)`.
   - Handler updates `mob.Velocity` based on AI logic + `data.Speed`.
   - System applies movement: `mob.Position += mob.Velocity * delta`.

### Diagram
```
Level.tscn
├── Player
├── Mobs (Node container)
│   ├── BlobMob instance #1 (MobEntity, MobType=Blob)
│   ├── BlobMob instance #2 (MobEntity, MobType=Blob)
│   ├── SnakeMob instance #1 (MobEntity, MobType=Snake)
│   └── ... 997 more
└── MobSystem (processes all above in _PhysicsProcess)

DataIndex.tres
├── MobType.Blob → BlobMobData.tres (speed=100, AI=Chase)
└── MobType.Snake → SnakeMobData.tres (speed=150, AI=Swarm)
```

## Extending the Pattern

### Spatial partitioning
```csharp
// Divide level into grid cells; only process mobs near player
Dictionary<Vector2I, List<MobEntity>> _spatialGrid;

// Each frame: update only cells within player's view radius
```

### Event-driven AI
Instead of updating all mobs every frame:
```csharp
// Mobs in "Idle" preset only update every 0.5 seconds
if (mob.StateTimer < 0.5f) continue;
mob.StateTimer = 0;
```

### Data-driven attacks (for bosses)
Add `AttackData : Resource` to `MobData`:
```csharp
[Export] public AttackData Attack { get; set; }

// In MobSystem
if (ShouldAttack(mob)) {
    SpawnProjectile(mob.Position, data.Attack);
}
```

---

### Pitfall: Entity lifecycle management
Mobs spawning/dying mid-frame need safe handling:
```csharp
// Use a separate "spawn queue" and "death queue"
private Queue<MobEntity> _spawnQueue = new();
private Queue<MobEntity> _deathQueue = new();

public void SpawnMob(MobEntity mob) => _spawnQueue.Enqueue(mob);
public void KillMob(MobEntity mob) => _deathQueue.Enqueue(mob);

// Process queues at end of frame
private void ProcessSpawnQueue() { /* add to _activeMobs */ }
private void ProcessDeathQueue() { /* remove + QueueFree() */ }
```

### Pitfall: Godot scene tree overhead
If even `CharacterBody2D` is too heavy, consider:
- Use `Node2D` with manual collision queries via `PhysicsDirectSpaceState2D`.
- Store mob data in plain C# structs; render with `MultiMeshInstance2D` (GPU instancing).

---

## Documentation References

### Godot Resources
- Official: [Godot Resources Documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/resources.html)
- Why: Explains `.tres` files, `[GlobalClass]` attribute, Resource loading.

### C# Delegates & Performance
- Microsoft: [Delegates (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/)
- Why: Explains delegate syntax, `Action<T>`, and delegate vs `switch` performance.

### Data-Oriented Design
- Richard Fabian: [Data-Oriented Design](https://www.dataorienteddesign.com/dodbook/)
- Why: Foundational text on separating data/behavior for performance; applies to game engines.

### Godot Performance Optimization
- Godot Docs: [Optimization using Servers](https://docs.godotengine.org/en/stable/tutorials/performance/using_servers.html)
- Why: Explains Godot's internal server architecture (RenderingServer, PhysicsServer); your pattern mimics this at gameplay level.