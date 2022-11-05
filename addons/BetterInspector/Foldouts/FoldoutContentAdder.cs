#if TOOLS
namespace BetterInspector.Editor;

using System;
using Godot;

// Takes the next sibling and puts it in a foldout container
public class FoldoutContentAdder : Control
{
    public float indentionSize = 10;
    private FoldoutContainer foldout;

    public override void _Ready()
    {
        // DefferedReady();
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

        // Collapse all resource properties because otherwise there will initially will be weird if there're expanded
        if (nextSibling.IsClass("EditorPropertyResource"))
        {
            CollapseResource(nextSibling as Control);
        }

        QueueFree();
    }

    private void CollapseResource(Control resourceProp)
    {
        bool isCollapsed = resourceProp.GetChildCount() < 2;

        if (isCollapsed) return;

        Button button = resourceProp
                .GetChildOrNull<EditorResourcePicker>(0)
                ?.GetChildOrNull<Button>(0);

        if (button == null) return;

        button.EmitSignal("pressed");
        button.Pressed = false;
    }
}

#endif