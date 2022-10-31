using System;
using Godot;

[Tool]
public class CoolControl : Control
{
    [Export, TypedPath(typeof(Control))] private NodePath ctrl;
    [Export, TypedPath(typeof(Control))] private NodePath ctrl2;
    [Export] private NodePath ctrl3;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
}
