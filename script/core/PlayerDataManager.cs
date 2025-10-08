namespace Core;

using Godot;
using Core.Interface;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class PlayerDataManager : IPlayerDataManager
{
    public Player GlobalPlayer { get; private set; } = null;
    public Dictionary<string, bool> UnlockedHeros { get; private set; }
    public Dictionary<string, bool> UnlockedEquipment { get; private set; }
    public Dictionary<string, bool> UnlockedWeapons { get; private set; }
    public Dictionary<string, bool> UnlockedItems { get; private set; }
    public Dictionary<string, bool> UnlockedAcheivments { get; private set; }
    public PlayerDataManager()
    {
        Json json = ResourceLoader.Load<Json>("res://DataIndex.tres") as Json;
        var data = Json.Stringify(json.Data);
        UnlockedHeros = BuildUnlockBook(data, UnlockBook.heros);
        UnlockedEquipment = BuildUnlockBook(data, UnlockBook.equipment);
        UnlockedWeapons = BuildUnlockBook(data, UnlockBook.weapons);
        UnlockedItems = BuildUnlockBook(data, UnlockBook.items);
        UnlockedAcheivments = BuildUnlockBook(data, UnlockBook.achievements);
    }
    public void SetGlobalPlayer(Player player)
    {
        if (GlobalPlayer != null) return;
        GlobalPlayer = player;
    }
    private Dictionary<string, bool> BuildUnlockBook(string input, UnlockBook book)
    {
        var result = new Dictionary<string, bool>();
        foreach (var line in input.Split("\n"))
        {
            if (line.StartsWith($"[{book.ToString()}]"))
            {
                var entries = line.Replace($"[{book.ToString()}]", "").Split(",");
                foreach (var entry in entries)
                {
                    var trimmedEntry = entry.Trim();
                    if (!string.IsNullOrEmpty(trimmedEntry) && !result.ContainsKey(trimmedEntry))
                    {
                        result.Add(trimmedEntry, false);
                    }
                }
                break;
            }
        }
        return result;
    }
    private enum UnlockBook
    {
        heros,
        equipment,
        weapons,
        items,
        achievements
    }
}