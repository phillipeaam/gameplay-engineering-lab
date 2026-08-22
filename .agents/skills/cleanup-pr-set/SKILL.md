---
name: cleanup-pr-set
description: Safely retire local branches and worktrees after their pull requests merge, while preserving dirty, unmerged, verification, and WIP recovery state. Use after a prepared PR set has been partially or fully merged and local package worktrees or snapshot branches remain.
---

# Cleanup PR Set

Remove local artifacts that are provably obsolete without losing incomplete or
unmerged work. Treat package cleanup, verification cleanup, remote cleanup, and
WIP cleanup as distinct decisions.

## Authority

An explicit invocation of `$cleanup-pr-set`, or an explicit request to clean up
a merged PR set, authorizes removal of local package worktrees and local package
branches only when every safety check below passes. Present the resolved cleanup
set before removing anything.

It does not authorize:

- discarding staged, unstaged, or untracked work;
- deleting a WIP snapshot or its worktree;
- deleting remote branches;
- closing, merging, or modifying pull requests;
- force-deleting a package branch that is not proven merged.

Obtain separate explicit confirmation for WIP deletion and remote branch
deletion. When the skill triggers implicitly or the user requests only an audit
or proposal, remain read-only.

## 1. Inventory local and remote state

- Record the current branch, `git status --short --branch`,
  `git worktree list --porcelain`, local branches and upstreams, and the
  integration branch.
- Fetch before deciding that a commit is merged. Do not mutate worktrees during
  inventory.
- Identify package branches, worktree paths, PRs, source WIP refs, and
  local-only verification refs. Do not infer ownership from a generic name.
- Inspect staged, unstaged, and untracked state separately in every candidate
  worktree.

If authentication, network access, or repository identity prevents reliable PR
or remote inspection, stop before cleanup and report the exact blocker.

## 2. Prove each package is safe to retire

A package worktree and local branch may be removed only when all are true:

1. Its PR is confirmed `MERGED`, not merely closed.
2. Its PR head commit is reachable from the fetched remote integration branch.
3. Its worktree has no staged, unstaged, or untracked changes.
4. Its local branch has no commits absent from both its published head and the
   integration branch.
5. The resolved branch and path are the exact intended package.

Classify every candidate as `safe`, `retain`, or `blocked`, with evidence.
Never treat a dirty worktree as safe because its PR merged.

## 3. Present and execute package cleanup

Before mutation, list each PR, merge state, local branch, worktree path, worktree
status, ancestry result, and proposed action or reason for retention.

For each `safe` package covered by the explicit invocation:

1. Run `git worktree remove <exact-path>` without `--force`.
2. Run `git branch -d <exact-branch>`.
3. Re-read worktree and branch state before continuing.

If either deletion refuses, stop for that package and diagnose the new state.
Do not escalate to `--force`, `git branch -D`, manual directory removal, or
worktree metadata deletion. Never recursively remove a directory as a substitute
for `git worktree remove`.

## 4. Retire verification artifacts separately

A local-only verification worktree and branch are not ordinary package refs.
They may be removed only when:

- the worktree is clean;
- they are positively identified as generated for this PR set;
- every package commit they assembled is reachable from the fetched integration
  branch;
- they contain no unique commit or tree content requiring preservation.

Present this proof and the exact verification paths before removal. When safe,
remove the worktree without force and delete the branch with `git branch -d`.
Retain verification artifacts when the set is only partially merged, their
provenance is ambiguous, or Git refuses safe deletion. Never publish them.

## 5. Audit the WIP snapshot separately

Never delete the source WIP snapshot during ordinary package or verification
cleanup.

After those steps, report:

- the WIP branch and worktree path;
- whether it is the only committed recovery point for a remaining package;
- which WIP changes are reachable from the integration branch;
- which changes survive only in unmerged branches or dirty worktrees;
- whether the WIP tree can be reconstructed from integration plus retained
  package state.

If any remaining package is uncommitted, treat the WIP as valuable recovery
evidence and recommend retaining it. A dirty worktree is not equivalent to a
committed backup.

Ask one explicit question naming the exact WIP branch and worktree before any
WIP removal. State whether deletion requires force because the snapshot is not
merged and exactly which recovery capability will be lost.

Only after explicit confirmation:

1. Recheck that the WIP worktree is clean.
2. Remove the exact WIP worktree when it is linked. Never remove the user's
   primary repository directory as a worktree-cleanup operation.
3. Delete the exact WIP branch. Use force only when required for that confirmed
   unmerged WIP ref, never for package branches or a pattern.
4. Verify that retained incomplete package worktrees are unchanged.

## 6. Handle remote branches separately

Do not delete remote branches merely because their PRs merged. If separately
authorized, resolve the exact merged heads again, exclude WIP and verification
refs, delete only those named remote branches, and report the result.

## 7. Report final state

Report removed local worktrees and branches; retained dirty, unmerged, or
blocked packages; verification artifacts removed or retained; WIP state and
decision; remote branches removed or untouched; and any failed command with the
state left behind.

Do not describe cleanup as complete while an authorized safe removal failed or
before re-reading the resulting state.
