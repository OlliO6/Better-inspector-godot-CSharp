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
        WindowTitle = "Select a " + typeof(T).FullName;

        this.WitchChilds(
            new VBoxContainer()
            .WitchChilds(
                (filterText = new LineEdit()
                {
                    PlaceholderText = "Filter nodes",
                    RightIcon = Plugin.GetIcon("Search")
                }),
                (tree = new Tree()
                {
                    SizeFlagsVertical = (int)SizeFlags.ExpandFill,
                    SelectMode = Tree.SelectModeEnum.Single
                })
            )
        );

        UpdateNodeTree();

        GetCancel().Connect("pressed", this, nameof(Cancel));
        GetCloseButton().Connect("pressed", this, nameof(Cancel));
        GetOk().Connect("pressed", this, nameof(Confirm));
    }

    private void UpdateNodeTree()
    {
    }

    public NodePath<T> GetSelectedPathResult() => null;

    public void Confirm()
    {
    }

    public void Cancel()
    {

    }
}
