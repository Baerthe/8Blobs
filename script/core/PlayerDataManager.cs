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
        var json = ResourceLoader.Load<Json>("res://DataIndex.tres");
        if (json == null)
        {
            GD.PrintErr("Could not load DataIndex.tres; cannot build unlock books.");
            return;
        }
        UnlockedHeros = BuildUnlockBook(json, UnlockBook.heros);
        UnlockedEquipment = BuildUnlockBook(json, UnlockBook.equipment);
        UnlockedWeapons = BuildUnlockBook(json, UnlockBook.weapons);
        UnlockedItems = BuildUnlockBook(json, UnlockBook.items);
        UnlockedAcheivments = BuildUnlockBook(json, UnlockBook.achievements);
    }
    public void SetGlobalPlayer(Player player)
    {
        if (GlobalPlayer != null) return;
        GlobalPlayer = player;
    }
    private Dictionary<string, bool> BuildUnlockBook(Json input, UnlockBook book)
    {

        return null;
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