#if TOOLS
namespace BetterInspector.Editor;

using System.Collections.Generic;
using Godot;

[Tool]
public class Foldout : Godot.Object
{
    static readonly PackedScene containerScene = GD.Load<PackedScene>("addons/BetterInspector/Foldouts/Foldout.tscn");

    public FoldoutPosition position;
    public FoldoutContainer container;
    public bool isCollapsed;
    public readonly string name;
    public readonly Godot.Object forObj;
    public readonly FoldoutInspectorPlugin fouldoutInspector;
    public List<string> properties = new();

    private Foldout() { }

    public Foldout(FoldoutInspectorPlugin fouldoutInspector, Godot.Object forObj, bool isCollapsed, string name, FoldoutPosition position)
    {
        this.fouldoutInspector = fouldoutInspector;
        this.isCollapsed = isCollapsed;
        this.name = name;
        this.forObj = forObj;
        this.position = position;
    }

    // Should only be called during parse ParseProperty
    public void Construct()
    {
        container = containerScene.Instance<FoldoutContainer>();
        fouldoutInspector.AddCustomControl(container);
        container.IsCollapsed = isCollapsed;
        container.FoldoutName = name;
        container.Toggled += OnToggled;
    }

    private void OnToggled(bool toggled) => fouldoutInspector.OnFoldoutToggled(this, toggled);
}

#endif