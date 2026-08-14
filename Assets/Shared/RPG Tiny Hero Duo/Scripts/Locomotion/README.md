# Locomotion

Locomotion connects player input, movement rules, and the Animator. It does not
define the character's visual states; it sends the relevant values to the
locomotion layer.

## Structure

```text
Locomotion/
├── Animation/
├── Input/
└── Gameplay/
```

### Animation

`ILocomotionAnimator` defines the visual contract:

```text
ApplyMovement(Vector2)  → MoveX and MoveY
SetGrounded(bool)       → IsGrounded
RequestJump()           → JumpRequest
```

`LocomotionAnimatorAdapter` implements this contract for Unity's `Animator` and
owns the cached parameter hashes.

### Input

`LocomotionInputEvents` translates Unity Input System callbacks into intentions:

```text
Move performed/canceled → MovementChanged(Vector2)
Jump performed          → JumpRequested
```

It does not know gameplay rules or the Animator. Call `Enable()` and
`Disable()` with the owning feature's lifecycle.

### Gameplay

`LocomotionController` coordinates the received intentions. It forwards
movement and grounded state to the Animator and asks `IJumpHandler` whether a
jump is allowed before requesting its animation.

`IJumpHandler` is the gameplay boundary for jump rules such as grounded checks,
air jumps, stamina, and control restrictions.

## Data flow

```text
Input System
    ↓
LocomotionInputEvents
    ↓ events
LocomotionController
    ├── IJumpHandler      → validates jump rules
    └── ILocomotionAnimator → updates visual parameters
```

The Animator adapter only represents the result. It does not decide whether a
character can move or jump.
