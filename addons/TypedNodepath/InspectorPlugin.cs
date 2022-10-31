#if TOOLS
namespace TypedNodePaths;

using System;
using System.Reflection;
using Godot;

[Tool]
public class InspectorPlugin : EditorInspectorPlugin
{
    private Type cachedType = null;

    public override bool CanHandle(Godot.Object @object) => true;

    public override void ParseBegin(Godot.Object @object)
    {
        cachedType = @object.GetInEditorType();
    }

    public override bool ParseProperty(Godot.Object @object, int typeArg, string path, int hint, string hintText, int usage)
    {
        string propName = path.GetFile();

        FieldInfo field = cachedType?.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (field == null || field.FieldType != typeof(NodePath)) return false;

        var attribute = field.GetCustomAttribute<TypedPathAttribute>();

        if (attribute == null) return false;

        // Make TypedPathPropertyEditor instance with correct type
        AddPropertyEditor(
            property: path,
            editor: new TypedPathPropertyEditor(attribute.type));

        return true;
    }
}

#endif