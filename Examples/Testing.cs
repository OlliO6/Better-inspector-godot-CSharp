using System;
using Godot;

public class Testing : Node2D
{
    [Export] private TypedNodePaths.NodePath<AnimationPlayer> anim;
    [Export] private NodePath animTree;
}
