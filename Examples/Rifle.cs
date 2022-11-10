using BetterInspector;
using Godot;

namespace RangeWeapons
{
    [Resource]
    public partial class Rifle : Weapon
    {
        [Export, StartFoldout("Rifle")] private float bulletDamage;
        [Export, EndFoldout] private float timeBetweenShooting;
    }
}