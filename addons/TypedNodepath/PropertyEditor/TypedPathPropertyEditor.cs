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
            if (value == this.value) return;

            this.value = value;

            Resource resource = new Resource();
            resource.SetScript(GD.Load("res://addons/TypedNodepath/TypedNodePath.cs"));
            resource.Set("path", Value.path);
            EmitChanged(GetEditedProperty(), resource);
            RefreshAssignButtonVisual();
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
        RefreshAssignButtonVisual();

        assignButton.Connect("pressed", this, nameof(OnAssignPressed));
        clearButton.Connect("pressed", this, nameof(OnClearPressed));
    }

    public override void UpdateProperty()
    {
        // Read the current value from the property.
        Resource resource = (Resource)GetEditedObject().Get(GetEditedProperty());
        var newValue = new NodePath<T>((NodePath)resource.Get("path"));
        if (newValue == Value)
            return;

        Value = newValue;
    }

    private async void OnAssignPressed()
    {
        NodePath<T> selectedPath = await StartSelection();

        if (selectedPath != null)
            Value = selectedPath;
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

    private void RefreshAssignButtonVisual()
    {
        if (Value is null)
        {
            assignButton.Text = "Assign...";
            assignButton.Flat = false;
            assignButton.Icon = null;
            return;
        }
        assignButton.Text = Value.path;
        assignButton.Flat = true;
        Node node = Plugin.Instance.GetEditorInterface().GetEditedSceneRoot().GetNodeOrNull(Value.path);
        assignButton.Icon = Plugin.GetIcon(node != null ? node.GetClass() : null);
    }
}

#endif