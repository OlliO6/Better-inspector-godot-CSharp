#if TOOLS
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
        string propName = propertyEditor.GetEditedProperty().GetPropName();

        Type desiredType = propertyEditor.GetEditedObject().GetInEditorTypeCached().GetSeenField(propName)?.FieldType;

        if (desiredType == null)
            desiredType = propertyEditor.GetEditedObject().GetInEditorTypeCached().GetSeenProperty(propName)?.PropertyType;

        if (desiredType == null) return;

        RecreateMenu(propertyEditor, menu, desiredType);

        // Give it the right position and size
        Vector2 rightTop = menu.RectPosition + Vector2.Right * menu.RectSize.x;
        menu.RectSize = new(0, 0);
        menu.RectPosition = rightTop + Vector2.Left * menu.RectSize.x;

        if (!menu.IsConnected("index_pressed", this, nameof(OnResPickerItemPressed)))
            menu.Connect("index_pressed", this, nameof(OnResPickerItemPressed), new(menu, propertyEditor));


        Texture GetIconFrom(Type type)
        {
            Texture icon = null;

            while (icon == null && type != null)
            {
                icon = Plugin.GetIcon(type.Name);
                type = type.BaseType;
            }

            return icon;
        }
        Texture GetIconFromClassDB(string @class)
        {
            Texture icon = null;

            while (icon == null && ClassDB.ClassExists(@class))
            {
                icon = Plugin.GetIcon(@class);
                @class = ClassDB.GetParentClass(@class);
            }

            return icon;
        }

        void RecreateMenu(EditorProperty propertyEditor, PopupMenu menu, Type desiredType)
        {
            bool copyShown = menu.GetItemIndex(6) != -1;
            bool pasteShown = menu.GetItemIndex(7) != -1;

            menu.Clear();
            AddNewIcons(propertyEditor, menu, desiredType);

            menu.AddItem("");
            menu.SetItemAsSeparator(menu.GetItemCount() - 1, true);

            menu.AddIconItem(Plugin.GetIcon("Load"), "Quick Load", 1);
            menu.AddIconItem(Plugin.GetIcon("Load"), "Load", 0);

            if (propertyEditor.GetEditedObject().Get(propertyEditor.GetEditedProperty()) != null)
            {
                menu.AddIconItem(Plugin.GetIcon("Edit"), "Edit", 2);
                menu.AddIconItem(Plugin.GetIcon("Clear"), "Clear", 3);
                menu.AddIconItem(Plugin.GetIcon("Duplicate"), "Make Unique", 4);
                menu.AddIconItem(Plugin.GetIcon("Save"), "Save", 5);
            }
            if (copyShown || pasteShown)
            {
                menu.AddItem("");
                menu.SetItemAsSeparator(menu.GetItemCount() - 1, true);
            }
            if (copyShown) menu.AddIconItem(Plugin.GetIcon("ActionCopy"), "Copy", 6);
            if (pasteShown) menu.AddIconItem(Plugin.GetIcon("ActionPaste"), "Paste", 7);

            void AddNewIcons(EditorProperty propertyEditor, PopupMenu menu, Type desiredType)
            {
                foreach (var type in GetType().Assembly.DefinedTypes
                                    .Where(type => desiredType.IsAssignableFrom(type)))
                {
                    var pathAttribute = type.GetCustomAttribute<ResourceScriptPathAttribute>();

                    if (pathAttribute == null) continue;

                    propertyEditor.SetMeta(menu.GetItemCount().ToString(), pathAttribute.path);
                    menu.AddIconItem(GetIconFrom(type), "New " + type.Name, 100);
                }

                if (ClassDB.ClassExists(desiredType.Name))
                {
                    string className = desiredType.Name;

                    if (ClassDB.CanInstance(className))
                    {
                        propertyEditor.SetMeta(menu.GetItemCount().ToString(), "#ClassDB");
                        menu.AddIconItem(GetIconFromClassDB(className), "New " + className, 100);
                    }

                    foreach (string @class in ClassDB.GetInheritersFromClass(className)
                            .Where(@class => ClassDB.CanInstance(@class)))
                    {
                        propertyEditor.SetMeta(menu.GetItemCount().ToString(), "#ClassDB");
                        menu.AddIconItem(GetIconFromClassDB(@class), "New " + @class, 100);
                    }
                }
            }
        }
    }

    private void OnResPickerItemPressed(int idx, PopupMenu menu, EditorProperty property)
    {
        if (menu.GetItemId(idx) != 100)
            return;

        string typeName = menu.GetItemText(idx).LStrip("New ");
        string metaString = (string)property.GetMeta(idx.ToString());

        if (metaString == "#ClassDB")
        {
            property.EmitChanged(property.GetEditedProperty(),
                ClassDB.Instance(typeName));
            return;
        }

        if (!metaString.IsAbsPath()) return;

        Resource res = new Resource();
        res.SetScript(GD.Load<Script>(metaString));
        res.ResourceName = typeName;
        property.EmitChanged(property.GetEditedProperty(), res);
    }
}

#endif