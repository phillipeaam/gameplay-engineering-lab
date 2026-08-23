# Full Body Actions

## Purpose

Full Body Actions exposes the animation contract for actions that take over the
complete character body. It maps the selected action and its playback mode to
Unity Animator parameters.

It does not define gameplay rules or how actions are requested.

## Structure

```text
FullBodyActions/
├── FullBodyAction.cs
├── FullBodyActionAnimation.cs
├── FullBodyActionAnimatorAdapter.cs
├── IFullBodyActionAnimator.cs
├── RPGTinyHeroDuo.FullBodyActions.asmdef
└── README.md
```

## Animator parameters

```text
FullBodyActionIndex   : Int
FullBodyActionActive  : Bool
FullBodyActionRequest : Trigger
```

## Contract

`IFullBodyActionAnimator` exposes the visual operations:

```text
PlayOnce(action)  → Active = false and Request
PlayLoop(action)  → Active = true and Request
Reset()           → Index = None and Active = false
```

`FullBodyActionAnimatorAdapter` implements the contract for Unity's `Animator`
and owns the cached parameter hashes.

`FullBodyAction` defines the stable action indexes:

```text
0 = None
1 = Fall
2 = Stun
3 = Hit Reaction
4 = Death
```

## Ownership

The adapter only maps supplied action data to Animator parameters. Action
authorization, duration, interruption, and input rules belong to the feature
that consumes this module.

The Animator Controller owns the transitions. One-shot actions should use exit
time, while looped actions should leave when gameplay calls `Reset()`.

## Assembly

The module is compiled by `RPGTinyHeroDuo.FullBodyActions.asmdef`.
