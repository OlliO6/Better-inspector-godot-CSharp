#if TOOLS
namespace TypedNodePaths;

using System;
using Godot;

[Tool]
public class TypedPathPropertyEditor<T> : EditorProperty
    where T : class
{
    private Button assignButton, clearButton;

    private NodePath<T> value;

    public NodePath<T> Value
    {
        get => value;
        set
        {
            this.value = value;
            EmitChanged(GetEditedProperty(), Value);
        }
    }

    public TypedPathPropertyEditor()
    {
        this.WitchChilds(
            new HBoxContainer()
                    .Setted("custom_constants/separation", 0)
            .WitchChilds(
                (assignButton = new Button()
                {
                    SizeFlagsHorizontal = (int)SizeFlags.ExpandFill
                }),
                (clearButton = new Button()
                {
                    Icon = GD.Load<Texture>("res://addons/TypedNodepath/Icons/Clear.png")
                })
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

    private void OnAssignPressed()
    {
        Value = new((GetEditedObject() as Node).GetChild(0).Name);
        GD.Print("ASSIGNING");
    }

    private void OnClearPressed()
    {
        Value = null;
        GD.Print("CLEARED");
    }

    private void RefreshAssignButtonText()
        => assignButton.Text = Value is null ? "Assign..." : Value.path;
}

#endif