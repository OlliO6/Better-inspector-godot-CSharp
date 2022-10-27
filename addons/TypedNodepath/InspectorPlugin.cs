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
        cachedType = Utilities.GetInEditorTypeOf(@object);
    }

    public override bool ParseProperty(Godot.Object @object, int typeArg, string path, int hint, string hintText, int usage)
    {
        string propName = path.GetFile();

        Type type = cachedType?.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.FieldType;

        if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NodePath<>))
        {
            // Make TypedPathPropertyEditor instance with correct type
            AddPropertyEditor(
                property: path,
                editor: (EditorProperty)Activator
                        .CreateInstance(typeof(TypedPathPropertyEditor<>)
                                .MakeGenericType(type.GetGenericArguments()[0])));

            return true;
        }

        return false;
    }
}

#endif