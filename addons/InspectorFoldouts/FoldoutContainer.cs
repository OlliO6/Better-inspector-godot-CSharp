#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using Godot;

[Tool]
public class FoldoutContainer : Control
{
    public event Action<bool> Toggled;

    private string foldoutName;

    [Export] private NodePath checkBoxPath;

    private CheckBox _checkBox;

    public bool IsCollapsed
    {
        get => CheckBox.Pressed;
        set => CheckBox.Pressed = value;
    }

    public CheckBox CheckBox => _checkBox ??= GetNode<CheckBox>(checkBoxPath);

    public string FoldoutName
    {
        get => foldoutName;
        set
        {
            foldoutName = value;
            if (CheckBox != null) CheckBox.Text = value;
        }
    }

    public override void _Ready()
    {
        CheckBox.Text = FoldoutName;
        CheckBox.Connect("toggled", this, nameof(OnCollapsedToggled));
    }

    private void OnCollapsedToggled(bool toggled) => Toggled(toggled);
}

#endif