namespace Core;
/// <summary>
/// The Enums used for Mobs
/// </summary>
public enum MobMovement : byte
{
    CurvedDirection = 0,
    PlayerAttracted = 1,
    RandomDirection = 2,
    ZigZagSway = 3,
    DashDirection = 4,
    Stationary = 5
}
public enum MobLevel : byte
{
    Basic = 1,
    Advanced = 2,
    Elite = 3,
    Boss = 4
}
public enum MobAbility : byte
{
    None = 0,
    Poison = 1,
    Healer = 2,
    Explodes = 3,
    Aura = 4
}