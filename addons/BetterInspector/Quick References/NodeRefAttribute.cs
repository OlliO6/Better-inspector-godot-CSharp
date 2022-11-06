namespace BetterInspector;

using System;

[AttributeUsage(AttributeTargets.Field)]
public class NodeRefAttribute : Attribute
{
    public string foldout = "References";
}
