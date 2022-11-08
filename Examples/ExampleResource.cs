using System;
using Godot;

[BetterInspector.ResourceScriptPath("res://Examples/ExampleResource.cs")]
public class ExampleResource : Resource
{
    [Export] private float number;

    [Export] private bool _StartF_PropertyPaths;
    [Export] private string speedPath;
    [Export, BetterInspector.TypedPath(typeof(Node))] private NodePath wwww;
    [Export] private string jumpPath;
    [Export] private bool _EndF_;

    [Export] private float number2;
}
