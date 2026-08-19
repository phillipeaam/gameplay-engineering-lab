# Unity Lifecycle and Assets Standard

## Contents

- [Lifecycle and ownership](#1-lifecycle-and-ownership)
- [Serialization and Inspector configuration](#2-serialization-and-inspector-configuration)
- [ScriptableObjects and runtime assets](#3-scriptableobjects-and-runtime-assets)
- [Frame and physics behavior](#4-frame-and-physics-behavior)
- [Authored assets and references](#5-authored-assets-and-references)

## 1. Lifecycle and ownership

- Select `Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, `OnDisable`, and
  `OnDestroy` according to the dependency and lifetime being managed; do not
  rely on incidental cross-object callback order.
- Initialization and teardown SHOULD be idempotent where repeated enable/disable
  cycles are possible.
- Subscribe or start owned work at an explicit activation boundary and undo it
  at the matching deactivation/destruction boundary.
- Do not use `MonoBehaviour` constructors for Unity-managed initialization.
- Static state MUST reset correctly when domain reload behavior changes and MUST
  not leak between tests or play sessions.
- Ownership of instantiated `GameObject`, native allocation, subscription,
  token source, and disposable resource MUST include a cleanup path.

## 2. Serialization and Inspector configuration

- Prefer private `[SerializeField]` fields for Inspector configuration when
  external mutation is not part of the runtime contract.
- Validate the complete serialized configuration at a suitable composition or
  lifecycle boundary and identify the invalid field.
- Serialized numeric configuration used by gameplay calculations MUST validate
  finitude and its domain-specific range at runtime. Inspector constraints such
  as `[Min]` and `[Range]` improve authoring feedback but are not runtime
  guarantees.
- Use Unity's overloaded `== null` semantics when checking whether a
  `UnityEngine.Object` is usable. Use `ReferenceEquals` only for deliberate CLR
  reference identity checks.
- Properties are not a substitute for serialized fields. Observe Unity's
  supported field types and container limitations.
- Use `[SerializeReference]` only when polymorphism, shared references, null
  preservation, or cycles genuinely require managed-reference semantics.
- `OnValidate` MAY improve authoring feedback but MUST NOT be the only runtime
  guarantee for required configuration.
- Renaming or changing serialized fields MUST preserve data with supported
  migration mechanisms or explicitly accept data loss.

## 3. ScriptableObjects and runtime assets

- Prefer `ScriptableObject` assets for intentionally shared authored
  configuration.
- Do not store transient session state in project assets unless persistence is
  explicit and tested.
- Runtime mutation of shared assets MUST be deliberate; clone per-session state
  when isolation is required.

## 4. Frame and physics behavior

- Use the correct time source and callback for frame-based or physics-based
  behavior.
- Frame-rate-dependent logic MUST account for elapsed time unless it is
  intentionally frame-count based.
- Physics state changes SHOULD occur through Unity's supported physics timing
  and APIs.
- Calculated vectors, velocities, rotations, and time values MUST be finite and
  within their required domain before they are passed to physics, transform, or
  animation APIs that cannot safely recover from invalid numeric state.
- Avoid repeated scene searches, component lookups, string-based dispatch, and
  avoidable allocation inside demonstrated hot paths.

## 5. Authored assets and references

- Preserve `.meta` files and GUID continuity when moving or renaming assets.
- Asset moves SHOULD occur through Unity-aware tooling when reference integrity
  could be affected.
- Scene, prefab, Animator, Input System, and ScriptableObject changes MUST be
  checked for missing scripts, missing references, unintended overrides, and
  accidental serialized noise.
- Generated, cache, temporary, and machine-local files MUST NOT be committed.
