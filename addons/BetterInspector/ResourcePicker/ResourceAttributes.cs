namespace BetterInspector;

using System;
using Godot;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
sealed class ResourceAttribute : Attribute { }


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
sealed class ResourceScriptPathAttribute : Attribute
{
    public readonly string path;
    public ResourceScriptPathAttribute(string path) => this.path = path;
}
