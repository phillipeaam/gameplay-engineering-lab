# Pull Request Standard

Verified: 2026-08-18

## Contents

- [Purpose](#purpose)
- [Title contract](#title-contract)
- [Description contract](#description-contract)
- [Unity and game-development evidence](#unity-and-game-development-evidence)
- [Scope and reviewability](#scope-and-reviewability)
- [Metrics](#metrics)
- [Automation path](#automation-path)
- [References](#references)

## Purpose

A pull request is a review and delivery record, not a restatement of its diff.
It should preserve why the change exists, its important decisions, its effect,
the evidence available, and the areas of uncertainty or risk.

The project quality standard remains authoritative for acceptance. This
reference defines how a PR communicates that evidence.

## Title contract

Use Conventional Commits:

```text
<type>(<scope>): <result>
```

- Select the type from `feat`, `fix`, `refactor`, `perf`, `test`, `docs`,
  `build`, `ci`, or `chore` according to the primary purpose.
- Use a domain scope such as `locomotion`, `combat`, `animation`, `ui`,
  `save`, `build`, or `tools`; do not use a filename merely because it changed.
- Describe the result in imperative or simple-present style.
- Mark a demonstrated breaking contract with `!` and explain migration or
  compatibility impact in the body.
- Avoid vague summaries such as `update code`, `changes`, or `improvements`.

Examples:

```text
fix(locomotion): clear input state when disabled
feat(combat): add directional guard reactions
perf(ai): reduce pathfinding allocations
```

## Description contract

Use `.github/pull_request_template.md` as the canonical structure. Retain only
the sections relevant to the change.

### Summary

State the result and affected user or developer in one to three sentences.

### Why

State the problem, requirement, limitation, or root cause. For a fix, precisely
characterize the prior failure. Link an issue or decision record when one exists.

### Changes

Describe the important behavior, data flow, contract, or design decisions.
Avoid a mechanical list of changed files.

### Validation

Report commands or procedures actually executed and their results. Keep these
states distinct:

| State | Acceptable wording |
|---|---|
| Evidence exists in the diff | `Added PlayMode regression coverage for ...` |
| Check was executed successfully | `Passed: Unity PlayMode tests (...)` |
| Check was executed and failed | `Failed: ...`; keep draft or explain disposition |
| Check was not executed | `Not run: ...` with the reason |

Never convert the presence of test code into a passing test result.

### Impact and risk

Describe relevant behavior, compatibility, migration, persistence, performance,
platform, lifecycle, dependency, or rollback implications. `Low risk` requires a
brief basis, such as an isolated and covered change; it is not a substitute for
analysis.

### Related work and reviewer focus

Link available issues, PRs, designs, or dependencies. Highlight the decisions,
files, or risks where review effort is most valuable. Use `None` when the field
is retained and no evidence exists.

## Unity and game-development evidence

Include only applicable context:

- Unity editor version used.
- Scene, prefab, character, level, or test fixture exercised.
- Target platform, graphics API, quality level, and input device when behavior
  depends on them.
- Screenshot, GIF, capture, or short video for visual, UI, animation, VFX,
  lighting, scene, or player-facing changes.
- Profiler capture, representative workload, target hardware, baseline, and
  before/after result for a performance claim.
- Intended serialized changes to scenes, prefabs, Animator Controllers, Input
  Actions, packages, or `ProjectSettings`.
- Verification of GUIDs, references, prefab overrides, and missing scripts when
  authored assets change.

Visual evidence complements tests; it does not replace deterministic coverage
for isolatable behavior.

## Scope and reviewability

- Prefer one self-contained behavior or objective per PR.
- Include tests that prove the changed behavior in the same PR when practical.
- Separate broad refactoring, generated changes, vendor imports, dependency
  upgrades, and unrelated cleanup when they obscure review or rollback.
- Every commit or PR in a dependent sequence should leave the project in a
  usable state. Link dependent or stacked PRs explicitly.
- Use draft status while the design, validation, dependencies, or evidence are
  incomplete.

Changed lines and file count indicate review cost but are not universal quality
thresholds. A serialized scene or generated asset can be large yet conceptually
cohesive; a small lifecycle change can be high risk.

## Metrics

Metrics are team-level decision signals. Do not rank individuals or optimize a
single number. Establish a reproducible baseline before setting targets.

### Per-PR acceptance signals

| Concern | Signal | Baseline expectation |
|---|---|---|
| Cohesion | Independent or unrelated changes mixed into scope | 0 when avoidable |
| Traceability | Problem, issue, or decision context recorded | Present when available |
| Evidence integrity | Unsupported test, build, compatibility, or performance claims | 0 |
| Validation | Relevant checks reported as passed, failed, or not run | Explicit |
| Risk | Material compatibility, lifecycle, asset, platform, and rollback risks | Disclosed |
| Reviewability | Reviewer focus identified for non-trivial changes | Present |
| Regression protection | Isolatable bug fixes with focused regression coverage | Expected |
| Visual confidence | Player-facing visual changes with representative evidence | Expected when practical |

### Trend metrics

Track these only when the repository can collect them consistently:

- PR batch size distribution by changed files and lines, segmented by generated
  or serialized content rather than reduced to one threshold.
- Median time from ready-for-review to first substantive review.
- Median review and merge lead time, segmented by change type and draft time.
- Percentage of PRs with required checks completed or explicitly waived.
- Post-merge reverts, hotfixes, or regressions attributable to a merged change.
- Deployment change lead time and change fail rate after a deployment workflow
  exists.

Interpret speed together with stability, review quality, and developer
experience. Short merge time with growing regressions is not improvement.

## Automation path

Introduce enforcement progressively:

1. Commit the GitHub template and use the preparation skill.
2. Validate PR title syntax in CI.
3. Add compile and EditMode/PlayMode checks when their batch execution is
   reproducible.
4. Protect `develop` with required checks and review policy.
5. Add changed-code coverage or performance budgets only after recording a
   meaningful baseline.

GitHub closing keywords automatically close issues only when a PR targets the
repository's default branch. For PRs targeting a non-default `develop` branch,
link the issue manually or verify the repository branch strategy.

## References

- [GitHub: creating a pull request template](https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests/creating-a-pull-request-template-for-your-repository)
- [GitHub: managing and standardizing pull requests](https://docs.github.com/en/pull-requests/reference/managing-and-standardizing-pull-requests)
- [GitHub: helping others review changes](https://docs.github.com/en/pull-requests/concepts/helping-others-review-your-changes)
- [GitHub: linking pull requests to issues](https://docs.github.com/en/issues/tracking-your-work-with-issues/using-issues/linking-a-pull-request-to-an-issue)
- [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/)
- [Google Engineering Practices: small changes](https://google.github.io/eng-practices/review/developer/small-cls.html)
- [Google Engineering Practices: what to review](https://google.github.io/eng-practices/review/reviewer/looking-for.html)
- [Godot: pull request rules and guidelines](https://github.com/godotengine/godot-contributing-docs/blob/main/pull_requests/pull_request_guidelines.rst)
- [Godot: contributor guidelines](https://github.com/godotengine/godot/blob/master/CONTRIBUTING.md)
- [DORA software delivery performance metrics](https://dora.dev/guides/dora-metrics/)
- [Microsoft Research: SPACE of Developer Productivity](https://www.microsoft.com/en-us/research/publication/the-space-of-developer-productivity-theres-more-to-it-than-you-think/)
