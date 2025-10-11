namespace Core.Interface;

using System.Collections.Generic;
/// <summary>
/// Interface for the DataService; this Service handles player data such as unlocked heros, Pickup, weapons, items, and achievements.
/// </summary>
public interface IDataService
{
    Dictionary<string, bool> UnlockedHeros { get; }
    Dictionary<string, bool> UnlockedPickup { get; }
    Dictionary<string, bool> UnlockedWeapons { get; }
    Dictionary<string, bool> UnlockedItems { get; }
    Dictionary<string, bool> UnlockedAcheivments { get; }
}