# Playground Locomotion

## Purpose

This module coordinates player input, movement through Unity's
`CharacterController`, grounded transitions, jumping, landing recovery, and the
animation contracts exposed by the shared Locomotion assembly.

`LocomotionModule` is the composition and frame-orchestration boundary. The
movement, jump, landing, grounding, and input types retain their own policies
and state instead of turning the module into a single locomotion controller.

## Settings boundaries

Configuration is divided by consumer and reason to change:

- `IMovementSettings` supplies movement and rotation speeds to
  `CharacterControllerMotor`.
- `IJumpSettings` supplies jump height and double-jump policy to
  `JumpController`.
- `ILandingSettings` supplies recovery duration and movement restriction to
  `LandingRecovery`.

The composition root may pass one object that implements all three contracts,
as the playground controller does, but consumers receive only the contract they
need. This keeps feature dependencies narrow without requiring three separate
Unity configuration assets.

## State ownership and collaboration

`LocomotionModule` owns the `LandingRecovery` instance and advances it once per
tick. `LandingRecovery` exclusively owns the recovery timer and the movement
multiplier calculation. When a landing begins, it validates and snapshots the
duration and initial multiplier so one recovery remains internally consistent;
later setting changes apply to the next landing.

Landing recovery affects jumping, but `JumpController` receives only the
decision it needs through `isJumpBlocked`. It does not receive
`LandingRecovery`, inspect its timer, or depend on landing settings. The
locomotion orchestrator remains responsible for translating one subsystem's
state into another subsystem's input.

Movement and jump animation are likewise separate contracts. A single adapter
may implement both `IMovementAnimator` and `IJumpAnimator`, while each consumer
depends only on its relevant visual capability.

## Alternatives and trade-offs

- **One locomotion settings interface:** simpler constructor wiring, but every
  consumer would depend on unrelated settings and independent policies would be
  harder to substitute or test. Narrow contracts were chosen; a single concrete
  object can still implement all of them when convenient.
- **Keep landing recovery state in `LocomotionModule`:** reduces the type count,
  but mixes timing and interpolation policy into orchestration. A dedicated
  owner makes the recovery lifecycle and multiplier invariant explicit.
- **Pass `LandingRecovery` into `JumpController`:** permits the jump controller
  to query landing state directly, but couples jumping to a concrete sibling
  subsystem. Passing `isJumpBlocked` preserves explicit data flow and keeps the
  jump policy reusable; the trade-off is that the orchestrator must compute and
  pass that decision.

## When to revisit

If landing restrictions become a broader set of composable gameplay conditions,
`isJumpBlocked` may evolve into a policy-level jump permission contract; the
concrete recovery object should still remain outside `JumpController`.
