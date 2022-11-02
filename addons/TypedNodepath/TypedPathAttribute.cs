namespace BetterInspector;

using System;
using System.Diagnostics;

[AttributeUsage(AttributeTargets.Field), Conditional("TOOLS")]
public sealed class TypedPathAttribute : Attribute
{
    public readonly Type type;

    public TypedPathAttribute(Type type)
    {
        this.type = type;
    }
}