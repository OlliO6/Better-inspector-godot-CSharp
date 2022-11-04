using System;
using BetterInspector;
using Godot;
using Godot.Collections;

public class Testing : Node2D
{
    event Action Event;
    [Export] private float value;
    [Export] private float value2;

    [Export, TypedPath(typeof(IDamageable)), StartFoldout("References")]
    private NodePath damagae;
    [Export, TypedPath(typeof(Tree))] private NodePath anim;
    [Export] private ExampleResource res;
    [Export] private PackedScene number;
    [Export] private PackedScene number2;
    [Export] private PackedScene number3;
    [Export, EndFoldout] private PackedScene number4;

    [Export] private float value3;
    [Export] private float value4;
}


public interface IDamageable { }