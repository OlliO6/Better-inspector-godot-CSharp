Better inspector godot
=================
> These plugins are mainly for c#


Typed node paths
------------------------------

If you have an exported node path and you want it to point to a specific type of node you can use the `TypedPath(Type)` attribute.
```c#
[Export, TypedPath(typeof(AnimationPlayer))]
private NodePath animPath;
```
You can even use unusual types like interfaces.

It will look like thhis in the inspector.

![Alt text](Screenshots/TypedPaths.gif)

An extra feature of this is the select button. Click on it and the node where the path is pointing to will get selected.

<img src="Screenshots/PathSelect.gif" width="350" height="200" />

Quick node refernces
---------------------------

Ita really annoying to write this kind of stuff all the time.

```c#
[Export, TypedPath(typeof(AnimationPlayer))]
private NodePath _animPlayer;

public AnimationPlayer animPlayer;

public override void _Ready()
{
    animPlayer = GetNode<AnimationPlayer>(_animPlayer);
}
```
So i implemented a source code generator that does that stuff automatically for you.\
With it you can just do this instead.

```c#
[NodeRef] public AnimationPlayer animPlayer;
```

This will export a typed node path with the type of the field and assign the field on ready.
To avoid having a cluddered inspector it will put those 
node paths in a foldout called 'References' unless you don't set 'InFoldout' to false.
```c#
[Reference(foldout = "")]
```

Foldouts
--------------

Instead