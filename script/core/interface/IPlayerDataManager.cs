namespace Core.Interface;

using System.Collections.Generic;
/// <summary>
/// Interface for the PlayerDataManager; this manager handles player data such as unlocked heros, equipment, weapons, items, and achievements.
/// </summary>
public interface IPlayerDataManager : ICore
{
    Dictionary<string, bool> UnlockedHeros { get; }
    Dictionary<string, bool> UnlockedEquipment { get; }
    Dictionary<string, bool> UnlockedWeapons { get; }
    Dictionary<string, bool> UnlockedItems { get; }
    Dictionary<string, bool> UnlockedAcheivments { get; }
}