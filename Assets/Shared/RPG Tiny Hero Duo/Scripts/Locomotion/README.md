# Locomotion

## Purpose

Locomotion exposes the animation contract for movement state. It maps supplied
movement, grounded, and jump values to Unity Animator parameters.

It does not define movement rules or how state changes are produced.

## Structure

```text
Locomotion/
├── ILocomotionAnimator.cs
├── LocomotionAnimatorAdapter.cs
├── RPGTinyHeroDuo.Locomotion.asmdef
└── README.md
```

## Animator parameters

```text
MoveX       : Float
MoveY       : Float
IsGrounded  : Bool
JumpRequest : Trigger
```

## Contract

`ILocomotionAnimator` exposes the visual operations:

```text
ApplyMovement(Vector2)  → MoveX and MoveY
SetGrounded(bool)       → IsGrounded
RequestJump()           → JumpRequest
```

`LocomotionAnimatorAdapter` implements the contract for Unity's `Animator` and
owns the cached parameter hashes.

## Ownership

The adapter only maps supplied state to Animator parameters. Movement, grounding,
jump, and input rules belong to the feature that consumes this module.

## Assembly

The module is compiled by `RPGTinyHeroDuo.Locomotion.asmdef`.
