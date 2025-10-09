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

## Need to do right now
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