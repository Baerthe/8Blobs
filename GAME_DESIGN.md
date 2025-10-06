# 8Blobs - Game Design Document

## Overview

**8Blobs** is a Godot 4.5 2D game template for creating "Survivors" style games, inspired by titles like Vampire Survivors and Megabonk. This template provides a robust foundation for top-down, wave-based survival games where players face increasing hordes of enemies while collecting power-ups and upgrades.

**Engine:** Godot 4.5  
**Language:** C#  
**Genre:** Survivors-like, Top-Down Action, Wave-Based Survival  
**Version:** 0.0.2  
**Display:** 256x240 viewport (retro-style)  
**Framerate:** 60 FPS

---

## Game Concept

Players control a character in a top-down arena, automatically attacking enemies while manually controlling movement. Enemies spawn in waves around the player, and the goal is to survive as long as possible while defeating enemies, collecting items, and building up character power through weapon pickups and upgrades.

### Core Gameplay Loop

1. **Move to Survive**: Player navigates using WASD/Arrow keys to avoid enemy contact
2. **Automatic Combat**: Weapons fire automatically at nearby enemies
3. **Collect Upgrades**: Pickup weapons and items that spawn periodically
4. **Score Points**: Defeated enemies grant points
5. **Endless Waves**: Enemies spawn continuously until player death

---

## Player System

### Player Character (`Player.cs`)

The player is a `CharacterBody2D` with the following core attributes:

- **Health System**
  - Maximum Health: 4 (configurable)
  - Takes damage from enemy collisions
  - Emits `OnDeath` signal when health reaches 0

- **Movement**
  - Speed: 400 pixels/second (configurable)
  - 8-directional movement (Up, Down, Left, Right, Diagonal)
  - Smooth keyboard controls (WASD/Arrow Keys)

- **Equipment**
  - Weapon inventory (List of Weapon objects)
  - Items can be collected via pickups

- **Visual Feedback**
  - AnimatedSprite2D with multiple animations
  - Direction-based animation states
  - Death animation on game over

---

## Enemy System

### Mob Base Class (`Mob.cs`)

All enemies inherit from the abstract `Mob` class, which extends `RigidBody2D`.

#### Statistics & Classification

- **Level**: Basic, Normal, Elite, Boss, etc. (`MobLevel` enum)
- **Element**: None, Fire, Water, Earth, Air, Light, Dark, Electric, Ice
- **Tribe**: None, Beast, Undead, Elemental, Humanoid, Goblinoid, Insectoid
- **Rarity**: Basic → One (14 tiers from common to unique)

#### Core Attributes

- **Health**: Configurable (default: 1)
- **Speed**: Movement speed value
- **Damage**: Collision damage to player
- **Movement Type**: Determines pathfinding behavior
  - Straight line to player
  - Curved/indirect approach
  - Other patterns (extensible)

#### Abilities

Mobs can have special abilities with configurable strength:
- **Runner**: Standard chase behavior
- Additional abilities can be added via the `MobAbility` enum

#### Lifecycle

- Spawned via `PathFollow2D` spawners around the screen edges
- Moves toward player based on movement type
- Takes damage from weapon collisions
- Dies when health reaches 0
- Removed from scene tree on death

### Implemented Enemy Types

1. **Blob** - Basic enemy mob
2. **Snake** - Variant enemy mob
3. **NegaBlob** - Advanced variant
4. **Trap** - Stationary or special behavior enemy

---

## Weapon & Combat System

### Weapon Base Class (`Weapon.cs`)

All weapons extend the abstract `Weapon` class, which itself extends `Pickup`.

#### Core Weapon Properties

- **Damage**: Amount of damage dealt per hit
- **Attack Speed**: Attacks per second (fire rate)
- **Range**: Detection/attack radius in pixels
- **Projectile Sprite**: Visual representation of attacks
- **Attack Sound**: Audio feedback for attacks

#### Weapon Behavior

- Weapons are picked up from spawned pickups
- Player can carry multiple weapons simultaneously
- Each weapon calls its `Attack()` method automatically
- Weapons are cleared on player death

### Pickup System (`Pickup.cs`)

- **Weapon Pickups**: Grant new weapons to the player
- **Item Pickups**: Provide temporary or permanent benefits
- **Spawn System**: Controlled by `PickupSpawnTimer` (default: every 10 seconds)
- Pickups spawn at configured spawn points around the map

#### Implemented Items

- **Potion**: Health restoration or stat boost
- **Poison**: Possible damage-over-time effect

---

## Core Game Systems

### Main Orchestrator (`Main.cs`)

The `Main` class serves as the central orchestration point for the entire game.

#### Responsibilities

- Dependency injection via `Services` singleton
- Scene graph management (Player, UI, Camera, Menu)
- Spawner configuration (mobs and pickups)
- Game state coordination
- Lifecycle management (initialization, reset, cleanup)

#### Spawning System

**Mob Spawning**
- Path-based spawning using `Path2D` and `PathFollow2D`
- Configurable spawn paths around the play area
- Multiple mob scene variants can be registered
- Spawn timing controlled by `ClockManager`

**Pickup Spawning**
- Similar path-based system
- Independent spawn timers
- Multiple pickup types supported

---

## Architecture

### Dependency Injection System

The game uses a custom dependency injection system to manage singletons and services.

#### Services (`Services.cs`)

Central service provider that builds and manages two containers:

**Core Container** (`CoreContainer.cs`)
- Singleton services (non-Node objects)
- Core game logic and managers
- No dependencies on tools
- Examples: `ClockManager`, `LevelManager`

**Tool Container** (`ToolContainer.cs`)
- Node-based services
- Can be instantiated and added to scene tree
- Can depend on core services
- Examples: UI tools, level tools, tiling systems

#### Registered Services

- **IClockManager** → `ClockManager`: Game timing and events
- **ILevelManager** → `LevelManager`: Level loading and management

---

## Clock & Timer System

### ClockManager (`ClockManager.cs`)

Centralized timer management system providing event-driven game timing.

#### Timer Types

1. **Pulse Timer** (0.05s / 20Hz)
   - High-frequency updates
   - ~1200 pulses per minute

2. **Slow Pulse Timer** (0.2s / 5Hz)
   - Medium-frequency updates
   - ~300 pulses per minute

3. **Mob Spawn Timer** (5s / 0.2Hz)
   - Controls enemy wave spawning
   - ~12 spawns per minute

4. **Pickup Spawn Timer** (10s / 0.1Hz)
   - Controls item drop rate
   - ~6 spawns per minute

5. **Game Timer** (60s / 0.016Hz)
   - Long-duration events
   - Level progression or difficulty scaling

6. **Starting Timer** (3s, One-shot)
   - Countdown before gameplay begins

#### Timer Events

All timers emit events that other systems can subscribe to:
- `PulseTimeout`
- `SlowPulseTimeout`
- `MobSpawnTimeout`
- `PickupSpawnTimeout`
- `GameTimeout`
- `StartingTimeout`

#### Features

- Pause/Resume all timers
- Configure timer durations dynamically
- Automatic timer lifecycle management

---

## UI System

### User Interface (`Ui.cs`)

The `Ui` class manages all on-screen display elements.

#### UI Elements

- **Score Display**: 8-digit formatted score counter
- **Health Display**: 2-digit health indicator
- **Message System**: Center-screen notifications
  - "Get Ready!" countdown
  - Game Over screen with final score
  - Timed messages with auto-hide

#### UI States

- **In-Game**: Shows health and score
- **Game Over**: Hides stats, shows game over message and restart prompt
- **Pre-Game**: Shows countdown timer

---

## Menu System

### Menu (`Menu.cs`)

Handles the main menu and game flow transitions.

#### Features

- Start new game
- Display instructions
- Credits/Settings (extensible)

---

## Level Management

### Level System

- **LevelManager**: Manages level loading, transitions, and state
- **LevelTool**: Node-based level utilities
- Level-specific configurations stored in level scenes
- Extendable for multiple levels/stages

---

## State Management

### IStateManager Interface

Provides access to current game state:
- Current player reference
- Current UI reference
- Score tracking
- Mob registry (add/remove/clear)

---

## Input System

### Configured Actions

- **move_up**: W / Up Arrow
- **move_down**: S / Down Arrow
- **move_left**: A / Left Arrow
- **move_right**: D / Right Arrow

All movement inputs have a 0.5 deadzone for gamepad support.

---

## Physics Layers

The game uses Godot's physics layers for collision detection:

1. **Layer 1 - Collision**: Environmental obstacles
2. **Layer 2 - Hitbox**: Character collision boxes
3. **Layer 3 - Weapon**: Weapon projectile collisions
4. **Layer 4 - Pickups**: Item pickup detection

---

## Progression & Difficulty

### Planned Progression Systems

The template is designed to support:

- **Wave-based difficulty scaling**
  - Increase enemy spawn rate over time
  - Spawn stronger enemy types
  - Mix multiple enemy types

- **Score System**
  - Points awarded for enemy defeats
  - High score tracking (via `PrefManager`)

- **Upgrade System** (extensible)
  - Weapon improvements
  - Stat enhancements
  - Special abilities

---

## Code Organization

### Folder Structure

```
script/
├── Main.cs                    # Central orchestrator
├── container/                 # Dependency injection containers
│   ├── CoreContainer.cs
│   └── ToolContainer.cs
├── core/                      # Core singleton services
│   ├── ClockManager.cs
│   ├── LevelManager.cs
│   ├── PrefManager.cs
│   ├── Services.cs
│   ├── enum/                  # Game-wide enumerations
│   └── interface/             # Core service interfaces
├── node/                      # Node-based game objects
│   ├── Player.cs
│   ├── mobs/                  # Enemy implementations
│   │   ├── base/Mob.cs
│   │   ├── Blob.cs
│   │   ├── Trap.cs
│   │   └── enum/              # Mob-specific enums
│   └── pickups/               # Collectible items
│       ├── weapon/            # Weapon system
│       └── items/             # Consumables
└── tool/                      # Node-based tool services
    ├── Ui.cs
    ├── Menu.cs
    ├── LevelTool.cs
    ├── TilingTool.cs
    └── interface/             # Tool interfaces
```

### Design Principles

1. **Separation of Concerns**
   - Core: Pure logic, no scene tree dependencies
   - Node: Godot scene tree components
   - Tool: Utility nodes that bridge core and scene

2. **Dependency Direction**
   - Tools can depend on Core
   - Core NEVER depends on Tools
   - Prevents circular dependencies

3. **Interface-Driven Design**
   - All services defined by interfaces
   - Enables testing and modularity
   - Supports dependency injection

4. **Scene-First Philosophy**
   - Enemies and weapons designed as Godot scenes
   - Editor-friendly configuration
   - Script provides base behavior
   - Scene provides specific implementation

---

## Technical Specifications

### Rendering

- **Renderer**: OpenGL Compatibility Mode
- **Stretch Mode**: Canvas Items
- **Viewport**: 256x240 (retro aesthetic)
- **Mobile Support**: GL Compatibility mode enabled

### Project Configuration

- **Assembly Name**: 8Blobs
- **Godot Features**: 4.5, C#, GL Compatibility
- **Max FPS**: 60

---

## Extension Points

This template is designed to be extended in the following areas:

### Enemy System

- Create new mob types by extending `Mob` class
- Implement custom movement patterns
- Add new abilities via `MobAbility` enum
- Configure stats in Godot editor

### Weapon System

- Create new weapons by extending `Weapon` class
- Implement custom `Attack()` logic
- Define projectile behavior
- Configure stats in scene properties

### Upgrade System

- Add progression mechanics
- Implement leveling system
- Create skill trees
- Add meta-progression

### Level Design

- Create multiple level scenes
- Design custom spawn patterns
- Add environmental hazards
- Implement boss encounters

---

## Asset Requirements

> **Note**: This template does NOT include art assets.

### Required Asset Types

- **Player Sprites**: Animated walk cycles, death animation
- **Enemy Sprites**: Walk animations for each mob type
- **Weapon Effects**: Projectile sprites, impact effects
- **UI Elements**: Health icons, score display, menu graphics
- **Audio**: Attack sounds, hit sounds, background music

### Recommended Asset Pack

The example implementation used [CanariPack 8Bit Topdown](https://canarigames.itch.io/canaripack-8bit-topdown) by Canari Games.

---

## Getting Started

### For Developers

1. **Clone the Repository**
   ```bash
   git clone https://github.com/Baerthe/8Blobs.git
   ```

2. **Open in Godot 4.5**
   - Import project in Godot Engine 4.5
   - Install .NET SDK if not already installed
   - Build C# solution

3. **Add Assets**
   - Provide sprite sheets for player and enemies
   - Add audio files
   - Configure AnimatedSprite2D nodes with your animations

4. **Configure Spawners**
   - Set `MobScenes` array in Main scene
   - Set `PickupScenes` array in Main scene
   - Adjust spawn paths as needed

5. **Customize Gameplay**
   - Modify timer intervals in `ClockManager`
   - Adjust player health and speed
   - Configure enemy stats
   - Add new weapons and items

### For Game Designers

- **Enemy Variants**: Create new mob scenes, inherit from existing mobs
- **Balance Tuning**: Adjust exported properties in scene inspector
- **Wave Design**: Modify spawn timer intervals
- **Progression**: Extend `LevelManager` for difficulty curves

---

## Future Development

Potential features to add to this template:

- [ ] Experience/Leveling system
- [ ] Upgrade selection system (pick 1 of 3 upgrades)
- [ ] Multiple playable characters
- [ ] Boss encounters
- [ ] Environmental hazards
- [ ] Save/Load progression
- [ ] Achievements system
- [ ] Multiple stages/biomes
- [ ] Particle effects system
- [ ] Sound manager
- [ ] Settings/Options menu
- [ ] Gamepad support refinement

---

## References & Inspiration

### Similar Games

- **Vampire Survivors**: The pioneer of modern survivors-like games
- **Megabonk**: Refined survivors-like mechanics
- **Brotato**: Wave-based survival with upgrade choices
- **20 Minutes Till Dawn**: Shooter-focused survivors-like

### Godot Resources

- [Godot 4.5 Documentation](https://docs.godotengine.org/en/stable/)
- [C# in Godot Guide](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/)
- [2D Movement Tutorial](https://docs.godotengine.org/en/stable/tutorials/2d/)

---

## License

This template is designed to be a starting point for your own games. Modify and extend as needed for your projects.

---

## Credits

**Template Created By**: Baerthe  
**Engine**: Godot Engine 4.5  
**Example Assets**: CanariPack 8Bit Topdown by Canari Games

---

*This document describes version 0.0.2 of the 8Blobs template. The template is under active development and subject to change.*
