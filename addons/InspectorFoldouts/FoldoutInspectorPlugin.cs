#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
// using Godot.Collections;

[Tool]
public class FoldoutInspectorPlugin : EditorInspectorPlugin
{
    private Dictionary<Godot.Object, string> prevFoldoutNameFor = new();
    private Dictionary<Godot.Object, Dictionary<string, Foldout>> currentFoldoutsFor = new();

    public override bool CanHandle(Godot.Object @object) => true;

    public override void ParseBegin(Godot.Object @object)
    {
        prevFoldoutNameFor.AddOrSet(@object, "");

        Dictionary<string, Foldout> prevFoldouts = currentFoldoutsFor.GetOrDefault(@object, null);

        currentFoldoutsFor.AddOrSet(@object, new());

        if (prevFoldouts == null) return;

        foreach (string item in prevFoldouts.Keys)
        {
            currentFoldoutsFor[@object].Add(item, new Foldout(
                this, @object,
                prevFoldouts[item].isCollapsed,
                prevFoldouts[item].name));
        }
    }

    public override bool ParseProperty(Godot.Object @object, int typeArg, string path, int hint, string hintText, int usage)
    {
        string propName = path.GetFile();
        Foldout foldout = null;
        string foldoutName = CheckForFoldout(propName, out bool isExpressionProperty);
        prevFoldoutNameFor[@object] = foldoutName;

        // Hide if the property is one starting with "_StartF_" or "_EndF_"
        if (isExpressionProperty)
            return true;

        if (foldoutName != "")
        {
            CreateFoldoutIfNeeded(foldoutName);
            foldout = currentFoldoutsFor[@object][foldoutName];
            foldout.properties.Add(path);
        }

        if (foldout != null)
        {
            AddCustomControl(new FoldoutContentAdder(foldout.container));
        }
        GD.Print(path);

        return false;

        string CheckForFoldout(string propName, out bool isExpressionProperty)
        {
            if (propName.StartsWith("_StartF_"))
            {
                isExpressionProperty = true;
                return propName.LStrip("_StartF_").Capitalize();
            }

            if (propName.StartsWith("_EndF_"))
            {
                isExpressionProperty = true;
                return "";
            }

            isExpressionProperty = false;
            return prevFoldoutNameFor[@object];
        }

        void CreateFoldoutIfNeeded(string foldoutName)
        {
            if (!currentFoldoutsFor[@object].ContainsKey(foldoutName))
            {
                currentFoldoutsFor[@object].Add(foldoutName, new Foldout(this, @object, true, foldoutName));
                currentFoldoutsFor[@object][foldoutName].Construct();
                return;
            }

            if (currentFoldoutsFor[@object][foldoutName].container == null)
            {
                currentFoldoutsFor[@object][foldoutName].Construct();
            }
        }
    }

    public void OnFoldoutToggled(Foldout sender, bool toggled)
    {
        sender.isCollapsed = toggled;
    }
}

#endif