using System;
using Godot;

public class Testing : Node2D
{
    [Export] private TypedNodePaths.NodePath<IDamageable> damagae;
    [Export] private TypedNodePaths.NodePath<AnimationPlayer> anim;
    [Export] private NodePath animTree;
}


public interface IDamageable { }