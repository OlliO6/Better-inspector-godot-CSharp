#if TOOLS
namespace TypedNodePaths;

using System;
using System.Reflection;
using Godot;

[Tool]
public class InspectorPlugin : EditorInspectorPlugin
{
    public PackedScene propertyEditorScene = GD.Load<PackedScene>("res://addons/TypedNodepath/PropertyEditor/PropertyEditor.tscn");

    private System.Type cachedType = null;

    public override bool CanHandle(Godot.Object @object) => true;

    public override void ParseBegin(Godot.Object @object)
    {
        cachedType = Utilities.GetInEditorTypeOf(@object);
    }

    public override bool ParseProperty(Godot.Object @object, int typeArg, string path, int hint, string hintText, int usage)
    {
        string propName = path.GetFile();

        Type type = cachedType?.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.FieldType;

        if (type != null) GD.Print("Type: ", type, " :: ", typeof(TypedNodePaths.NodePath<>));

        if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(TypedNodePaths.NodePath<>))
        {
            AddPropertyEditor(path, new TypedPathPropertyEditor(type));
            return true;
        }

        return false;
    }
}

#endif