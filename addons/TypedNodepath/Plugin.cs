#if TOOLS
namespace TypedNodePaths;

using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    private static Plugin instance;
    public static Plugin Instance => instance;
    public static bool hasInstance => IsInstanceValid(instance);

    public InspectorPlugin inspectorPlugin;

    public override void _EnterTree()
    {
        AddInspectorPlugin(inspectorPlugin = new());
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(inspectorPlugin);
    }
}

#endif