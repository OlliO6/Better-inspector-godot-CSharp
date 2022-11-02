#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    private FoldoutInspectorPlugin inspectorPlugin;

    public override void _EnterTree()
    {
        inspectorPlugin = new();
        AddInspectorPlugin(inspectorPlugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(inspectorPlugin);
    }
}

#endif