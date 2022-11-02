#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using Godot;

// Takes the next sibling and applies a indention to it
public class IndentionBuilder : HBoxContainer
{
    public float indentionSize = 10;

    public override void _Ready()
    {
        CallDeferred(nameof(DefferedReady));
    }

    private void DefferedReady()
    {
        Node nextSibling = GetParent()
                .GetChild(GetIndex() + 1);

        GetParent().RemoveChild(nextSibling);

        SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
        (nextSibling as Control).SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;

        this.WitchChilds(
            new Control()
            {
                RectMinSize = new(indentionSize, 0)
            },
            nextSibling
        );
    }
}

#endif