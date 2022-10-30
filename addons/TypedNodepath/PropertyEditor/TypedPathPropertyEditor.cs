#if TOOLS
namespace TypedNodePaths;

using System;
using System.Threading.Tasks;
using Godot;

[Tool]
public class TypedPathPropertyEditor : EditorProperty
{
    private Type type;

    private Button assignButton, clearButton;

    private bool isUpdating;

    private NodePath value;
    private SelectDialog selectDialog;

    public NodePath Value
    {
        get => value;
        set
        {
            if (value == this.value) return;

            this.value = value;

            if (!isUpdating) EmitChanged(GetEditedProperty(), value);
            RefreshAssignButtonVisual();
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
                        SizeFlagsHorizontal = (int)SizeFlags.ExpandFill
                    }),
                    (clearButton = new Button()
                    {
                        Icon = Plugin.GetIcon("Clear"),
                        Flat = true
                    })
                ),
                selectDialog = new SelectDialog(type)
            )
        );

        AddFocusable(assignButton);
        AddFocusable(clearButton);

        RefreshAssignButtonVisual();

        assignButton.Connect("pressed", this, nameof(OnAssignPressed));
        clearButton.Connect("pressed", this, nameof(OnClearPressed));
    }

    public override void UpdateProperty()
    {
        isUpdating = true;

        NodePath property = GetEditedObject().Get(GetEditedProperty()) as NodePath;
        Value = property.IsEmptyOrNull() ? null : property;

        isUpdating = false;
    }

    private async void OnAssignPressed()
    {
        NodePath selectedPath = await StartSelection();

        if (!selectedPath.IsEmptyOrNull())
            Value = selectedPath;
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

    private void RefreshAssignButtonVisual()
    {
        if (!Plugin.HasInstance || !IsInstanceValid(assignButton))
            return;

        if (Value.IsEmptyOrNull())
        {
            assignButton.Text = "Assign...";
            assignButton.Flat = false;
            assignButton.Icon = null;
            return;
        }
        assignButton.Text = Value;
        assignButton.Flat = true;
        Node node = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot().GetNodeOrNull(Value);
        assignButton.Icon = node != null ? Plugin.GetIcon(node.GetClass()) : null;
    }
}

#endif