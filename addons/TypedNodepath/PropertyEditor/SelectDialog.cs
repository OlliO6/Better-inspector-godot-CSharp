#if TOOLS
namespace TypedNodePaths;

using System;
using System.Collections.Generic;
using Godot;

public class SelectDialog : ConfirmationDialog
{
    private bool assigning;
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
                .Connected("item_activated", this, nameof(OnItemDoubleClicked))
            )
        );

        this.Connect("about_to_show", this, nameof(OnAboutToShow));
        GetOk().Connect("pressed", this, nameof(Confirm));
    }

    private void OnItemDoubleClicked()
    {
        if (tree.GetSelected() == null) return;

        assigning = true;
        Hide();
    }

    private void OnAboutToShow()
    {
        assigning = false;
        UpdateNodeTree();
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
        if (!Plugin.HasInstance)
            return;

        var rootNode = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot();

        // Get instanced Nodes with editable children
        PackedScene packedScene = GD.Load<PackedScene>(rootNode.Filename);
        List<Node> editableNodes = new();

        if (packedScene != null)
        {
            var editablePaths = packedScene._Bundled["editable_instances"] as Godot.Collections.Array;

            foreach (NodePath path in editablePaths)
                editableNodes.Add(rootNode.GetNode(path));
        }

        tree.Clear();
        var rootItem = tree.CreateItem();
        AddNodeRecursive(tree, rootItem, rootNode);

        void AddNodeRecursive(Tree tree, TreeItem treeItem, Node node, bool currentUnowned = false)
        {
            ProcessNode(treeItem, node, currentUnowned);

            foreach (Node child in node.GetChildren())
            {
                if (child.Owner != rootNode)
                {
                    if (onlyOwnNodes)
                    {
                        if (treeItem.HasMeta("Instanced"))
                            AddNodeRecursive(tree, tree.CreateItem(treeItem), child, true);
                        continue;
                    }
                    AddNodeRecursive(tree, tree.CreateItem(treeItem), child, true);
                    continue;
                }
                AddNodeRecursive(tree, tree.CreateItem(treeItem), child, currentUnowned);
            }

            void ProcessNode(TreeItem treeItem, Node node, bool currentUnowned)
            {
                Type nodeType = node.GetInEditorType();

                bool tyeAssignable = type.IsAssignableFrom(nodeType);

                treeItem.Collapsed = false;
                treeItem.SetText(0, node.Name);
                treeItem.SetIcon(0, Plugin.GetIcon(node.GetClass()));
                treeItem.SetEditable(0, false);
                treeItem.SetSelectable(0, tyeAssignable);

                if (editableNodes.Contains(node) ||
                        (treeItem.GetParent() != null && treeItem.GetParent().HasMeta("Instanced")))
                    treeItem.SetMeta("Instanced", true);

                treeItem.SetCustomColor(0, currentUnowned ? (tyeAssignable ? Colors.LightSteelBlue : Colors.DarkSlateGray) :
                        (tyeAssignable ? Colors.White : Colors.DimGray));
            }
        }
    }

    public NodePath GetSelectedPathResult()
    {
        TreeItem selected = tree.GetSelected();

        if (assigning && selected != null)
            return GetPathTo(tree.GetSelected());

        return null;

        static NodePath GetPathTo(TreeItem item)
        {
            string path = string.Empty;

            TreeItem current = item;

            while (current.GetParent() != null)
            {
                path = path.Insert(0, current.GetText(0) + "/");
                current = current.GetParent();
            }

            return path;
        }
    }

    public void Confirm()
    {
        assigning = true;
    }
}

#endif