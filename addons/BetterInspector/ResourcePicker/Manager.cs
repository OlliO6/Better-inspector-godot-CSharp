namespace BetterInspector.Editor.ResourcePicker;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Utilities;

[Tool]
public class Manager : Node
{
    public Plugin plugin;

    public Manager(Plugin plugin)
    {
        this.plugin = plugin;
    }
    public Manager() { }

    public override void _Ready()
    {
        GetTree().Connect("node_added", this, nameof(OnNodeAdded));
    }

    private void OnNodeAdded(Node node)
    {
        if (!(node.GetClass() == "EditorPropertyResource")) return;

        GD.Print("FOUND RESOURCE");

        var picker = node.GetChildren()
                .OfType<EditorResourcePicker>()
                .First();

        var popup = picker.GetChildren()
                .OfType<PopupMenu>()
                .First();

        if (!popup.IsConnected("about_to_show", this, nameof(OnPickerIsPoppingUp)))
            popup.Connect("about_to_show", this, nameof(OnPickerIsPoppingUp), new(node as EditorProperty, picker, popup));
    }

    private void OnPickerIsPoppingUp(EditorProperty propertyEditor, EditorResourcePicker picker, PopupMenu menu)
    {
        Type desiredType = propertyEditor.GetEditedObject().GetInEditorTypeCached()
                ?.GetField(propertyEditor.GetEditedProperty().GetPropName(), Utilities.InstancePubAndNonPubBindingFlags)?.FieldType;

        if (desiredType == null)
            desiredType = propertyEditor.GetEditedObject().GetInEditorTypeCached()
                    ?.GetProperty(propertyEditor.GetEditedProperty().GetPropName(), Utilities.InstancePubAndNonPubBindingFlags)?.PropertyType;

        if (desiredType == null) return;

        if (ClassDB.ClassExists(desiredType.Name) && ClassDB.CanInstance(desiredType.Name))
        {
            propertyEditor.EmitChanged(propertyEditor.GetEditedProperty(),
                ClassDB.Instance(desiredType.Name));
            GD.Print("Instanced from ClassDB");
            return;
        }

        ResourceScriptPathAttribute resourceScriptPathAttribute = desiredType.GetCustomAttribute<ResourceScriptPathAttribute>();

        if (resourceScriptPathAttribute != null)
        {
            Resource res = new Resource();
            res.SetScript(GD.Load<Script>(resourceScriptPathAttribute.path));
            res.ResourceName = desiredType.Name;
            propertyEditor.EmitChanged(propertyEditor.GetEditedProperty(), res);
            GD.Print("Instanced from script");
            return;
        }


        GD.Print("Can't instance type: ", desiredType.FullName);
    }
}
