# Architecture and C# Standard

## Contents

- [Architecture and dependency boundaries](#1-architecture-and-dependency-boundaries)
- [C# implementation standard](#2-c-implementation-standard)

## 1. Architecture and dependency boundaries

### 1.1 Responsibilities and cohesion

- Every type MUST have one coherent reason to change at its current abstraction
  level. Split responsibilities when unrelated behavior, state, or dependencies
  repeatedly change independently.
- Methods SHOULD expose one understandable unit of behavior. Extract logic when
  nesting, branching, mutation, or side effects obscure the main path.
- State MUST have a clear owner. Avoid multiple writers unless synchronization
  and invariants are explicit.

### 1.2 Layering

- Gameplay rules SHOULD be plain C# when they do not require Unity APIs.
- `MonoBehaviour` components SHOULD coordinate Unity lifecycle, scene bindings,
  and presentation rather than contain all domain policy.
- Composition roots MAY depend on concrete implementations. Core rules SHOULD
  depend on narrow contracts only where substitution or isolation is valuable.
- Dependencies MUST point toward the more stable policy. Avoid cycles between
  modules or assemblies.
- Hidden global mutable state and service locators MUST NOT be used in core
  gameplay rules. A deliberately global service needs explicit ownership,
  lifecycle, reset behavior, and test isolation.

### 1.3 Assembly definitions and modules

- Runtime, Editor, EditMode test, and PlayMode test code MUST remain in suitable
  folders and assemblies.
- Assembly references MUST be minimal, intentional, and acyclic.
- New reusable modules SHOULD receive an `.asmdef` when it creates a meaningful
  compilation or dependency boundary; tiny local experiments need not be split
  prematurely.
- Editor-only APIs MUST NOT leak into player assemblies.
- Test assemblies MUST NOT become runtime dependencies.

### 1.4 Contracts and evolution

- Public APIs MUST express valid states and use the narrowest practical access.
- Public mutable fields are prohibited except where a Unity or serialization
  contract specifically requires them and the trade-off is documented.
- Validate inputs at public/protected boundaries and constructors. Do not repeat
  the same check through private layers after an invariant is established.
- Changes to public contracts, serialized fields, save data, scene bindings, or
  package-facing APIs MUST consider compatibility and migration.
- Breaking changes MUST be explicit, isolated, and accompanied by migration or
  an intentional reset strategy.

## 2. C# implementation standard

### 2.1 Readability and style

- Follow existing project naming: PascalCase for types, methods, properties,
  events, and constants; camelCase for parameters and locals; `_camelCase` for
  private fields.
- Prefer names that communicate domain intent rather than implementation type.
- Use `var` when the right-hand side makes the type obvious. Use an explicit
  type when inference hides an important domain, API, numeric, or ownership
  detail.
- Prefer target-typed `new()` when the declared target type is clear and the
  concrete type is not an architectural decision. Never write `var x = new()`.
- Always use braces for control-flow bodies, including guard clauses, with
  Allman-style brace placement.
- Comments SHOULD explain decisions, invariants, non-obvious algorithms, and
  Unity constraints. Do not restate readable code.
- Keep formatting-only edits out of behavioral changes when they add review
  noise.

### 2.2 Nullability, errors, and invariants

- Use `ArgumentNullException` or another precise exception for invalid caller
  input at a public boundary.
- Do not catch an exception unless the code can recover, add useful context, or
  translate it at a boundary. Never silently swallow failures.
- Error messages MUST identify the failed operation or field sufficiently for
  diagnosis without exposing secrets.
- Use assertions for developer invariants, not for recoverable player input.
- Do not use finalizers in Unity runtime code.

### 2.3 Collections and state exposure

- Expose read-only views or behavior instead of mutable collections when callers
  must not mutate owned state.
- Avoid returning internal mutable buffers that bypass invariants.
- Choose collections according to access patterns; do not replace clear code
  with specialized structures without a demonstrated need.

### 2.4 Asynchronous and event-driven code

- Every asynchronous operation MUST have explicit ownership for completion,
  cancellation, exceptions, and teardown.
- Propagate `CancellationToken` through cancellable operations. Tie long-lived
  operations to the owning object's lifecycle.
- Avoid `async void` except framework-required event or Unity callbacks; handle
  exceptions explicitly in those boundaries.
- Fire-and-forget work is prohibited unless failure handling and lifetime are
  explicit.
- Event subscriptions MUST have a symmetric, idempotent unsubscription path.
- Unity API access MUST remain on a permitted Unity thread/context.
