#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    public static Plugin instance;

    private FoldoutInspectorPlugin inspectorPlugin;

    public static Texture GetIcon(string name)
    {
        if (!IsInstanceValid(instance))
            return null;

        return instance.GetEditorInterface().GetBaseControl().Theme.GetIcon(name, "EditorIcons");
    }

    public override void _EnterTree()
    {
        inspectorPlugin = new();
        AddInspectorPlugin(inspectorPlugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(inspectorPlugin);
    }

    public override void _Process(float delta)
    {
        if (instance == null)
        {
            instance = this;

            // Reset inspector plugin on build
            RemoveInspectorPlugin(inspectorPlugin);
            inspectorPlugin = new();
            AddInspectorPlugin(inspectorPlugin);
        }
    }
}

#endif