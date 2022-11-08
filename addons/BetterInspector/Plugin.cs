#if TOOLS
namespace BetterInspector.Editor;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Utilities;

[Tool]
public class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }
    public static bool HasInstance => IsInstanceValid(Instance);

    private ResourcePicker.Manager resManager;
    private FoldoutInspectorPlugin foldoutInspectorPlugin;
    private TypedPathsInspectorPlugin typedPathInspectorPlugin;

    public override void _EnterTree()
    {
        Instance = this;

        Reset();
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

            Reset();
        }
    }

    private async void Reset()
    {
        // Clear type cache
        foreach (var key in new List<Godot.Object>(TypeCache.Cache.Keys))
            TypeCache.Instance.Remove(key);

        TypeCache.Cache.Clear();

        resManager?.QueueFree();
        AddChild(resManager = new(this));

        await ToSignal(GetTree(), "idle_frame");

        // Reset inspector plugins (order matters(last will parse property first))
        RestartInspectorPlugin(ref typedPathInspectorPlugin);

        // Foldout inspector plugin needs to be added at last of all
        await ToSignal(GetTree(), "idle_frame");
        RestartInspectorPlugin(ref foldoutInspectorPlugin);

        Reselect();

        void Reselect()
        {
            var selection = GetEditorInterface().GetSelection().GetSelectedNodes();

            SetSelectedNodes(selection);
        }
    }

    public void SetSelectedNodes(Godot.Collections.Array nodes)
    {
        var selection = GetEditorInterface().GetSelection().GetSelectedNodes();

        foreach (Node node in selection)
        {
            GetEditorInterface().GetSelection().RemoveNode(node);
        }
        foreach (Node node in nodes)
        {
            GetEditorInterface().GetSelection().AddNode(node);
        }
    }

    private void RestartInspectorPlugin<T>(ref T inspectorPlugin)
        where T : EditorInspectorPlugin, new()
    {
        if (IsInstanceValid(inspectorPlugin))
            RemoveInspectorPlugin(inspectorPlugin);

        inspectorPlugin = new();
        AddInspectorPlugin(inspectorPlugin);
    }
}

#endif