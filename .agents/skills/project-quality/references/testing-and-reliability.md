# Testing and Reliability Standard

## Contents

- [Test strategy](#1-test-strategy)
- [Test quality](#2-test-quality)
- [Coverage](#3-coverage)
- [Reliability, persistence, and diagnostics](#4-reliability-persistence-and-diagnostics)

## 1. Test strategy

- Test at the lowest practical level that proves observable behavior.
- Use EditMode tests for deterministic rules and plain C# behavior.
- Use PlayMode tests for lifecycle, frame progression, physics, scene bindings,
  and integration with Unity runtime behavior.
- Add broader integration or smoke tests only where component-level tests cannot
  expose the relevant failure.
- A bug fix MUST include a regression test when the behavior can be isolated.
  Otherwise document why and provide the strongest practical validation.
- Mutable configuration SHOULD be tested both when initially invalid and when
  it becomes invalid after successful composition when those moments exercise
  different validation boundaries.
- New gameplay behavior MUST test the main success path and relevant failure,
  boundary, or lifecycle-sensitive paths.

## 2. Test quality

- Tests MUST be fast enough for their layer, isolated, repeatable, self-checking,
  and deterministic.
- Tests SHOULD follow Arrange–Act–Assert with one primary action or scenario.
- Names SHOULD communicate behavior, scenario, and expected outcome.
- Tests SHOULD include a concise XML `<summary>` that explains the behavioral
  contract, regression, or design decision being protected rather than merely
  restating the test name.
- Assert observable outputs, state, interactions, or side effects; do not test
  private implementation details.
- Do not widen a production API solely for testing. A runtime assembly MAY grant
  a dedicated test assembly access to internal contracts when that enables a
  lower, deterministic test boundary without creating a runtime dependency.
- Control time, random seeds, input, and external state when they affect results.
- Avoid order dependencies, shared mutable fixtures, real waits, and arbitrary
  frame counts when a deterministic signal is available.
- PlayMode tests MUST destroy created objects, cancel work, unsubscribe handlers,
  and restore global/static state.
- Asynchronous tests MUST surface operation exceptions and have bounded failure
  behavior; they must not hang indefinitely.
- A flaky test is a defect. Fix or quarantine it with ownership and rationale;
  do not normalize retries as proof of correctness.

## 3. Coverage

- Coverage is a discovery and trend signal, not proof of correctness.
- Do not enforce an arbitrary repository-wide percentage before establishing a
  reproducible baseline and coverage tooling.
- When coverage is available, prioritize meaningful changed-code behavior and
  risk-critical branches over maximizing line count.

## 4. Reliability, persistence, and diagnostics

- Validate untrusted or persisted data before use and fail safely with actionable
  diagnostics.
- Numeric tests SHOULD cover non-finite inputs and non-finite calculated results
  when those values can reach physics, transforms, animation, persistence, or
  native APIs.
- Persistence changes MUST define compatibility, migration, corruption handling,
  and failure atomicity when player data is at risk.
- Logs MUST be actionable, appropriately leveled, and free of secrets. Avoid
  repeated logging in hot paths.
- Recoverable failures SHOULD preserve a usable state. Unrecoverable developer
  configuration errors SHOULD fail early and identify the source.
- Randomized gameplay or simulation behavior that requires reproducibility MUST
  expose or record its seed.
