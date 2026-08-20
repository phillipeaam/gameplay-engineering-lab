---
name: project-quality
description: Review project changes against the documented project quality standard. Use when reviewing staged or working-tree changes, assessing architecture, validating implementation quality, identifying long-term maintenance risks, or evolving the project's quality standard.
---

# Project Quality

Treat `SPEC.md` in this skill directory as the project's authoritative quality
standard. Read it completely before reviewing or changing project code, then
load only the reference modules relevant to the requested scope.

## Reference routing

Read every applicable module; combine modules for mixed changes. Read all
modules only for a project-wide audit or broad evolution of the standard.

- Responsibilities, dependencies, contracts, C#, async, or events: read
  [architecture-and-csharp.md](references/architecture-and-csharp.md).
- Unity lifecycle, serialization, frame/physics behavior, scenes, prefabs, or
  authored assets: read
  [unity-lifecycle-and-assets.md](references/unity-lifecycle-and-assets.md).
- Tests, persistence, diagnostics, reproducibility, or failure handling: read
  [testing-and-reliability.md](references/testing-and-reliability.md).
- Optimization, profiling, allocation, jobs, Burst, or hot paths: read
  [performance.md](references/performance.md).
- Allocation-free review, GC pressure, pooling, boxing, closures, temporary
  collections, or allocation budgets: also read
  [allocation-analysis.md](references/allocation-analysis.md).
- Repository boundaries, packages, build settings, documentation, or delivery:
  read [organization-and-delivery.md](references/organization-and-delivery.md).
- Acceptance evidence and gates: always read the applicable sections of
  [change-gates.md](references/change-gates.md) after classifying the change.

Examples:

- Review gameplay C#: architecture, testing, and change gates; add Unity when a
  `MonoBehaviour` or Unity API is involved.
- Review a scene, prefab, or material: Unity and change gates; add organization
  when ownership or vendor boundaries are involved.
- Review performance work: performance and change gates, plus the modules for
  the affected implementation.
- Review allocation-sensitive code: performance, allocation analysis, change
  gates, and the modules for the affected implementation.

## Operating modes

- Review (default): inspect and report; do not modify files.
- Fix: modify code only when the user explicitly requests implementation.
- Architecture: evaluate a proposal against the standard and state trade-offs.
- Standard evolution: update `SPEC.md` or the owning reference module only for
  stable, reusable rules supported by a recurring problem, project evidence, or
  an authoritative source.

## Scope resolution

1. Use the files, diff, feature, or proposal named by the user.
2. Otherwise review staged changes.
3. If nothing is staged, review relevant working-tree changes.
4. Perform a project-wide audit only when explicitly requested.
5. Inspect surrounding code and assets when needed to understand behavior, but
   keep findings attributable to the requested change.

## Review workflow

1. Establish the intended behavior from the request, diff, tests, and project
   documentation. Do not invent requirements.
2. Classify the change type and risk using `SPEC.md`; apply only relevant gates.
3. Review correctness and data safety before architecture, tests, lifecycle,
   maintainability, performance, assets, and style.
4. Prefer facts, reproducible evidence, and documented rules over preferences.
5. Run the narrowest relevant validation available. Never claim a test, build,
   profile, or manual check that was not actually performed.
6. Distinguish defects from missing evidence. Do not turn absent automation into
   proof that the change is wrong.
7. Do not block a change for personal style, speculative abstractions, unrelated
   cleanup, or hypothetical performance concerns.

## Review output

Lead with one verdict: `Pass`, `Pass with recommendations`, `Changes required`,
or `Inconclusive`.

Then report:

1. Scope and risk classification.
2. Findings ordered by severity: Blocker, Required, Recommendation, Observation.
3. For each finding: absolute file path and line, evidence, violated rule,
   impact, and the smallest concrete correction.
4. Validation performed and its result.
5. Residual risks, missing evidence, and checks not performed.
6. Positive evidence only when it helps preserve a good decision.

If no actionable finding exists, state that explicitly. For implementation
requests, apply the smallest cohesive fix, preserve unrelated user changes, and
validate in proportion to risk.
