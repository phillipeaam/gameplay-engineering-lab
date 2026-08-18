# XYZ Commit Message Generation Specification

## Purpose

Generate a structured Git commit message from the provided code changes.

The commit message must be useful for:

- human code review;
- technical history;
- changelog generation;
- engineering reports;
- automated analysis;
- RepoDNA processing.

The generated message must describe the **reason, technical change, validation and impact** of the commit.

Do not simply repeat filenames or describe the diff line-by-line.

---

# Output Format

Use the following format when the change is significant:

```
<type>(<scope>): <short summary>

Why:
<why the change was necessary>

Changes:
- <important technical change>
- <important technical change>
- <important technical decision>

Validation:
<how the change was validated>

Area: <area>
Impact: <impact>
Risk: <risk>
Report: <include|exclude>
Refs: <reference if available>
```

---

# Title

Format:

```
<type>(<scope>): <summary>
```

The title must:

- use imperative/simple present style;
- be concise;
- describe the result rather than the implementation process;
- avoid vague words such as `changes`, `updates`, `stuff`, `adjustments`;
- avoid ending with a period.

Example:

```
feat(ai): add weighted target selection
```

---

# Type Classification

Choose the most appropriate Conventional Commit type.

```
feat      new behavior or capability
fix       bug correction
refactor  structural change without intended behavior change
perf      performance improvement
test      test-related change
docs      documentation
build     dependencies or build system
ci        continuous integration
chore     repository maintenance
```

Prefer the type that represents the **primary purpose of the commit**.

---

# Scope

The scope should represent the system or domain primarily affected.

Examples:

```
ai
combat
movement
pathfinding
inventory
ui
audio
save
tools
architecture
performance
build
tests
```

Do not use filenames as scopes unless the repository itself uses that convention.

---

# Why

Explain the reason behind the change.

Focus on:

- problem being solved;
- limitation of previous behavior;
- requirement introduced;
- architectural motivation;
- technical debt being addressed.

Do not merely restate the title.

Good:

```
Why:
Enemy decisions were coupled to movement execution, making new
combat behaviors difficult to introduce.
```

Bad:

```
Why:
The AI code needed to be changed.
```

---

# Changes

Describe the most important implementation decisions.

Use 1–5 bullets.

Prioritize:

- architecture;
- algorithms;
- behavior;
- data flow;
- contracts/interfaces;
- important dependencies;
- performance-relevant changes.

Do not list every changed file.

Example:

```
Changes:
- Extracted target evaluation from enemy turn execution.
- Added configurable scoring rules for available targets.
- Excluded unreachable targets before action selection.
```

---

# Validation

Describe evidence that the change works.

Possible evidence:

- unit tests;
- integration tests;
- automated tests;
- manual gameplay verification;
- profiling;
- benchmarks;
- build validation;
- static analysis.

Examples:

```
Validation:
Added unit coverage for target scoring and manually verified enemy
behavior across three tactical encounters.
```

If no validation evidence is provided or observable:

```
Validation:
Not specified.
```

Never invent tests or validation.

---

# Area

Classify the broader technical area.

Preferred values:

```
Gameplay
Gameplay/AI
Gameplay/Combat
Gameplay/Movement
Architecture
Performance
UI
Tools
Persistence
Build
Testing
Documentation
Infrastructure
```

Use the most specific meaningful value.

---

# Impact

Select one primary impact:

```
Behavior
Architecture
Performance
Reliability
UX
Tooling
Build
Testing
Documentation
```

Choose based on the most important consequence of the change.

---

# Risk

Estimate change risk from the available evidence.

```
Low
Medium
High
```

Guidelines:

### Low

- documentation;
- isolated tooling;
- small bug fix;
- tests;
- localized implementation.

### Medium

- gameplay behavior;
- refactors affecting multiple components;
- new systems;
- persistence changes with backward compatibility.

### High

- save format changes;
- core architecture replacement;
- networking;
- data migration;
- large cross-system changes;
- changes that can break existing consumers.

Do not exaggerate risk.

---

# Report

Determine whether the commit is useful for future engineering reports.

Use:

```
Report: Include
```

for:

- features;
- meaningful fixes;
- architecture changes;
- performance work;
- significant refactoring;
- major tooling;
- important tests or reliability improvements.

Use:

```
Report: Exclude
```

for:

- typo corrections;
- formatting;
- dependency lockfile noise;
- minor cleanup;
- trivial maintenance.

---

# References

If an issue, ticket, PR or task identifier is provided, include:

```
Refs: #42
```

or:

```
Refs: GAME-142
```

If no reference exists:

```
Refs: None
```

Never invent references.

---

# Breaking Changes

If the change breaks an existing public contract, API, data format or expected behavior, use:

```
feat(save)!: replace legacy save format
```

And include:

```
BREAKING CHANGE: Existing save files require migration.
```

Only mark a change as breaking when there is clear evidence.

---

# Small Commit Mode

Do not generate the full XYZ body for trivial changes.

For small commits, output only:

```
<type>(<scope>): <summary>
```

Examples:

```
docs(readme): add installation instructions
```

```
fix(ui): correct tooltip alignment
```

```
test(combat): cover zero-damage attacks
```

---

# When to Use Full XYZ

Generate the full structure when the commit contains:

- a new feature;
- gameplay behavior;
- important bug fix;
- architectural decision;
- significant refactor;
- optimization;
- new integration;
- persistence/data changes;
- tooling worth reporting;
- significant testing or reliability work.

---

# Evidence Rules

Only describe facts supported by the supplied information.

Possible inputs include:

- git diff;
- staged diff;
- changed files;
- issue description;
- task description;
- test output;
- developer notes;
- repository context.

Never invent:

- motivations;
- tests;
- performance gains;
- bugs;
- requirements;
- architecture decisions.

If information is unknown, either omit it or explicitly state that it was not specified.

---

# Important Reasoning Rule

The Git diff already records:

> what lines changed.
>

The commit message should preserve:

> why the change happened, what technical decision was made, and what impact it has.
>

Prefer describing intent and system behavior instead of mechanically translating code changes.

---

# Example Input

```
Changed:
- EnemyTurnController no longer calculates target scores.
- Added TargetEvaluator.
- Added configurable TargetScoringRule ScriptableObjects.
- Targets without reachable paths are ignored.
- Added tests for distance and health scoring.
```

# Expected Output

```
refactor(ai): separate target evaluation from enemy turns

Why:
Target selection was coupled to enemy turn execution, making new
decision rules difficult to add and test independently.

Changes:
- Extracted target scoring into a dedicated TargetEvaluator.
- Added configurable TargetScoringRule ScriptableObjects.
- Filtered unreachable targets before scoring.
- Decoupled decision evaluation from turn execution.

Validation:
Added automated coverage for distance and health scoring rules.

Area: Gameplay/AI
Impact: Architecture
Risk: Medium
Report: Include
Refs: None
```

---

# Final Instruction to the Model

Analyze the supplied repository context and code changes.

Determine whether the change requires a full XYZ commit or a simple Conventional Commit.

Generate **only the final commit message**.

Do not include explanations, Markdown fences, alternatives or commentary outside the commit message.
