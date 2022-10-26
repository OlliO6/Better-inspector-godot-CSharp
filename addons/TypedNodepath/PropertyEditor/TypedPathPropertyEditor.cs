#if TOOLS
namespace TypedNodePaths;

using System;
using Godot;

[Tool]
public class TypedPathPropertyEditor : EditorProperty
{
    private Button assignButton, clearButton;

    private NodePath value;
    private readonly Type forType;

    public NodePath Value
    {
        get => value;
        set
        {
            this.value = value;
            EmitChanged(GetEditedProperty(), Value);
        }
    }

    public TypedPathPropertyEditor(Type forType)
    {
        this.forType = forType;

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

        assignButton.Connect("pressed", this, nameof(OnAssignPressed));
        clearButton.Connect("pressed", this, nameof(OnClearPressed));
    }

    public override void UpdateProperty()
    {

    }

    private void OnAssignPressed()
    {
        value = (GetEditedObject() as Node).GetChild(0).Name;
        GD.Print("ASSIGNING");
    }

    private void OnClearPressed()
    {
        Value = null;
        GD.Print("CLEARED");
    }

    private void RefreshAssignButtonText()
        => assignButton.Text = Value is null ? "Assign..." : Value;
}

#endif