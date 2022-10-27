using System;
using Godot;

public class Testing : Node2D
{
    [Export] private TypedNodePaths.TypedNodePath<IDamageable> damagae;
    [Export] private TypedNodePaths.TypedNodePath<AnimationPlayer> anim;
    [Export] private NodePath animTree;
    [Export] private PackedScene number;
}


public interface IDamageable { }