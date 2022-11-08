using BetterInspector;
using Godot;

[ResourceScriptPath("res://Examples/Rifle.cs")]
public class Rifle : Weapon
{
    [Export, StartFoldout("Rifle")] private float bulletDamage;
    [Export, EndFoldout] private float timeBetweenShooting;
}