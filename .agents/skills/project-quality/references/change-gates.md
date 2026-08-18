# Change Gates and Automation Standard

## Contents

- [Quality gates by change type](#1-quality-gates-by-change-type)
- [Metrics and automation maturity](#2-metrics-and-automation-maturity)

## 1. Quality gates by change type

### 1.1 Every change

- The change is cohesive and its intent can be stated clearly.
- The project has no new compiler errors or warnings attributable to the change.
- Relevant existing tests pass, or missing execution is reported.
- No known exception, missing reference, broken GUID, accidental generated file,
  secret, or unrelated edit is included.
- New behavior and public contracts have validation proportional to risk.
- Deliberate exceptions to this standard are recorded.

### 1.2 Gameplay feature

- Main success behavior is tested at the lowest practical level.
- Relevant invalid input, boundary, timing, cancellation, and lifecycle paths are
  covered or their omission is justified.
- Responsibilities, state ownership, and dependencies are understandable.
- Player-facing behavior receives a representative PlayMode or manual gameplay
  check when pure tests cannot prove it.

### 1.3 Bug fix

- The original failure is reproduced or precisely characterized.
- A regression test fails before and passes after the fix when practical.
- The fix addresses the cause without masking a wider invariant violation.

### 1.4 Architecture or refactor

- Observable behavior is preserved unless a behavior change is explicit.
- Dependency direction and ownership become no worse.
- Existing tests pass; characterization tests cover risky legacy behavior.
- Added complexity has a documented problem and trade-off.

### 1.5 Scene, prefab, or authored asset

- Missing scripts/references and prefab overrides are checked.
- The affected scene or prefab is loaded and exercised when practical.
- Serialized diffs contain only intended changes.

### 1.6 Performance change

- A baseline, representative workload, target platform, and metric exist.
- Before/after captures are comparable and retained or summarized.
- Correctness and maintainability are not traded away without an explicit budget
  need and documented rationale.

### 1.7 Dependency, tooling, or build change

- Version and lockfile changes are understood.
- Compatibility, licensing, build impact, and rollback are considered.
- A relevant build, package resolution, or tool execution is validated.

### 1.8 Pull request evidence

- The title identifies the primary type, domain, and result.
- The description states why the change exists and avoids restating the diff.
- Validation claims are supported by observed output or explicitly supplied
  evidence; checks not executed are identified as `Not run`.
- Material impact, compatibility, Unity serialization, platform, performance,
  migration, and rollback risks are disclosed when relevant.
- The reviewer focus identifies non-obvious decisions or risk areas for a
  non-trivial change.

## 2. Metrics and automation maturity

Track trends on changed code first. Current review-level signals are:

| Quality concern | Signal | Baseline expectation |
|---|---|---|
| Functional suitability | Relevant behavior checks | Required by change risk |
| Reliability | New attributable errors/failures | 0 |
| Maintainability | Blocker/Required findings | 0 accepted without exception |
| Testability | New gameplay behavior with focused tests | Expected when isolatable |
| Architecture | New cycles or unjustified dependencies | 0 |
| Performance | Regressions against an established budget | 0 unexplained |
| Organization | Runtime/Test/Editor/vendor boundary violations | 0 |
| Delivery | Unrelated change mixed into scope | 0 when avoidable |
| PR evidence | Unsupported validation or performance claims | 0 |
| PR reviewability | Required context, risk, and validation status | Explicit |

The following become hard automated gates only after their tooling is committed
and reproducible:

1. `.editorconfig` and selected Roslyn analyzer severities.
2. Batch-mode compile and EditMode/PlayMode tests in CI.
3. Coverage collection with a recorded baseline and changed-code policy.
4. Target-platform build validation.
5. Performance tests and budgets for identified critical paths.

Until then, a review MUST report these as missing automation or missing evidence,
not silently mark them as passed.
