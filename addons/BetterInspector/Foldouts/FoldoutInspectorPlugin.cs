#if TOOLS
namespace BetterInspector.Editor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterInspector.Utilities;
using Godot;

public class FoldoutInspectorPlugin : EditorInspectorPlugin
{
    private Dictionary<Godot.Object, string> prevFoldoutNameFor = new();
    private Dictionary<Godot.Object, Dictionary<string, Foldout>> currentFoldoutsFor = new();

    public override bool CanHandle(Godot.Object @object) => true;

    public override void ParseBegin(Godot.Object @object)
    {
        prevFoldoutNameFor.AddOrSet(@object, "");
        currentFoldoutsFor.AddOrSet(@object, new());
    }

    public override bool ParseProperty(Godot.Object @object, int typeArg, string path, int hint, string hintText, int usage)
    {
        string propName = path.GetPropName();
        Foldout foldout = null;

        string foldoutName = CheckForFoldout(
            propName,
            @object.GetInEditorTypeCached(),
            out FoldoutPosition position,
            out bool isExpressionProperty,
            out bool dontSetPrevFoldout,
            out bool isLastEntry);

        if (!dontSetPrevFoldout)
            prevFoldoutNameFor[@object] = isLastEntry ? "" : foldoutName;

        if (foldoutName != "")
        {
            CreateFoldoutIfNeeded(foldoutName, position);
            foldout = currentFoldoutsFor[@object][foldoutName];
            foldout.properties.Add(path);
        }

        // Hide if the property is one starting with "_StartF_" or "_EndF_"
        if (isExpressionProperty)
            return true;

        if (foldout != null)
            AddCustomControl(new FoldoutContentAdder(foldout.container));

        return false;

        string CheckForFoldout(string propName, Type objType, out FoldoutPosition position, out bool isExpressionProperty, out bool dontSetPrevFoldout, out bool isLastEntry)
        {
            isExpressionProperty = false;
            dontSetPrevFoldout = false;
            isLastEntry = false;
            position = FoldoutPosition.Dynamic;

            if (propName.StartsWith("_StartF_"))
            {
                isExpressionProperty = true;

                string strippedName = propName.LStrip("_StartF_");

                position = strippedName.StartsWith("AtBottom_")
                    ? FoldoutPosition.Bottom
                    : (strippedName.StartsWith("AtTop_")
                        ? FoldoutPosition.Top
                        : FoldoutPosition.Dynamic);

                return strippedName
                        .LStrip("AtBottom_")
                        .LStrip("AtTop_")
                        .Capitalize();
            }

            if (propName.StartsWith("_EndF_"))
            {
                isExpressionProperty = true;
                return "";
            }

            FieldInfo field = objType.GetField(propName, Utilities.InstancePubAndNonPubBindingFlags);

            if (field != null)
            {
                var inFouldout = field.GetCustomAttribute<InFoldoutAttribute>();
                if (inFouldout != null)
                {
                    position = inFouldout.position;
                    dontSetPrevFoldout = true;
                    return inFouldout.foldoutName;
                }

                var fouldoutStart = field.GetCustomAttribute<StartFoldoutAttribute>();
                if (fouldoutStart != null)
                {
                    position = fouldoutStart.position;
                    return fouldoutStart.foldoutName;
                }

                var foldoutEnd = field.GetCustomAttribute<EndFoldoutAttribute>();
                if (foldoutEnd != null)
                {
                    isLastEntry = true;
                    return prevFoldoutNameFor[@object];
                }
            }

            PropertyInfo prop = objType.GetProperty(propName, Utilities.InstancePubAndNonPubBindingFlags);

            if (prop != null)
            {
                var inFouldout = prop.GetCustomAttribute<InFoldoutAttribute>();

                if (inFouldout != null)
                {
                    position = inFouldout.position;
                    dontSetPrevFoldout = true;
                    return inFouldout.foldoutName;
                }
            }

            return prevFoldoutNameFor[@object];
        }

        void CreateFoldoutIfNeeded(string foldoutName, FoldoutPosition position)
        {
            if (!currentFoldoutsFor[@object].ContainsKey(foldoutName))
            {
                currentFoldoutsFor[@object].Add(foldoutName, new Foldout(this, @object, true, foldoutName, position));
                currentFoldoutsFor[@object][foldoutName].Construct();
                return;
            }

            if (currentFoldoutsFor[@object][foldoutName].container == null)
            {
                currentFoldoutsFor[@object][foldoutName].Construct();
            }
        }
    }

    public override void ParseEnd()
    {
        Godot.Object @object = currentFoldoutsFor.Last().Key;

        // Move the foldout containers if there position is specified
        foreach (Foldout foldout in currentFoldoutsFor[@object].Values)
        {
            switch (foldout.position)
            {
                case FoldoutPosition.Top:
                    foldout.container.GetParent().MoveChild(foldout.container, 0);
                    break;

                case FoldoutPosition.Bottom:
                    foldout.container.Raise();
                    break;
            }
        }

        currentFoldoutsFor.Remove(@object);
        prevFoldoutNameFor.Remove(@object);
    }

    public void OnFoldoutToggled(Foldout sender, bool toggled)
    {
        sender.isCollapsed = toggled;
    }
}

#endif