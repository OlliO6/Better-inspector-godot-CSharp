using System;
using BetterInspector;
using Godot;
using Godot.Collections;

public class Testing : Node2D
{
    event Action Event;
    [Export] bool _StartF_References;
    [Export, TypedPath(typeof(IDamageable))] private NodePath damagae;
    [Export, TypedPath(typeof(Tree))] private NodePath anim;
    [Export] private PackedScene number;
    [Export] bool _EndF_;

    // public override Godot.Collections.Array _GetPropertyList()
    // {
    //     return new Godot.Collections.Array()
    //     {
    //         new Dictionary()
    //         {
    //             { "name", "Test Node" },
    //             { "type", Variant.Type.Nil },
    //             { "usage", PropertyUsageFlags.Category }
    //         },
    //         new Dictionary()
    //         {
    //             { "name", "References" },
    //             { "type", Variant.Type.Nil },
    //             { "usage", PropertyUsageFlags.Group }
    //         },
    //         new Dictionary()
    //         {
    //             { "name", "damagae" },
    //             { "type", Variant.Type.Object },
    //             { "usage", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable }
    //         },
    //         new Dictionary()
    //         {
    //             { "name", "anim" },
    //             { "type", Variant.Type.Object },
    //             { "usage", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable }
    //         },
    //         new Dictionary()
    //         {
    //             { "name", "animTree" },
    //             { "type", Variant.Type.NodePath },
    //             { "usage", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable }
    //         },
    //     };
    // }
}


public interface IDamageable { }