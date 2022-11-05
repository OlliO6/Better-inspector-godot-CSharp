namespace BetterInspector.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public static class Utilities
{
    public static Dictionary<Godot.Object, Type> objectTypeCache = new();

    public static Type GetInEditorTypeCached(this Godot.Object obj)
    {
        if (objectTypeCache.ContainsKey(obj))
            return objectTypeCache[obj];

        // So the dictionary doesn't gets to large when for example switching scenes
        if (obj is Node node)
        {
            node.Connect(
                "tree_exited",
                new ConnectionHelper { OnFired = () => objectTypeCache.Remove(obj) },
                "Fire");
        }

        Type type = obj.GetInEditorType();
        objectTypeCache.Add(obj, type);
        return type;
    }

    public static Type GetInEditorType(this Godot.Object obj)
    {
        Type type = obj.GetType();

        if (type.GetCustomAttribute(typeof(ToolAttribute)) != null)
            return type;

        CSharpScript script = obj.GetScript() as CSharpScript;

        if (script is null) return type;

        return script.GetScriptType();
    }

    public static Type GetScriptType(this CSharpScript script) => GetTypeFromSource(GetRealSourceCode(script));

    public static Type GetTypeFromSource(string source) => Type.GetType(GetTypeName(source));

    // Because source code property doesn't updates sometimes
    public static string GetRealSourceCode(CSharpScript script)
    {
        File file = new();
        file.Open(script.ResourcePath, File.ModeFlags.Read);
        string text = file.GetAsText();
        file.Close();

        return text;
    }

    public static string GetTypeName(string source)
    {
        const string nspace = "namespace";

        int nameStartIdx = source.IndexOf(" class ") + 7;
        string name = source.Substring(nameStartIdx, source.IndexOf(' ', nameStartIdx) - nameStartIdx);

        if (!source.Contains(nspace)) return name;

        int startIdx = source.IndexOf(nspace) + nspace.Length + 1;
        string nspaceName = source.Substring(startIdx, source.IndexOf(';', startIdx) - startIdx);

        return $"{nspaceName}.{name}";
    }

    public static T WitchChilds<T>(this T from, params Node[] childs) where T : Node
    {
        foreach (var child in childs)
            from.AddChild(child);

        return from;
    }

    public static T Setted<T>(this T from, string property, object value) where T : Node
    {
        from.Set(property, value);
        return from;
    }

    public static T DefferedSetted<T>(this T from, string property, object value) where T : Node
    {
        from.SetDeferred(property, value);
        return from;
    }

    public static T Connected<T>(this T from, string signal, Godot.Object to, string method) where T : Node
    {
        from.Connect(signal, to, method);
        return from;
    }

    public static bool IsEmptyOrNull(this NodePath path) => path == null || path.IsEmpty();


    public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, value);
            return;
        }

        dict[key] = value;
    }

    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default)
    {
        if (dict.ContainsKey(key))
            return dict[key];

        return @default;
    }

    [Tool]
    private class ConnectionHelper : Godot.Object
    {
        public Action OnFired;

        public void Fire()
        {
            OnFired?.Invoke();
            CallDeferred("free");
        }
    }
}