using System;
using BetterInspector;
using Godot;

[Tool]
public class CoolControl : Control
{
    [Export] bool _StartF_Door_Cell_Positions;
    [Export, TypedPath(typeof(Control))] private NodePath ctrl;
    [Export, TypedPath(typeof(Control))] private NodePath ctrl2;
    [Export] private NodePath ctrl3;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
    [Export] bool _EndF_Door_Cell_Positions;
}
