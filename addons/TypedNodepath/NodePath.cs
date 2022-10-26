namespace TypedNodePaths;

using System;
using Godot;

public class NodePath<T> : Resource
    where T : class
{
    public NodePath path;

    public NodePath(NodePath path)
    {
        this.path = path;
    }
}