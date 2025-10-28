- Add some sort of factory subsystem to the GameSystems?
- Refactor entities to no longer use Inject but constructors since we should be able to get the factory to force it.
- Make sure GameSystems actually use DI where possible instead of locator.
- Revert mob groups to instead use a list of actives that is manually handled.
- Establish GameManager update loop and order of updates finally.
- For safety, drop string events in favor of hard coding strict types. Strings are error prone and could face issues with performance.
    - Add a common events file to hold empty event types.
- For the gamesystems that load resources, we should make sure we are doing this during a loading process and cache them, not during gameplay
- Though setup right now for testing, we need to revise the main state processing into an actual state machine with proper transitions
- Add in DEBUG wrappers to clean up debug prints

x - Make a note that IEvents must never be structs to avoid boxing
- Consider making Attributes a class instead of struct to avoid boxing when passing around
- Make a note to *never* use lambdas for event subscriptions as they cause allocations due to closures and istead DO NOT sub/unsub/publish inside of an event or update loop
- Consider adding a pause check to GameManager's _Process to avoid updating systems when paused