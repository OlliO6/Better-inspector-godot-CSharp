#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using Godot;

// Takes the next sibling and puts it in a foldout container
public class FoldoutContentAdder : Control
{
    public float indentionSize = 10;
    private FoldoutContainer foldout;

    public override void _Ready()
    {
        CallDeferred(nameof(DefferedReady));
    }

    private FoldoutContentAdder() { }

    public FoldoutContentAdder(FoldoutContainer foldout)
    {
        this.foldout = foldout;
    }

    private void DefferedReady()
    {
        Node nextSibling = GetParent()
                .GetChild(GetIndex() + 1);

        if (nextSibling is not Control content)
        {
            QueueFree();
            return;
        }

        content.SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill;
        foldout.AddContent(content);

        QueueFree();
    }
}

#endif