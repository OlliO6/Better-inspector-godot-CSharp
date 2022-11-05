#if TOOLS
namespace BetterInspector.Editor;

using System;
using Godot;

[Tool]
public class FoldoutContainer : VBoxContainer
{
    public event Action<bool> Toggled;

    private string foldoutName;
    private bool collapsed;

    [Export, TypedPath(typeof(Button))] private NodePath foldButtonPath;
    [Export, TypedPath(typeof(Container))] private NodePath contentPath;

    private Button _foldButton;
    private Container _contentContainer;

    public bool IsCollapsed
    {
        get => collapsed;
        set
        {
            collapsed = value;

            OnCollapsedToggled(value);
        }
    }

    public Button FoldButton => _foldButton ??= GetNode<Button>(foldButtonPath);
    public Container ContentContainer => _contentContainer ??= GetNode<Container>(contentPath);

    public string FoldoutName
    {
        get => foldoutName;
        set
        {
            foldoutName = value;

            if (FoldButton != null)
                FoldButton.Text = value;
        }
    }

    public override void _Ready()
    {
        FoldButton.Text = FoldoutName;
        FoldButton.Connect("pressed", this, nameof(Toggle));

        OnCollapsedToggled(IsCollapsed);
    }

    private void Toggle()
    {
        IsCollapsed = !IsCollapsed;
    }

    private void OnCollapsedToggled(bool toggled)
    {
        Toggled?.Invoke(toggled);
        ContentContainer.Visible = !toggled;

        FoldButton.Icon = Plugin.GetIcon(toggled ? "GuiTreeArrowRight" : "GuiTreeArrowDown");
    }

    public void AddContent(Control content)
    {
        content.GetParent()?.RemoveChild(content);
        ContentContainer.AddChild(content);
    }
}

#endif