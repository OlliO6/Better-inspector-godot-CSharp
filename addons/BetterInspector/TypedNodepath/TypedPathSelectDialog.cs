#if TOOLS
namespace BetterInspector.Editor;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BetterInspector.Utilities;
using Godot;

public class TypedPathSelectDialog : ConfirmationDialog
{
    private bool assigning, onlyShowOwnNodes = true;
    private readonly Type type;
    private Tree tree;
    private LineEdit filterText;
    private CheckButton showUnownedToggle;

    public TypedPathSelectDialog(Type type)
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
                    })
                    .Connected("text_changed", this, nameof(OnFilterTextChanged)),
                    (showUnownedToggle = new CheckButton()
                    {
                        Text = "Show unowned"
                    }
                    .Connected("toggled", this, nameof(OnShowUnownedNodesToggled)))
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

    private void OnFilterTextChanged(string pattern)
    {
        UpdateNodeTree();
        HashSet<TreeItem> dontRemove = new();
        ExpandAll(tree.GetRoot());
        FilterRecursive(tree.GetRoot());
        RemoveNotMatchingNodes(tree.GetRoot());
        tree.Update();

        void FilterRecursive(TreeItem item)
        {
            if (item == null) return;

            TreeItem current = item;

            while (current != null)
            {
                if (Regex.IsMatch(current.GetText(0), pattern,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant))
                    DontRemove(current);

                FilterRecursive(current.GetChildren());
                current = current.GetNext();
            }
        }

        void DontRemove(TreeItem item)
        {
            TreeItem current = item;

            while (current != null)
            {
                dontRemove.Add(current);
                current = current.GetParent();
            }
        }

        void ExpandAll(TreeItem item)
        {
            if (item == null) return;

            TreeItem current = item;

            while (current != null)
            {
                current.Collapsed = false;
                ExpandAll(current.GetChildren());
                current = current.GetNext();
            }
        }

        void RemoveNotMatchingNodes(TreeItem item)
        {
            TreeItem current = item;

            while (current != null)
            {
                if (!dontRemove.Contains(current))
                {
                    current.GetParent()?.RemoveChild(current);
                    current.CallDeferred("free");
                }
                RemoveNotMatchingNodes(current.GetChildren());
                current = current.GetNext();
            }
        }
    }

    private void OnItemDoubleClicked()
    {
        if (tree.GetSelected() == null) return;

        assigning = true;
        Hide();
    }

    private void OnAboutToShow()
    {
        filterText.Clear();
        showUnownedToggle.Pressed = false;
        tree.CallDeferred("grab_focus");
        assigning = false;
        UpdateNodeTree();
    }

    private void OnShowUnownedNodesToggled(bool toggled)
    {
        onlyShowOwnNodes = !toggled;
        UpdateNodeTree();
    }

    private void UpdateNodeTree()
    {
        if (!Plugin.HasInstance)
            return;

        var rootNode = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot();

        // Get instanced Nodes with editable children
        PackedScene packedScene = rootNode.Filename != string.Empty ? GD.Load<PackedScene>(rootNode.Filename) : null;
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
        rootItem.Collapsed = false;

        void AddNodeRecursive(Tree tree, TreeItem treeItem, Node node, bool currentUnowned = false)
        {
            ProcessNode(treeItem, node, currentUnowned);

            foreach (Node child in node.GetChildren())
            {
                if (child.Owner != rootNode)
                {
                    if (onlyShowOwnNodes)
                    {
                        if (treeItem.HasMeta("Instanced") && child.Owner == treeItem.GetMeta("Instanced"))
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
                Type nodeType = node.GetInEditorTypeCached();
                bool typeAssignable = type.IsAssignableFrom(nodeType);

                treeItem.Collapsed = true;
                treeItem.SetText(0, node.Name);
                treeItem.SetIcon(0, Plugin.GetIcon(node.GetClass()));
                treeItem.SetEditable(0, false);
                treeItem.SetSelectable(0, typeAssignable);

                if (editableNodes.Contains(node))
                    treeItem.SetMeta("Instanced", node);

                else if (treeItem.GetParent() != null && treeItem.GetParent().HasMeta("Instanced"))
                    treeItem.SetMeta("Instanced", treeItem.GetParent().GetMeta("Instanced"));

                if (typeAssignable)
                {
                    Expand(treeItem);

                    treeItem.SetCustomColor(0,
                        currentUnowned ? Colors.LightSteelBlue : Colors.White);
                    return;
                }

                treeItem.SetCustomColor(0,
                    currentUnowned ? Colors.DarkSlateGray : Colors.DimGray);
            }
        }

        static void Expand(TreeItem item)
        {
            TreeItem current = item.GetParent();

            while (current != null && current.Collapsed)
            {
                current.Collapsed = false;
                current = current.GetParent();
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
            StringBuilder path = new();
            TreeItem current = item;

            while (current.GetParent() != null)
            {
                path = path.Insert(0, current.GetText(0) + "/");
                current = current.GetParent();
            }

            path = path.Insert(0, "./");
            return path.ToString();
        }
    }

    public void Confirm()
    {
        assigning = true;
    }
}

#endif