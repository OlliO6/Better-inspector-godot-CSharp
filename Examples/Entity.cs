using System.Collections.Generic;
using BetterInspector;
using Godot;

public class Entity : Node
{
    [Export] private Weapon weapon;

    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
}
