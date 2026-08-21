# Project Quality Standard

Status: Baseline v1.0
Scope: Unity 6 / C# gameplay code, tests, scenes, assets, tooling, dependencies,
and project organization
Enforcement: review-enforced, with automation introduced progressively

## Contents

- [Purpose](#1-purpose)
- [Normative language and evidence](#2-normative-language-and-evidence)
- [Engineering principles](#3-engineering-principles)
- [Review severity and verdicts](#4-review-severity-and-verdicts)
- [Module routing](#5-module-routing)
- [Exceptions and evolution](#6-exceptions-and-evolution)
- [Authoritative references](#7-authoritative-references)

## 1. Purpose

This standard defines the minimum quality bar for a long-lived gameplay
engineering laboratory. Its purpose is to preserve correctness, learning value,
maintainability, testability, and safe evolution as experiments grow or become
shared systems.

The priority order is:

1. Correct behavior, data integrity, and safe Unity lifecycle usage.
2. Clear ownership, explicit dependencies, and controlled change impact.
3. Tests and evidence proportional to risk.
4. Readability, consistency, and project organization.
5. Measured performance on relevant platforms and hot paths.

The standard adapts the ISO/IEC 25010:2023 product quality model to this project,
with emphasis on functional suitability, reliability, maintainability,
performance efficiency, compatibility, security, flexibility, and safety.

## 2. Normative language and evidence

- **MUST**: required unless an explicit exception is documented.
- **SHOULD**: default choice; deviations need a concrete reason.
- **MAY**: optional technique selected according to context.

Rules apply only when relevant to the change. A review MUST distinguish:

- observed defects;
- demonstrated risks;
- missing validation evidence;
- optional improvements;
- personal preferences, which are not findings.

Never invent requirements, tests, results, performance gains, bugs, or design
intent. When evidence is unavailable, say `Not verified`.

## 3. Engineering principles

### 3.1 Improve code health incrementally

- Each change MUST leave the touched system no harder to understand, test, or
  evolve without a documented trade-off.
- Reviewers SHOULD approve sound incremental improvement without demanding
  unrelated perfection.
- Functional changes and broad formatting or cleanup SHOULD be separate when
  combining them obscures review, rollback, or history.

### 3.2 Prefer the simplest sufficient design

- Solve the current demonstrated problem; do not build speculative frameworks.
- Add an abstraction only when it creates a useful boundary, supports a real
  variation, isolates a volatile dependency, or materially improves testing.
- Treat patterns and SOLID principles as diagnostics, not implementation goals.
- Prefer composition, cohesive objects, and explicit data flow over deep
  inheritance, hidden control flow, or service location.

### 3.3 Make risk and trade-offs explicit

- Scale design, review depth, and validation to change risk.
- Assess risk through user impact, likelihood, blast radius, reversibility,
  lifecycle sensitivity, persistence impact, and platform dependence.
- Significant architectural, persistence, dependency, or performance decisions
  MUST record alternatives and trade-offs in the owning experiment's README or
  another project-approved decision record.

### 3.4 Prefer enforceable rules

- Automate stable mechanical rules when the project has suitable tooling.
- Do not declare numeric gates that cannot be measured reproducibly.
- Metrics are decision signals, not targets to game.

## 4. Review severity and verdicts

### 4.1 Severity

- **Blocker**: credible risk of data loss, security exposure, crash, unusable
  core behavior, broken build, corrupted Unity references, or a failing required
  validation attributable to the change.
- **Required**: a concrete correctness, lifecycle, contract, architecture, test,
  or maintainability defect that should be fixed before accepting the change.
- **Recommendation**: a meaningful improvement with limited current risk; it
  does not block acceptance.
- **Observation**: relevant context, positive evidence, or future consideration
  with no requested action.

Severity MUST reflect demonstrated impact and likelihood, not wording intensity.

### 4.2 Verdict

- **Pass**: no actionable defects and sufficient evidence for the change risk.
- **Pass with recommendations**: no required fixes; optional improvements exist.
- **Changes required**: at least one Blocker or Required finding exists.
- **Inconclusive**: the available scope or evidence cannot support a responsible
  verdict. State exactly what is missing.

## 5. Module routing

Read every module relevant to the requested scope. Read all modules only for a
project-wide audit or broad evolution of the quality standard.

| Change or concern | Required module |
|---|---|
| Responsibilities, dependencies, contracts, C# implementation, async or events | `references/architecture-and-csharp.md` |
| `MonoBehaviour`, serialization, frame/physics behavior, scenes, prefabs or Unity assets | `references/unity-lifecycle-and-assets.md` |
| Tests, persistence, diagnostics, reproducibility or failure handling | `references/testing-and-reliability.md` |
| Optimization, allocation, profiling, jobs, Burst or hot paths | `references/performance.md` |
| Allocation-free review, GC pressure, pooling, boxing, closures or temporary collections | `references/allocation-analysis.md` |
| Repository boundaries, packages, build settings, documentation or delivery | `references/organization-and-delivery.md` |
| Acceptance evidence and gates for the classified change type | `references/change-gates.md` |

For mixed changes, combine modules. For example, review a gameplay
`MonoBehaviour` with the architecture, Unity, testing, and change-gate modules.
Review an authored scene with the Unity and change-gate modules.

## 6. Exceptions and evolution

An exception MUST record:

- the rule being waived;
- the concrete reason;
- scope and duration;
- risk and mitigation;
- owner or revisit trigger when temporary.

Add or strengthen a rule only when it addresses a recurring problem, protects a
known quality attribute, can be reviewed consistently, and includes a practical
example or measurable signal. Remove or relax rules that create ceremony without
reducing ambiguity or risk.

Review this standard after several meaningful changes, after a production-like
failure, or when tooling makes a previously manual gate enforceable.

## 7. Authoritative references

Verified 2026-08-20:

- [ISO/IEC 25010:2023 product quality model](https://www.iso.org/standard/78176.html)
- [Unity 6.3 assembly definitions](https://docs.unity3d.com/6000.3/Documentation/Manual/assembly-definitions-intro.html)
- [Unity 6.3 script serialization](https://docs.unity3d.com/6000.3/Documentation/Manual/script-serialization.html)
- [Unity 6.3 Test Framework](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.test-framework.html)
- [Unity 6.3 profiling on a target platform](https://docs.unity3d.com/6000.3/Documentation/Manual/profiling-target-device.html)
- [Unity 6.3 optimizing code for managed memory](https://docs.unity3d.com/6000.3/Documentation/Manual/performance-optimizing-code-managed-memory.html)
- [Unity 6.3 tracking garbage collection allocations](https://docs.unity3d.com/6000.3/Documentation/Manual/performance-track-garbage-collection.html)
- [Microsoft C# performance guidance](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/)
- [Microsoft C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Microsoft .NET enum design guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/enum)
- [Microsoft unit testing best practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Google code review standard](https://google.github.io/eng-practices/review/reviewer/standard.html)
- [Google code review checklist](https://google.github.io/eng-practices/review/reviewer/looking-for.html)
