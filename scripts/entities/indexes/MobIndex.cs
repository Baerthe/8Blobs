namespace Entities;
using Godot;
using Godot.Collections;
[GlobalClass]
public partial class MobIndex : Resource
{
    [Export] public string IndexDescription { get; private set; }
    [Export] public Array<PackedScene> MobTable { get; private set; } = new();
}