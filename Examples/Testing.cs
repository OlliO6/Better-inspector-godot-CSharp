using System;
using BetterInspector;
using Godot;
using Health;

public partial class Testing : Node2D
{
    event Action Events;
    [NodeRef(foldout = "")] public IDamageable damageableObj;
    [NodeRef(foldout = "")] public AnimationPlayer animationPlayer;
    [Export] private float value2;
    [Export, InFoldout("")] private float value;

    [Export, TypedPath(typeof(IDamageable)), StartFoldout("", position = FoldoutPosition.Bottom)]
    private NodePath damageManager;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
    [Export, TypedPath(typeof(Area2D))] private NodePath playerRange;
    [Export, TypedPath(typeof(Node))] private NodePath node;
    [Export, TypedPath(typeof(Node))] private NodePath node2;
    [Export] private ExampleResource res;
    [Export] private PackedScene number;
    [Export] private PackedScene number2;
    [Export] private PackedScene number3;
    [Export, EndFoldout] private PackedScene number4;

    [Export] private float value3;
    [Export] private float value4;
}