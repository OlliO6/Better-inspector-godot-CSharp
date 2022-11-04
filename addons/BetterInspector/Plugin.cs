#if TOOLS
namespace BetterInspector;

using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    public static Plugin Instance { get; private set; }
    public static bool HasInstance => IsInstanceValid(Instance);


    public override void _EnterTree()
    {

    }

    public override void _ExitTree()
    {

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