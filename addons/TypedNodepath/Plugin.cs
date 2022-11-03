#if TOOLS
namespace BetterInspector.TypedNodePaths;

using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    private static Plugin instance;
    public static Plugin Instance => instance;
    public static bool HasInstance => IsInstanceValid(instance);

    public InspectorPlugin inspectorPlugin;

    public static Texture GetIcon(string name)
    {
        if (!HasInstance) return null;

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
        if (!HasInstance)
        {
            instance = this;

            var selection = GetEditorInterface().GetSelection().GetSelectedNodes();

            foreach (Node node in selection)
            {
                GetEditorInterface().GetSelection().RemoveNode(node);
            }
            foreach (Node node in selection)
            {
                GetEditorInterface().GetSelection().AddNode(node);
            }
        }
    }
}

#endif