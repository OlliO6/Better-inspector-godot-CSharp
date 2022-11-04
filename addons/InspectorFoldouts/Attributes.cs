namespace BetterInspector;

using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Field), Conditional("TOOLS")]
public sealed class StartFoldoutAttribute : Attribute
{
    public readonly string foldoutName;

    public StartFoldoutAttribute(string foldoutName)
    {
        this.foldoutName = foldoutName;
    }
}

[AttributeUsage(AttributeTargets.Field), Conditional("TOOLS")]
public sealed class EndFoldoutAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property), Conditional("TOOLS")]
public sealed class InFoldoutAttribute : Attribute
{
    public readonly string foldoutName;

    public InFoldoutAttribute(string foldoutName)
    {
        this.foldoutName = foldoutName;
    }
}