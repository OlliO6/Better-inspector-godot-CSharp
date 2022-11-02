#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

[Tool]
public class FoldoutInspectorPlugin : EditorInspectorPlugin
{
    private Type cachedType = null;
    private string prevFoldoutName;
    private Dictionary<string, Foldout> currentFoldouts;

    public override bool CanHandle(Godot.Object @object) => true;

    public override void ParseBegin(Godot.Object @object)
    {
        cachedType = @object.GetInEditorType();
        prevFoldoutName = "";

        Dictionary<string, Foldout> prevFoldouts = currentFoldouts;
        currentFoldouts = new();

        if (prevFoldouts == null) return;

        foreach (string item in prevFoldouts.Keys)
        {
            currentFoldouts.Add(item, new Foldout(
                this, @object,
                prevFoldouts[item].isCollapsed,
                prevFoldouts[item].name));
        }
    }

    public override bool ParseProperty(Godot.Object @object, int typeArg, string path, int hint, string hintText, int usage)
    {
        string propName = path.GetFile();
        FieldInfo field = cachedType?.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        Foldout foldout = null;
        string foldoutName = CheckForFoldout(propName);

        prevFoldoutName = foldoutName;

        if (foldoutName != "")
        {
            CreateFoldoutIfNeeded(foldoutName);
            foldout = currentFoldouts[foldoutName];

            foldout.properties.Add(path);
        }

        if (foldout != null && foldout.isCollapsed)
            return true;

        return false;

        string CheckForFoldout(string propName)
        {
            if (propName.StartsWith("_StartF_"))
                return propName.LStrip("_StartF_").Capitalize();

            if (propName.StartsWith("_EndF_"))
                return "";

            return prevFoldoutName;
        }

        void CreateFoldoutIfNeeded(string foldoutName)
        {
            if (!currentFoldouts.ContainsKey(foldoutName))
            {
                currentFoldouts.Add(foldoutName, new Foldout(this, @object, true, foldoutName));
                currentFoldouts[foldoutName].Construct();
                return;
            }

            if (currentFoldouts[foldoutName].container == null)
            {
                currentFoldouts[foldoutName].Construct();
            }
        }
    }

    public override void ParseEnd()
    {
        foreach (var item in currentFoldouts)
        {
            GD.Print($"{item.Key}/{item.Value.name}: \n    Collapsed:{item.Value.isCollapsed}\n    Properties: {{\n{new Godot.Collections.Array(item.Value.properties)}\n}}");
        }
    }

    public void OnFoldoutToggled(Foldout sender, bool toggled)
    {
        sender.isCollapsed = toggled;
        // Redraw inspector for the object where the foldout was toggled
        sender.forObj.PropertyListChangedNotify();
    }
}

#endif