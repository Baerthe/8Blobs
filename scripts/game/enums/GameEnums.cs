namespace Game;
/// <summary>
/// The relevant enums used across game systems. Initially all values added were spaced by 4s, this is to give room for future additions, but allowing them to still be alphabetical.
/// </summary>
public enum LevelType : byte
{
    Plains = 0,
    Forest = 4,
    Village = 8,
    City = 12,
    Swamp = 16,
    Unset = 255
}