using System;
using BetterInspector;
using Godot;
using Godot.Collections;

public class Testing : Node2D
{
    event Action Event;
    [Export] private float value2;
    [Export, InFoldout("LOLS")] private float value;

    [Export, TypedPath(typeof(IDamageable)), StartFoldout("References", position = FoldoutPosition.Bottom)]
    private NodePath damagae;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
    [Export, TypedPath(typeof(Area2D))] private NodePath playerRange;
    [Export] private ExampleResource res;
    [Export] private PackedScene number;
    [Export] private PackedScene number2;
    [Export] private PackedScene number3;
    [Export, EndFoldout] private PackedScene number4;

    [Export] private float value3;
    [Export] private float value4;
}


public interface IDamageable { }