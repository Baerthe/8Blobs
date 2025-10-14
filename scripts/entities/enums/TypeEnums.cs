namespace Entities;
/// <summary>
/// Global enumerations used across various entities in the game.
/// </summary>
public enum DamageType : byte
{
    Blunt = 0,
    Slash = 2,
    Pierce = 4,
    Arcane = 6,
}
/// <summary>
/// An enumeration of elemental types that can be used for various game mechanics.
/// </summary>
public enum ElementType : byte
{
    None = 0,
    Fire = 2,
    Water = 4,
    Earth = 6,
    Air = 8,
    Light = 10,
    Dark = 12,
    Electric = 14,
    Ice = 16
}
public enum RarityType : byte
{
    Basic = 0,
    Common = 2,
    Uncommon = 4,
    Rare = 6,
    Epic = 8,
    Legendary = 10,
    Mythic = 12,
    Ascendant = 14,
    Cosmic = 16,
    Eldritch = 18,
    Multiversal = 20,
    Omniversal = 22
}