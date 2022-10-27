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
        Resizable = true;

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

        UpdateNodeTree(false);

        GetCancel().Connect("pressed", this, nameof(Cancel));
        GetCloseButton().Connect("pressed", this, nameof(Cancel));
        GetOk().Connect("pressed", this, nameof(Confirm));
    }

    private void UpdateNodeTree(bool onlyOwnNodes)
    {
        if (!Plugin.hasInstance)
            return;

        tree.Clear();

        var rootItem = tree.CreateItem();
        var rootNode = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot();

        AddNodeRecursive(tree, rootItem, rootNode);

        void AddNodeRecursive(Tree tree, TreeItem treeItem, Node node)
        {
            treeItem.Collapsed = node.IsDisplayedFolded();
            treeItem.SetText(0, node.Name);
            treeItem.SetIcon(0, Plugin.GetIcon(node.GetClass()));

            GD.Print(node.Name, " Child count: ", node.GetChildCount());

            foreach (Node child in node.GetChildren())
            {
                if (onlyOwnNodes && child.Owner != rootNode)
                    continue;

                AddNodeRecursive(
                   tree,
                   tree.CreateItem(treeItem),
                   child);
            }
        }
    }

    public NodePath<T> GetSelectedPathResult() => null;

    public void Confirm()
    {
    }

    public void Cancel()
    {
    }
}
