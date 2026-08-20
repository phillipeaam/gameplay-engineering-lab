---
name: prepare-pr-set
description: Split a large staged, unstaged, or mixed working tree into a safe set of small branches, XYZ commits, and published draft pull requests, targeting the integration branch directly when packages are independent and stacking only genuine dependencies. Use when Codex needs to execute an end-to-end PR split, decompose a mega PR, recover reviewable packages from accumulated local changes, design a mixed direct-and-stacked PR graph, preserve a WIP snapshot, or prove that every intended change is represented without unrelated work.
---

# Prepare PR Set

Turn accumulated local work into a reviewable, reversible set of PRs without
losing user changes. Allow independent PRs and dependent stacks in the same
delivery plan. Orchestrate existing project skills instead of duplicating them.

## Required skills

Read and follow these skills before acting:

1. `../project-quality/SKILL.md` to classify and review every package.
2. `../xyz-commit/SKILL.md` to commit each staged package.
3. `../prepare-pull-request/SKILL.md` to prepare, publish, and verify each PR.

Their specifications remain authoritative. This skill owns only preservation,
partitioning, dependency modeling, branch construction, and set verification.

## Authority checkpoints

An explicit invocation of `$prepare-pr-set`, or an explicit request to execute
the PR-set workflow, authorizes the complete workflow: local snapshots,
branches, XYZ commits, branch pushes, and creation of the corresponding draft
pull requests. Complete every applicable step through published metadata
verification unless a validation fails or an external blocker prevents it.

Do not infer publication authority when this skill triggers implicitly or when
the user asks only for analysis, review, explanation, or a proposed strategy.
In those cases, remain read-only and request authorization before mutation.

Creating non-draft PRs, merging, closing, deleting branches, rewriting remote
history, or updating unrelated existing PRs always requires separate explicit
authorization.

Never use destructive recovery commands, discard user changes, repurpose a
user branch as a temporary branch, or publish WIP verification branches.

## Workflow

### 1. Inventory the complete change set

- Record the current branch, upstream, HEAD, and `git status --short --branch`.
- Inspect staged, unstaged, and untracked files separately.
- Read the complete diff and relevant surrounding code.
- Identify generated files, Unity `.meta` files, serialized assets, and
  unrelated user work.
- Resolve the integration base from repository context or ask when a wrong
  assumption would change the PR graph.

Do not assume the index represents the desired package boundaries.

### 2. Preserve a recoverable source snapshot

Before rearranging mixed local changes, create a clearly named local-only WIP
branch and snapshot commit containing the approved source change set. Preserve
the original staged/unstaged inventory in the analysis record.

Use a name such as:

```text
wip/<domain>-split-source
```

Keep it local. Do not delete it until the PR set has been proven complete and
the user no longer needs the recovery point. If unrelated changes cannot be
isolated safely, stop and request direction.

### 3. Design cohesive packages and dependency edges

Use `project-quality` to classify responsibilities, contracts, runtime behavior,
tests, documentation, Unity assets, and risk. Split by independently explainable
objective rather than file count.

For each package define:

- branch name and Conventional Commit title;
- exact behavior or decision it owns;
- included files or hunks;
- selected base branch and why;
- validation proportional to risk;
- compatibility, Unity, and review concerns.

Choose the base according to a real dependency:

- Target the integration branch directly when a package can compile, validate,
  merge, and be reviewed independently.
- Target another package branch only when the package requires its contracts,
  code, tests, assets, or migration to remain usable.
- Do not stack PRs merely to impose review order or because the changes were
  discovered together.

The result may be one direct PR, several independent PRs, one stack, or multiple
stacks rooted at the integration branch. Model it as a dependency graph and
ensure it is acyclic. Order dependent packages topologically.

Present the proposed PR set before mutation when the user requested a strategy
or when package boundaries or dependency edges require material judgment.

### 4. Build every branch from its selected base

For each package:

1. Switch to the selected base and confirm a clean working tree.
2. Create a specifically named branch.
3. Apply only its approved files or hunks from the source snapshot.
4. Preserve required Unity `.meta` pairing and GUID continuity.
5. Review the package with `project-quality`.
6. Run the narrowest meaningful validation and record observed results.
7. Stage only that package and inspect the complete staged diff.
8. Invoke `xyz-commit`; do not handcraft a substitute commit format.
9. Confirm the working tree state before continuing.

Default to one cohesive commit per branch. Use more only when the user approves
a reason that improves independent review or rollback.

### 5. Prove package and aggregate completeness

After building all local packages:

- confirm each branch descends from its declared base;
- confirm each branch contains the intended commit count;
- run `git diff --check` for every base-to-head pair;
- confirm no package contains unrelated changes;
- assemble all terminal packages on a local-only verification branch when the
  set has more than one independent root;
- compare the assembled tree with the WIP source snapshot;
- confirm the working tree is clean.

Never publish the verification branch. A merge conflict during assembly is
evidence that the packages are not operationally independent until the graph or
integration decision is corrected.

Run [`scripts/verify_pr_set.sh`](scripts/verify_pr_set.sh) for one-commit
packages:

```bash
scripts/verify_pr_set.sh \
  <source-snapshot|-> \
  <assembled-ref|-> \
  <branch-1> <base-1> \
  [<branch-2> <base-2> ...]
```

For a single chain, its terminal branch can be `<assembled-ref>`. For multiple
independent roots, pass the local verification branch. Pass `-` for both
snapshot arguments only when aggregate tree equivalence is not applicable.
Treat unexplained failures as blockers to publication.

### 6. Publish and prepare draft PRs

When the full workflow is authorized:

1. Push package branches, never snapshot or verification branches.
2. Invoke `prepare-pull-request` for every package.
3. Target each PR at the base selected during dependency analysis.
4. Link the immediate prerequisite PR only when a dependency edge exists.
5. State `None` for related work when an independent PR has no known link.
6. Keep every PR in draft status unless the user explicitly requests otherwise.

If authentication, network access, repository permissions, or another external
condition blocks publication, preserve the verified local branches and report
the exact blocker. Do not describe the workflow as complete without the draft
PRs.

Do not reuse cumulative validation claims. Each PR body must report only checks
observed for that package or explicitly state what was not run.

### 7. Re-read the published PR set

Fetch the published metadata and verify for every PR:

- title, head, and declared base;
- draft status;
- commit count and cohesive scope;
- dependency link only where one exists;
- validation wording and risk disclosure;
- absence of placeholders, template comments, null links, or unsupported claims.

Report direct PRs and each dependent chain clearly, along with local WIP refs,
validation results, and residual risk. Warn that automatic remote branch
deletion can retarget open dependent PRs, so bases should be checked after a
prerequisite merges. Independent PRs targeting the integration branch are not
part of that retargeting chain.

## Recovery rules

- Prefer recoverable local commits and branches over stashes or patch files.
- Never use `git reset --hard`, destructive checkout, or broad cleanup.
- Never delete source snapshots or verification refs as part of this workflow.
- If a command is interrupted, inspect local and remote state before retrying.
- Use `--force-with-lease` only when updating a branch owned by this workflow
  after an intentional local commit rewrite and explain why it is required.
