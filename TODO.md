- Coupling issues
    - MobSystem is accessing clock, it should access event.
    - Player and Mob use get parent, they need to not do that, instead have them inject
- Service initialization method is misspelled "Initialize" instead of "Initilize"?
    - Standardize as Init
- MobSystem needs a clean up for active mobs and such as well as the phantom mobs created for loading purposes
    - MobSystem should have a ClearMobs() method that is called on level unload
    - In CalculateSpawnWeight(), the line while(_mobSpawnPool.Find(x => x.mob == mob).weight == spawnWeight) will cause issues:
        If the item isn't found, Find() returns default which has weight 0, creating an infinite loop
        The logic adds 0.32f repeatedly to ensure "unique weights" but this isn't a reliable approach
    - Mob Pooling though instead of phantom loading; dont add to parent until needed. List? Queue?
- Need to figure out how to properly avoid node usage in services. Nodes are for Systems.
    - ILevelService exposes ParentNode property, mixing service abstraction with Godot scene tree management
    - IHeroService has the same issue with HeroNode
- Menu should use Godot Signals, but we need to make sure everything else is using the EventService
- Check for Lambda subs in the managers
- The IEntity children need to have a Guard Method to stop multiple calls to Inject


Consider breaking down MobEntity and a few others into smaller components to adhere to single responsibility principle.
Then use GetNodeInGroup to access those components from systems rather than having large monolithic entity classes collected in lists? This way we do not need to worry about removing them from the list and prevents stale references.