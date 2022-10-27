namespace TypedNodePaths;

using System;
using System.Reflection;
using Godot;

public static class Utilities
{
    public static Type GetInEditorTypeOf(Godot.Object node)
    {
        Type type = node.GetType();

        if (type.GetCustomAttribute(typeof(ToolAttribute)) != null)
            return type;

        CSharpScript script = node.GetScript() as CSharpScript;

        if (script is null) return type;

        return script.GetScriptType();
    }

    public static Type GetScriptType(this CSharpScript script) => GetTypeFromSource(GetRealSourceCode(script));

    public static Type GetTypeFromSource(string source) => Type.GetType(GetTypeName(source));

    // Becaause source code property doesn't updates sometimes
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
}