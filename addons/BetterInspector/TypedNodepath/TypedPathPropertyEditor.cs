#if TOOLS
namespace BetterInspector.Editor;

using System;
using System.Threading.Tasks;
using BetterInspector.Utilities;
using Godot;

[Tool]
public class TypedPathPropertyEditor : EditorProperty
{
    private Type type;

    private Button assignButton, clearButton, selectButton;

    private bool isUpdating;

    private NodePath value;
    private TypedPathSelectDialog selectDialog;

    public NodePath Value
    {
        get => value;
        set
        {
            if (value == this.value) return;

            this.value = value;

            if (!isUpdating) EmitChanged(GetEditedProperty(), value);
            RefreshVisual();
        }
    }

    public TypedPathPropertyEditor() { }

    public TypedPathPropertyEditor(Type type)
    {
        this.type = type;
    }

    public override void _Ready()
    {
        if (!Plugin.HasInstance)
            return;

        // Construct property editor buttons
        this.WitchChilds(
            new PanelContainer()
            .Setted("custom_styles/panel", new StyleBoxFlat()
            {
                BgColor = new("202431")
            })
            .WitchChilds(
                new HBoxContainer()
                {
                    AnchorRight = 1,
                    AnchorBottom = 1
                }
                .Setted("custom_constants/separation", 0)
                .WitchChilds(
                    (assignButton = new Button()
                    {
                        SizeFlagsHorizontal = (int)SizeFlags.ExpandFill,
                        ClipText = true
                    })
                    .Connected("pressed", this, nameof(OnAssignPressed)),
                    (selectButton = new Button()
                    {
                        Icon = Plugin.GetIcon("ListSelect"),
                        Flat = true
                    })
                    .Connected("pressed", this, nameof(OnSelectPressed)),
                    (clearButton = new Button()
                    {
                        Icon = Plugin.GetIcon("Clear"),
                        Flat = true
                    })
                    .Connected("pressed", this, nameof(OnClearPressed))
                ),
                selectDialog = new TypedPathSelectDialog(type)
            )
        );

        AddFocusable(assignButton);
        AddFocusable(clearButton);

        RefreshVisual();
    }

    public override void UpdateProperty()
    {
        isUpdating = true;

        NodePath property = GetEditedObject().Get(GetEditedProperty()) as NodePath;

        if (GetEditedObject() is Node editedNode && property != null)
        {
            Node pointsTo = editedNode.GetNodeOrNull(property);

            if (!type.IsAssignableFrom(pointsTo?.GetInEditorTypeCached()))
            {
                isUpdating = false;
                Value = null;
                return;
            }
        }

        Value = property.IsEmptyOrNull() ? null : property;
        isUpdating = false;
    }

    private async void OnAssignPressed()
    {
        NodePath selectedPath = await StartSelection();

        if (!selectedPath.IsEmptyOrNull())
        {
            if (GetEditedObject() is not Node editedNode || !Plugin.HasInstance)
            {
                Value = selectedPath;
                return;
            }

            Node pointsTo = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot().GetNodeOrNull(selectedPath);

            if (pointsTo == null)
            {
                GD.PrintErr($"Selected invalid node path: '{selectedPath}'");
                return;
            }

            Value = editedNode.GetPathTo(pointsTo);
        }
    }

    private async Task<NodePath> StartSelection()
    {
        selectDialog.PopupCenteredMinsize(new(500, 800));

        await ToSignal(selectDialog, "popup_hide");

        return selectDialog.GetSelectedPathResult();
    }

    private void OnClearPressed()
    {
        Value = null;
    }

    private void OnSelectPressed()
    {
        if (!Plugin.HasInstance) return;

        Plugin.Instance.SetSelectedNodes(new(
            ((GetEditedObject() is Node node) ? node : Plugin.Instance.GetEditorInterface().GetEditedSceneRoot())
                    .GetNode(GetEditedObject().Get(GetEditedProperty()) as NodePath)));
    }

    private void RefreshVisual()
    {
        if (Value.IsEmptyOrNull())
        {
            assignButton.Text = "Assign...";
            assignButton.Flat = false;
            assignButton.Icon = null;
            selectButton.Disabled = true;
            return;
        }

        Node from = (GetEditedObject() is Node editedNodee) ? editedNodee : Plugin.Instance.GetEditorInterface().GetEditedSceneRoot();
        Node node = from.GetNodeOrNull(Value);

        selectButton.Disabled = false;
        assignButton.Flat = true;
        assignButton.HintTooltip = Value;

        if (node == null)
        {
            assignButton.Text = Value;
            return;
        }

        assignButton.Text = node.Name;
        assignButton.Icon = Plugin.GetIcon(node.GetClass());
    }
}

#endif