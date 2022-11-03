#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using Godot;

[Tool]
public class FoldoutContainer : VBoxContainer
{
    public event Action<bool> Toggled;

    private string foldoutName;

    [Export, TypedPath(typeof(CheckBox))] private NodePath checkBoxPath;
    [Export, TypedPath(typeof(Container))] private NodePath contentPath;

    private CheckBox _checkBox;
    private Container _contentContainer;

    public bool IsCollapsed
    {
        get => CheckBox.Pressed;
        set => CheckBox.Pressed = value;
    }

    public CheckBox CheckBox => _checkBox ??= GetNode<CheckBox>(checkBoxPath);
    public Container ContentContainer => _contentContainer ??= GetNode<Container>(contentPath);

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

    private void OnCollapsedToggled(bool toggled)
    {
        Toggled(toggled);
        ContentContainer.Visible = !toggled;
    }

    public void AddContent(Control content)
    {
        content.GetParent()?.RemoveChild(content);
        ContentContainer.AddChild(content);
    }
}

#endif