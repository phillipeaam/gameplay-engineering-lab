# Locomotion

Locomotion coordinates movement state, jump rules, and the Animator. It does
not define how movement requests are produced; it receives state changes and
translates their results into the locomotion animation layer.

## Structure

```text
Locomotion/
├── Animation/
└── Gameplay/
```

The layer is compiled by `RPGTinyHeroDuo.Locomotion.asmdef`.

### Animation

`ILocomotionAnimator` defines the visual contract:

```text
ApplyMovement(Vector2)  → MoveX and MoveY
SetGrounded(bool)       → IsGrounded
RequestJump()           → JumpRequest
```

`LocomotionAnimatorAdapter` implements this contract for Unity's `Animator` and
owns the cached parameter hashes.

### Gameplay

`LocomotionController` coordinates movement state changes. It forwards
movement and grounded state to the Animator and asks `IJumpHandler` whether a
jump is allowed before requesting its animation.

`IJumpHandler` is the gameplay boundary for jump rules such as grounded checks,
air jumps, stamina, and control restrictions.

## Module flow

```text
Movement state changes
    ↓
LocomotionController
    ├── IJumpHandler      → validates jump rules
    └── ILocomotionAnimator → updates visual parameters
```

The Animator adapter only represents the result. It does not decide whether a
character can move or jump.
