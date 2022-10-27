#if TOOLS
namespace TypedNodePaths;

using System;
using System.Threading.Tasks;
using Godot;

[Tool]
public class TypedPathPropertyEditor<T> : EditorProperty
    where T : class
{
    private Button assignButton, clearButton;

    private NodePath<T> value;
    private SelectDialog<T> selectDialog;

    public NodePath<T> Value
    {
        get => value;
        set
        {
            this.value = value;
            EmitChanged(GetEditedProperty(), Value);
            RefreshAssignButtonText();
        }
    }

    public TypedPathPropertyEditor()
    {
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
                selectDialog = new SelectDialog<T>()
            )
        );

        AddFocusable(assignButton);
        AddFocusable(clearButton);
        RefreshAssignButtonText();

        assignButton.Connect("pressed", this, nameof(OnAssignPressed));
        clearButton.Connect("pressed", this, nameof(OnClearPressed));
    }

    public override void UpdateProperty()
    {
        // Read the current value from the property.
        var newValue = (NodePath<T>)GetEditedObject().Get(GetEditedProperty());
        if (newValue == Value)
            return;
    }

    private async void OnAssignPressed()
    {
        NodePath<T> selectedPath = await StartSelection();

        if (selectedPath != null) Value = selectedPath;
    }

    private async Task<NodePath<T>> StartSelection()
    {
        selectDialog.PopupCenteredMinsize(new(500, 800));

        await ToSignal(selectDialog, "popup_hide");

        return selectDialog.GetSelectedPathResult();
    }

    private void OnClearPressed()
    {
        Value = null;
    }

    private void RefreshAssignButtonText()
        => assignButton.Text = Value is null ? "Assign..." : Value.path;
}

#endif