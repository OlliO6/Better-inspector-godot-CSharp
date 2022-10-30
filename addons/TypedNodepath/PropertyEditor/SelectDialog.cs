#if TOOLS
namespace TypedNodePaths;

using System;
using System.Collections.Generic;
using Godot;

public class SelectDialog : ConfirmationDialog
{
    private readonly Type type;
    private Tree tree;
    private LineEdit filterText;

    public SelectDialog(Type type)
    {
        this.type = type;
        WindowTitle = "Select a " + type.FullName;
        Resizable = true;

        // Construct the dialog content
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

        var rootNode = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot();

        // Get instanced Nodes with editable children
        var editablePaths = GD.Load<PackedScene>(rootNode.Filename)._Bundled["editable_instances"] as Godot.Collections.Array;
        List<Node> editableNodes = new(editablePaths.Count);

        foreach (NodePath path in editablePaths)
            editableNodes.Add(rootNode.GetNode(path));

        tree.Clear();
        var rootItem = tree.CreateItem();
        AddNodeRecursive(tree, rootItem, rootNode);

        void AddNodeRecursive(Tree tree, TreeItem treeItem, Node node)
        {
            treeItem.Collapsed = node.IsDisplayedFolded();
            treeItem.SetText(0, node.Name);
            treeItem.SetIcon(0, Plugin.GetIcon(node.GetClass()));

            if ((node.Owner != rootNode && node != rootNode) ||
                    (treeItem.GetParent() != null && (bool)treeItem.GetParent().HasMeta("Instanced")))
            {
                treeItem.Collapsed = true;
                treeItem.SetCustomColor(0, Colors.LightSteelBlue);
            }

            if (editableNodes.Contains(node) ||
                    (treeItem.GetParent() != null && treeItem.GetParent().HasMeta("Instanced")))
            {
                treeItem.SetMeta("Instanced", true);
            }

            foreach (Node child in node.GetChildren())
            {
                if ((onlyOwnNodes && child.Owner != rootNode))
                {
                    if (treeItem.HasMeta("Instanced"))
                        AddNodeRecursive(tree, tree.CreateItem(treeItem), child);

                    continue;
                }
                AddNodeRecursive(tree, tree.CreateItem(treeItem), child);
            }
        }
    }

    public NodePath GetSelectedPathResult() => tree.GetSelected()?.GetText(0);

    public void Confirm()
    {
    }

    public void Cancel()
    {
    }
}

#endif