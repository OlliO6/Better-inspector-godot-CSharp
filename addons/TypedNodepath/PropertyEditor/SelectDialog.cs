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
                new HBoxContainer()
                .WitchChilds(
                    (filterText = new LineEdit()
                    {
                        SizeFlagsHorizontal = (int)SizeFlags.ExpandFill,
                        PlaceholderText = "Filter nodes",
                        RightIcon = Plugin.GetIcon("Search")
                    }),
                    new CheckButton()
                    {
                        Text = "Show unowned"
                    }
                    .Connected("toggled", this, nameof(OnShowUnownedNodes))
                ),
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

    private void OnShowUnownedNodes(bool toggled)
    {
        if (toggled)
        {
            UpdateNodeTree(false);
            return;
        }
        UpdateNodeTree();
    }

    private void UpdateNodeTree(bool onlyOwnNodes = true)
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

            if (node.Owner != rootNode && node != rootNode)
            {
                treeItem.Collapsed = true;
                treeItem.SetCustomColor(0, Colors.Cornsilk);
            }

            foreach (Node child in node.GetChildren())
            {
                if (onlyOwnNodes && child.Owner != rootNode)
                    continue;

                AddNodeRecursive(tree, tree.CreateItem(treeItem), child);
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
