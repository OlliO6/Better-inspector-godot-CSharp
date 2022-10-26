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

    public string[] icons;

    public static Texture GetIcon(string name)
    {
        if (!hasInstance) return null;

        return instance.GetEditorInterface().GetBaseControl().Theme.GetIcon(name, "EditorIcons");
    }

    public override void _EnterTree()
    {
        instance = this;

        AddInspectorPlugin(inspectorPlugin = new());
    }

    public override void _ExitTree()
    {
        instance = null;

        RemoveInspectorPlugin(inspectorPlugin);
    }

    public override void _Process(float delta)
    {
        if (!hasInstance)
        {
            instance = this;

            // OnBuilded
        }
    }
}

#endif