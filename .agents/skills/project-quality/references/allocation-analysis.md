# Allocation and Garbage-Collection Analysis

Use this module when a review mentions allocation-free code, garbage collection,
GC spikes, pooling, boxing, closures, temporary collections, per-frame or
per-tick allocations, or memory-related performance suggestions.

## Contents

- [Position](#position)
- [Vocabulary](#vocabulary)
- [Classify the execution path](#classify-the-execution-path)
- [Review workflow](#review-workflow)
- [Static signals and caveats](#static-signals-and-caveats)
- [Preferred remedies](#preferred-remedies)
- [Suggestions outside hot paths](#suggestions-outside-hot-paths)
- [Evidence and tests](#evidence-and-tests)
- [Finding format](#finding-format)
- [References](#references)

## Position

Treat allocation-free code as a targeted quality goal, not a universal style
rule. The objective is to prevent unnecessary, recurring managed allocations
from destabilizing frame or tick budgets while preserving correctness,
readability, ownership, and memory bounds.

For an established simulation tick or another measured hot path, the default
expectation is zero avoidable managed allocations after initialization and
warm-up. This reduces allocation pressure and makes execution more predictable;
it does not, by itself, make a simulation deterministic. Initialization,
loading, editor tooling, menus, diagnostics, and other infrequent paths may
allocate when that is the simplest safe design.

Do not report an allocation as a defect merely because `new` appears in source.
First determine whether it creates a managed heap object, how often the path
runs, whether the object is short-lived or retained, and whether the measured
cost matters on a supported target.

## Vocabulary

- **Managed allocation:** memory obtained for a managed object, array, delegate,
  closure, boxed value, string, or collection. Unity's `GC.Alloc` profiler data
  primarily describes this category.
- **Transient allocation:** an object that becomes garbage soon after use. A high
  transient allocation rate increases GC work and can produce frame-time spikes.
- **Retained allocation:** memory that remains reachable. Eliminating one
  allocation may simply move cost into retained memory or increase peak usage.
- **Native allocation:** memory owned by Unity, a package, a graphics API, or a
  native container. It is not necessarily visible as managed `GC.Alloc` and still
  requires explicit ownership and disposal where applicable.
- **Allocation-free:** in this project, shorthand for no avoidable managed heap
  allocation on the measured execution path after setup. It is not a promise
  that no native work, stack use, copying, or memory growth occurs.

## Classify the execution path

Classify before proposing a remedy. If frequency is unknown, mark it
`Needs profiling` instead of assuming it is a hot path.

| Class | Typical examples | Default review stance |
|---|---|---|
| Critical hot path | Simulation tick, `Update`, `FixedUpdate`, tight entity loop, per-packet serialization | Investigate recurring allocations; require evidence for the chosen fix |
| Interactive path | Input event, UI refresh, ability activation, spawn/despawn burst | Prefer bounded and intentional allocations; measure if repeated or visible |
| Cold path | Scene setup, menu transition, one-off command, editor tooling | Suggest obvious waste, but do not block on allocation alone |
| Startup/loading | Asset loading, pool warm-up, session creation | Allocations are expected; review peak memory, lifetime, and hitch timing |
| Unknown | Path not exercised or frequency not established | Request a representative profile before elevating severity |

Use the project's performance standard and change gates together: a performance
claim needs a target platform, representative workload, metric, budget, and
comparable before/after evidence.

## Review workflow

1. **Locate the boundary.** Identify the method, caller, Unity callback, system,
   job, or test that owns the work. Trace whether it runs once, per frame, per
   tick, per entity, or per network message.
2. **Separate static suspicion from proof.** Record constructs that may allocate,
   but label them `Likely` until the actual runtime and compiler path are known.
3. **Check semantics and ownership.** Before recommending a pool, `struct`,
   `Span`, `ArrayPool`, native container, or `NonAlloc` API, check lifetime,
   aliasing, thread safety, capacity, truncation, reset behavior, disposal, and
   whether the change alters value/reference semantics.
4. **Measure the baseline.** Use a representative development build on target
   hardware when possible. Capture `GC.Alloc` bytes/events, frame or tick time,
   GC activity/spikes, and retained/peak memory. Warm up code and exclude the
   profiler's own setup from the measured interval.
5. **Choose the smallest remedy.** Prefer eliminating unnecessary work or
   reusing an already-owned buffer. Add pooling or data-oriented machinery only
   when the measured problem justifies its lifecycle and memory cost.
6. **Re-measure and classify the result.** Keep the change only when the metric
   improves without unacceptable correctness, memory, complexity, or platform
   regressions. If no runtime evidence exists, report `Not verified`.

## Static signals and caveats

These are review prompts, not automatic violations:

| Signal | What to inspect | Common false positive or trade-off |
|---|---|---|
| `new` class, array, collection, or string in a loop | Frequency, lifetime, capacity growth, and whether it becomes garbage | `new` for a value type may be stack/inline; an allocation during setup may be correct |
| LINQ in recurring code | Iterator, delegate, closure, and intermediate collection behavior | A query outside a hot path may be clearer and acceptable |
| Lambda, method group, or event subscription | Whether a closure captures state and whether a delegate is created repeatedly | A non-capturing delegate can be cached; verify generated/runtime behavior |
| Boxing or interface conversion | Value types passed as `object`, interface, interpolated/logging APIs, or non-generic collections | Some APIs are optimized; inspect the concrete overload and profile |
| `params`, string formatting, `ToString`, logging | Temporary arrays/strings and whether logging is enabled in the build | Diagnostics may intentionally allocate on a cold path; never log unbounded data in a tick |
| `foreach` | Concrete enumerator type and interface conversion | `List<T>`/array enumeration can be allocation-free; interface enumeration may box |
| Method returns an array/collection | Whether it creates a result each call and whether ownership is clear | Returning a snapshot can be the safest contract; do not expose mutable internal storage casually |
| Unity API returns an array or allocates query results | Use a cached result, buffer, or documented `NonAlloc` variant where appropriate | Fixed buffers can truncate results; handle overflow explicitly |
| Coroutine or async state machine in recurring work | Creation frequency and lifetime | One coroutine at a state transition may be fine; per-tick creation is suspicious |
| `struct` conversion or `ref` optimization | Copy size, mutation semantics, interface boxing, and reference fields | Large structs and `ref` APIs can increase complexity or copying; do not convert mechanically |
| Pooling | Reset contract, ownership, maximum size, exhaustion, and retained memory | Pools can hide leaks, preserve stale state, or waste memory; collection checks may add cost |
| Native containers or unmanaged buffers | Disposal owner, allocator lifetime, job dependencies, and safety checks | Native memory is not managed GC allocation; leaks and races remain defects |

Do not claim that a construct allocates on every platform or backend without
evidence. Unity Editor behavior can differ from a player build, and compiler,
IL2CPP, package, and API-version details matter.

## Preferred remedies

Apply in this order unless profiling or ownership constraints justify another
choice:

1. Remove unnecessary work or repeated queries.
2. Move one-time setup, delegate creation, capacity planning, and pool warm-up
   out of the recurring path.
3. Reuse an owned collection or buffer with an explicit `Clear`/reset contract;
   size it from a measured bound and define what happens at capacity.
4. Use a documented Unity non-allocating query API when repeated query results
   are the actual source of garbage, and test overflow/truncation behavior.
5. Use a pool only for objects with repeated creation/destruction and a clear
   lifecycle. Reset all mutable state on release/acquire, bound retained memory,
   and make ownership visible.
6. Consider `struct`, `ref`/`in`, `Span<T>`, `ArrayPool<T>`, Jobs, Burst, or native
   containers only after measuring and checking Unity/package compatibility.
   Preserve value/reference semantics and dispose resources through their owner.
7. Keep a readable allocation in a cold path when replacing it would add more
   complexity than the measured benefit.

For simulation code, keep presentation concerns such as strings, logging,
Unity object lookup, and UI updates outside the deterministic tick when the
architecture permits. This improves both allocation behavior and separation of
concerns, but it does not remove the need to profile the resulting boundary.

## Suggestions outside hot paths

Use recommendations to cultivate analysis without imposing a blanket gate:

- **Obvious repeated waste:** recommend moving a collection or delegate to
  initialization even if the caller is currently cold; classify as
  `Recommendation`, not `Required`, unless evidence shows impact.
- **Unbounded retention:** recommend a capacity limit, disposal path, or pool
  policy even when allocations are infrequent; memory growth is a separate risk
  from GC pressure.
- **Readability-first cold code:** leave a simple allocation in place when its
  ownership is clear and there is no demonstrated cost. Record the trade-off if
  an API is intentionally optimized for a future hot path.
- **Repeated formatting/logging:** suggest lazy/guarded diagnostics or moving
  formatting out of the tick, but do not optimize messages that are only emitted
  during failure handling unless they can occur unboundedly.
- **Allocation suspicion without runtime data:** ask for a profile or add a
  focused measurement rather than asserting a performance regression.

Good suggestions explain the likely mechanism, expected benefit, and cost of
the remedy. Avoid comments that merely say “make this allocation-free.”

## Evidence and tests

### Runtime profiling

- Use the Unity CPU Usage Profiler and inspect `GC.Alloc` by thread and call
  stack. For final conclusions, prefer a development build on representative
  target hardware; Editor-only allocations can mislead.
- Pair allocation data with frame/tick time, GC markers, and retained/peak
  memory. Zero `GC.Alloc` does not prove that a path is fast or free of native
  allocations.
- If using `ProfilerRecorder`, dispose it when recording ends. Treat profiler
  setup, string formatting, and HUD display as outside the measured interval.
- Repeat the same workload after warm-up and retain enough context to compare
  build, platform, frame/tick count, entity count, and configuration.

### Allocation-focused tests

Use Unity Test Framework's allocation constraint for small, stable contracts,
for example a warmed-up tick or serializer. Keep the measured delegate free of
captured locals and setup work; otherwise the test can measure the test harness
or delegate creation instead of the code under test. Confirm the constraint API
available in the repository's installed Test Framework version (`1.6.0` at the
time this module was written).

Allocation tests are focused regression guards, not substitutes for profiling.
They can be brittle when an implementation legitimately changes, and they do
not measure native allocations, total frame time, memory retention, or behavior
under realistic entity counts.

### Finding severity

- **Required:** measured recurring allocation breaches an established budget,
  causes a reproducible hitch, leaks/unbounds memory, or violates an explicit
  critical-path contract.
- **Recommendation:** likely avoidable allocation in an interactive or
  potentially hot path, or a safe reuse/pooling improvement with a clear owner.
- **Observation:** allocation in a cold/setup path, static suspicion without
  evidence, or a trade-off worth documenting.
- **Inconclusive:** frequency, backend, target, or measurement is missing and
  the claim cannot responsibly be classified.

## Finding format

For each allocation-related finding, report:

1. Path and execution class (`Critical hot path`, `Interactive`, `Cold`, etc.).
2. Signal and confidence (`Confirmed`, `Likely`, `Needs profiling`).
3. Evidence: profiler sample, allocation test, source construct, or explicitly
   `Not verified`.
4. Impact: transient GC pressure, retained memory, frame/tick variance, or no
   demonstrated impact.
5. Smallest remedy and its trade-offs, including ownership/capacity/reset rules.
6. Validation required or performed, with platform/build and before/after data.

## References

The guidance above is grounded in the following primary sources, checked on
2026-08-20:

- [Unity 6.3: Optimizing your code for managed memory](https://docs.unity3d.com/6000.3/Documentation/Manual/performance-optimizing-code-managed-memory.html)
- [Unity 6.3: Tracking garbage collection allocations](https://docs.unity3d.com/6000.3/Documentation/Manual/performance-track-garbage-collection.html)
- [Unity 6.3: Garbage collection modes](https://docs.unity3d.com/6000.3/Documentation/Manual/performance-incremental-garbage-collection.html)
- [Unity 6.3: `ProfilerRecorder`](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Unity.Profiling.ProfilerRecorder.html)
- [Unity Test Framework 1.6: `AllocatingGCMemory`](https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/api/UnityEngine.TestTools.Constraints.Is.html#UnityEngine_TestTools_Constraints_Is_AllocatingGCMemory)
- [Unity 6.3: Performance testing API](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.test-framework.performance.html)
- [Microsoft: Avoid memory allocations and data copies in C#](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/)
- [Microsoft: Reduce allocations with `ref` safety](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/ref-tutorial)
- [Microsoft: Garbage collection and performance](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/performance)
- [Microsoft: Object pooling with .NET collections](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/how-to-create-object-pool)
