using System;
using Godot;
using Godot.Collections;

[Tool]
public class Testing : Node2D
{
    [Export, TypedPath(typeof(IDamageable))] private NodePath damagae;
    [Export, TypedPath(typeof(AnimationPlayer))] private NodePath anim;
    [Export] private NodePath animTree;
    [Export] private PackedScene number;

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