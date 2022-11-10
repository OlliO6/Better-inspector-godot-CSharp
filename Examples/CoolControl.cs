using System;
using BetterInspector;
using Godot;

public partial class CoolControl : Control
{
    [Export] private float number;
    [Export] private float number2;
    [Export] private float number3;

    [NodeRef] public Particles2D dust;

    [Export] bool _StartF_AtTop_Door_Cell_Positions;
    [Export, TypedPath(typeof(Control))] private NodePath ctrl;
    [Export, TypedPath(typeof(Control))] private NodePath ctrl2;
    [Export] private NodePath ctrl3;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
    [Export] bool _EndF_Door_Cell_Positions;

    [Export] private float number4;

    partial void OnReady()
    {
        // Code here
    }
}
