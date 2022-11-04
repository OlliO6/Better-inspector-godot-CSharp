#if TOOLS
namespace BetterInspector.InspectorFoldout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
// using Godot.Collections;

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

        string foldoutName = CheckForFoldout(
            propName,
            @object.GetInEditorTypeCached(),
            out bool isExpressionProperty,
            out bool dontSetPrevFoldout,
            out bool isLastEntry);

        if (!dontSetPrevFoldout)
            prevFoldoutNameFor[@object] = isLastEntry ? "" : foldoutName;

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

        return false;

        string CheckForFoldout(string propName, Type objType, out bool isExpressionProperty, out bool dontSetPrevFoldout, out bool isLastEntry)
        {
            isExpressionProperty = false;
            dontSetPrevFoldout = false;
            isLastEntry = false;

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

            FieldInfo field = objType.GetField(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null)
            {
                var inFouldout = field.GetCustomAttribute<InFoldoutAttribute>();
                if (inFouldout != null)
                {
                    dontSetPrevFoldout = true;
                    return inFouldout.foldoutName;
                }

                var fouldoutStart = field.GetCustomAttribute<StartFoldoutAttribute>();
                if (fouldoutStart != null)
                    return fouldoutStart.foldoutName;

                var foldoutEnd = field.GetCustomAttribute<EndFoldoutAttribute>();
                if (foldoutEnd != null)
                {
                    isLastEntry = true;
                    return prevFoldoutNameFor[@object];
                }
            }

            PropertyInfo prop = objType.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop != null)
            {
                var inFouldout = prop.GetCustomAttribute<InFoldoutAttribute>();

                if (inFouldout != null)
                {
                    dontSetPrevFoldout = true;
                    return inFouldout.foldoutName;
                }
            }

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