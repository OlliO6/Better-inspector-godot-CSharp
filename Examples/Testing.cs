using System;
using BetterInspector;
using Godot;
using Health;
using RangeWeapons;

public class Testing : Node2D
{
    [Export, StartFoldout("Movement", position = FoldoutPosition.Bottom)] private float groundedSpeed;
    [Export] private float airSpeed;
    [Export] private float jumpHeight;
    [Export, EndFoldout] private bool canCrouch;

    [Export] private ExampleResource res;
    [Export] private Weapon startWeapon;
    [Export] private Rifle rifle;

    [Export, InFoldout("Movement")] private bool allowJump, allowSprint;
    [Export] private Resource someRes;
    [Export] private Texture tex;
    [Export] private Texture3D tex3d;
}
