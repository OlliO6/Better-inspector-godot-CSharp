using BetterInspector;
using Godot;

namespace RangeWeapons
{
    [ResourceScriptPath("res://Examples/Rifle.cs")]
    public class Rifle : Weapon
    {
        [Export, StartFoldout("Rifle")] private float bulletDamage;
        [Export, EndFoldout] private float timeBetweenShooting;
    }
}