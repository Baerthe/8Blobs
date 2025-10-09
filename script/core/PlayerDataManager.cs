namespace Core;
using Godot;
using Core.Interface;
using System.Collections.Generic;
public sealed class PlayerDataManager : IPlayerDataManager
{
    public Dictionary<string, bool> UnlockedHeros { get; private set; }
    public Dictionary<string, bool> UnlockedPickup { get; private set; }
    public Dictionary<string, bool> UnlockedWeapons { get; private set; }
    public Dictionary<string, bool> UnlockedItems { get; private set; }
    public Dictionary<string, bool> UnlockedAcheivments { get; private set; }
    private bool _isInitialized = false;
    public PlayerDataManager()
    {
        _isInitialized = false;
        Initilize();
    }
    private void Initilize()
    {
        Json json = ResourceLoader.Load<Json>("res://DataIndex.tres") as Json;
        var data = Json.Stringify(json.Data);
        UnlockedHeros = BuildUnlockBook(data, UnlockBook.heros);
        UnlockedPickup = BuildUnlockBook(data, UnlockBook.Pickup);
        UnlockedWeapons = BuildUnlockBook(data, UnlockBook.weapons);
        UnlockedItems = BuildUnlockBook(data, UnlockBook.items);
        UnlockedAcheivments = BuildUnlockBook(data, UnlockBook.achievements);
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]PlayerDataManager initialized.[/color]");
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
                        bool unlocked = false; // Default to locked; can implement save/load later
                        result.Add(trimmedEntry, unlocked);
                        GD.Print($"Added '{trimmedEntry}' to {book.ToString()} unlock book.");
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
        Pickup,
        weapons,
        items,
        achievements
    }
}