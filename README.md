# 8Blobs
### This does not currently function, it is a work in progress.
A simple Survivors Style Game Template made with Godot Engine 4.5.
This project is made to be a starting point for anyone wanting to make a game similar to the "Survivors" genre of indie games. It includes basic movement, shooting, enemy AI, and a simple UI. The project is structured to be easily expandable and modifiable. The project is open source and free to use for any purpose. It uses a sort of ECS (Entity Component System) style architecture, with a focus on modularity and separation of concerns.
## Architecture
This project uses a service-oriented architecture, with a focus on modularity and separation of concerns. The main services are delivered via a CoreProvider singleton, which allows for easy access to services throughout the project. The services handle low-level operations, while game logic is managed by higher-level systems like GameManager, MapSystem, MobSystem, and PlayerSystem. The main scene consists of a main node with a handful of child manager nodes.
### Main Node Structure
- **Main**: The root node that initializes and manages the game state.
  - **GameManager**: Manages overall game logic and state transitions. This instantiates and holds references to the main systems.
  - **HudManager**: Manages the heads-up display (HUD) and UI elements.
  - **AudioManager**: Handles audio playback and sound effects.
### Main Services
- **AudioService**: Manages audio playback and sound effects.
- **ClockService**: Manages game timing, including pulse ticks for game updates.
- **DataService**: Handles data persistence and retrieval as it concerns progression.
- **InputService**: Handles player input and controls.
- **SaveService**: Manages saving and loading game data.
- **UIService**: Manages UI elements and interactions.
### Main Systems
- **ChestSystem**: Manages chest interactions and loot generation.
- **MapSystem**: Manages map generation, stitching, and interactions.
- **MobSystem**: Manages enemy mobs, their spawning, and behaviors.
- **PlayerSystem**: Manages player state, health, and interactions.
These are not all or final, more services and systems may be added as the project evolves.
## Resources and Entities
The project uses a resource and entity system to manage game objects and their properties. Resources are defined in the `entities` folder, while scenes for various game objects are located in the `assets/scenes` folder. This allows for easy modification and extension of game objects without altering core logic. Objects in the game, like mobs and items, are defined as scenes build of both an entity script, nodes and resources. The idea behind this is to maximize reusability and modularity; while allowing for designer to easily create and modify game objects without coding.
## Getting Started
Don't. Not yet.
# Resource Assets are not included
This repo does not contain the resource assets used in this project, they must be self provided. This project used assets from [CanariPack 8Bit Topdown](https://canarigames.itch.io/canaripack-8bit-topdown) by Canari Games.
# License
Licensed under AGPL-3.0-only (see LICENSE for details).