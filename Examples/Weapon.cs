using BetterInspector;
using Godot;
using Health;

// [ResourceScriptPath(@"C:\Users\olive\GodotProjects\BetterInspector\Examples\Weapon.cs")]
[Resource]
public partial class Weapon : Resource, IDamageable
{
    [Export] private float number;
    [Export] private float number2;
    [Export] private float number3;
}
