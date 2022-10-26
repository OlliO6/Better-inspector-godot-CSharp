namespace TypedNodePaths;

using System;
using System.Runtime.CompilerServices;
using Godot;

public class SelectDialog<T> : ConfirmationDialog
    where T : class
{
    private Tree tree;
    private LineEdit filterText;

    public SelectDialog()
    {
        WindowTitle = "SelectNode";

        this.WitchChilds(
            new VBoxContainer()
            .WitchChilds(
                (filterText = new LineEdit() { PlaceholderText = "Filter nodes" }),
                (tree = new Tree()
                {
                    SizeFlagsVertical = (int)SizeFlags.ExpandFill
                })
            )
        );

        GetCancel().Connect("pressed", this, nameof(Cancel));
        GetCloseButton().Connect("pressed", this, nameof(Cancel));
        GetOk().Connect("pressed", this, nameof(Confirm));
    }

    public NodePath<T> GetSelectedPathResult() => null;

    public void Confirm()
    {

    }

    public void Cancel()
    {

    }
}
