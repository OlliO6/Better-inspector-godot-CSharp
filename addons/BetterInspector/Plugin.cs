#if TOOLS
namespace BetterInspector.Editor;

using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }
    public static bool HasInstance => IsInstanceValid(Instance);

    private FoldoutInspectorPlugin foldoutInspectorPlugin;
    private TypedPathsInspectorPlugin typedPathInspectorPlugin;

    public override void _EnterTree()
    {
        Instance = this;

        AddInspectorPlugin(typedPathInspectorPlugin = new());
        AddInspectorPlugin(foldoutInspectorPlugin = new());

        OnBuild();
    }

    public override void _ExitTree()
    {
        Instance = null;

        RemoveInspectorPlugin(typedPathInspectorPlugin);
        RemoveInspectorPlugin(foldoutInspectorPlugin);
    }

    public static Texture GetIcon(string name)
    {
        if (!HasInstance) return null;

        return Instance.GetEditorInterface().GetBaseControl().Theme.GetIcon(name, "EditorIcons");
    }

    public override void _Process(float delta)
    {
        if (!HasInstance)
        {
            Instance = this;
            OnBuild();
        }
    }

    private void OnBuild()
    {
        Reselect();

        // Reset inspector plugin
        RemoveInspectorPlugin(foldoutInspectorPlugin);
        foldoutInspectorPlugin = new();
        AddInspectorPlugin(foldoutInspectorPlugin);

        void Reselect()
        {
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