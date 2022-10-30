using System;
using Godot;

[Tool]
public class CoolControl : Control
{
    [Export, TypedPath(typeof(IDamageable))] private NodePath damagae;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
}
