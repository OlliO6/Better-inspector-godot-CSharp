using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class TypedPathAttribute : Attribute
{
    public readonly Type type;

    public TypedPathAttribute(Type type)
    {
        this.type = type;
    }
}