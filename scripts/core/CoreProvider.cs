namespace Core;
using Godot;
using Core.Interface;
/// <summary>
/// Where the magic happens; builds our dependency injection containers for Core Services Injection. CoreProvider is a global static class that allows any other class to access core services via a simple container.
/// In usage, call CoreProvider.[ServiceName]Service() to get the singleton instance of the service; it can be helpful to register the service instance to a local readonly variable for easier access.
/// Example: private readonly IAudioService _audioService = CoreProvider.AudioService();
/// </summary>
public static class CoreProvider
{
    internal static CoreContainer CoreContainer { get; private set; } = new();
    private static bool _isBuilt = false;
    static CoreProvider()
    {
        InitilizationCheck();
    }
    public static IEventService EventService() => CoreContainer.Resolve<IEventService>(); // This must be first to ensure event subscriptions work
    public static IClockService ClockService() => CoreContainer.Resolve<IClockService>(); // This must be second to ensure timers work
    public static IAudioService AudioService() => CoreContainer.Resolve<IAudioService>();
    public static IHeroService HeroService() => CoreContainer.Resolve<IHeroService>();
    public static IPrefService PrefService() => CoreContainer.Resolve<IPrefService>();
    public static ILevelService LevelService() => CoreContainer.Resolve<ILevelService>();
    private static void InitilizationCheck()
    {
        if (_isBuilt)
            GD.PrintErr("How did you call this twice? Only one CoreProvider should exist.");
        if (!_isBuilt)
            BuildCoreContainer();
        _isBuilt = true;
    }
    /// <summary>
    /// Builds the core container with all core singletons.
    /// </summary>
    /// <remarks>
    /// The core container contains singletons that are essential for the application's core functionality.
    /// </remarks>
    private static void BuildCoreContainer()
    {
        if (_isBuilt)
            return;
        GD.PrintRich("[color=#00ff00]Registering Cores to CoreBox...[/color]");
        CoreContainer.Register<IAudioService, AudioService>();
        CoreContainer.Register<IClockService, ClockService>();
        CoreContainer.Register<IEventService, EventService>();
        CoreContainer.Register<IHeroService, HeroService>();
        CoreContainer.Register<IPrefService, PrefService>();
        CoreContainer.Register<ILevelService, LevelService>();
        GD.PrintRich("[color=#00ff00]Cores Registered.[/color]");
    }
}