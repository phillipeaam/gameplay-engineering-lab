---
name: prepare-pr-set
description: Split a large staged, unstaged, or mixed working tree into a safe set of isolated worktrees, small branches, approved XYZ commits, and approved draft pull requests, targeting the integration branch directly when packages are independent and stacking only genuine dependencies. Use when Codex needs to execute an end-to-end PR split, decompose a mega PR, recover reviewable packages from accumulated local changes, design a mixed direct-and-stacked PR graph, preserve a WIP snapshot, or prove that every intended change is represented without unrelated work.
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
the PR-set workflow, authorizes inventory, package design, and preparation of
the proposed worktree plan. It does not by itself authorize creating commits,
pushing branches, or creating pull requests.

Obtain explicit confirmation at these checkpoints:

1. Before creating the local WIP snapshot commit and package worktrees, present
   the package boundaries, branch names, bases, worktree paths, dependencies,
   and known incomplete or failing state.
2. After a package has been isolated and reviewed, ask whether that specific
   package may be committed. Report its diff scope, validation result, known
   failures, and incomplete work. The user may approve some packages and leave
   others uncommitted in their worktrees.
3. After approved commits and aggregate verification, ask which branches may be
   pushed and which draft PRs may be created. Commit approval does not imply
   publication approval.

Confirmation may cover multiple explicitly listed packages, but never infer
approval for an unlisted package or a later checkpoint. Do not commit or
publish a package the user identifies as incomplete or broken. Preserve it in
its worktree for continued work.

When this skill triggers implicitly or the user asks only for analysis, review,
explanation, or a proposed strategy, remain read-only and request authorization
before creating snapshots, branches, or worktrees.

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

The snapshot is a recovery artifact and may intentionally preserve incomplete
or broken work. Explain that distinction and obtain the first authority
checkpoint before creating it. Never present the snapshot as a delivery commit
or publish its branch.

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

### 4. Create a worktree and build every branch from its selected base

For each package:

1. Confirm the selected base ref exists and the source worktree remains
   recoverable. Do not switch the source worktree away from its snapshot.
2. Choose a package-specific sibling or dedicated worktree path. Prefer a
   recognizable repository-and-package name over a generic reusable name.
3. Create a specifically named branch and attach it to its own worktree from
   the selected base. A dependent package may start from its prerequisite
   branch after that prerequisite commit is approved and exists.
4. Apply only its approved files or hunks from the source snapshot.
5. Preserve required Unity `.meta` pairing and GUID continuity.
6. Review the package with `project-quality`.
7. Run the narrowest meaningful validation and record observed results.
8. Inspect the complete package diff and report untracked, staged, and unstaged
   state. Do not stage merely to imply readiness.
9. Reach the package commit checkpoint. Invoke `xyz-commit` only after the user
   explicitly approves committing that package.
10. Confirm the worktree state after an approved commit, or leave the package
    uncommitted and intact when approval is withheld.

Default to one cohesive commit per branch. Use more only when the user approves
a reason that improves independent review or rollback.

Keep package worktrees while their work is incomplete or their PRs remain open
so fixes and review changes stay isolated. Do not reuse an existing worktree,
remove a worktree, or delete its branch without explicit authorization.

For Unity projects, first follow the repository's Editor-connection policy.
Avoid opening multiple Editors merely because multiple worktrees exist. Account
for each worktree's separate `Library` and import cost when selecting which
package receives Unity validation.

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

Prefer a dedicated temporary verification worktree so package and source
worktrees remain untouched. Keep its branch local and do not remove it without
the same recovery care applied to other workflow artifacts.

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

After presenting the verified branch list and receiving the publication
checkpoint approval for explicitly identified packages:

1. Push package branches, never snapshot or verification branches.
2. Invoke `prepare-pull-request` for every package.
3. Target each PR at the base selected during dependency analysis.
4. Link the immediate prerequisite PR only when a dependency edge exists.
5. State `None` for related work when an independent PR has no known link.
6. Keep every PR in draft status unless the user explicitly requests otherwise.

Do not push or create a PR for an unapproved, uncommitted, incomplete, broken,
or validation-blocked package. Leave its worktree available and report what
must be completed or decided before reconsidering publication.

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
- Never remove package worktrees automatically after commit or PR creation;
  retain them until the user authorizes cleanup, normally after merge.
- If a command is interrupted, inspect local and remote state before retrying.
- Use `--force-with-lease` only when updating a branch owned by this workflow
  after an intentional local commit rewrite and explain why it is required.
