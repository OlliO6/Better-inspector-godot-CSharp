using System;
using BetterInspector;
using Godot;
using Health;

public class Testing : Node2D
{
    [Export, StartFoldout("Movement", position = FoldoutPosition.Bottom)] private float groundedSpeed;
    [Export] private float airSpeed;
    [Export] private float jumpHeight;
    [Export, EndFoldout] private bool canCrouch;

    [Export] private Weapon startWeapon;

    [Export, InFoldout("Movement")] private bool allowJump, allowSprint;
    [Export, InFoldout("Movement")] private Resource someRes;
}

public class Weapon : Godot.Object
{
}