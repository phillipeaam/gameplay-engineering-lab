#!/usr/bin/env bash

# Fail on the first command error, on unset variables, and on failures hidden
# inside pipelines. These options prevent a partial verification from being
# reported as successful.
set -euo pipefail

# The first pair optionally proves aggregate completeness. Every remaining pair
# describes one proposed PR as: <head branch> <declared base branch>.
usage() {
    echo "Usage: $0 <source-snapshot|-> <assembled-ref|-> <branch-1> <base-1> [<branch-2> <base-2> ...]" >&2
}

# Require the two aggregate arguments plus at least one branch/base pair. After
# those first two arguments, an odd count means a branch is missing its base.
if (( $# < 4 )) || (( ($# - 2) % 2 != 0 )); then
    usage
    exit 2
fi

# Keep the aggregate verification refs, then remove them from the positional
# arguments so the loop below can consume only branch/base pairs.
source_snapshot=$1
assembled_ref=$2
shift 2

# Stop early when the command is executed outside a Git working tree. Standard
# error output remains visible, while the successful repository path is hidden.
git rev-parse --is-inside-work-tree >/dev/null

# Verification must observe committed trees only. Local staged, unstaged, or
# untracked files could otherwise make the result ambiguous or non-reproducible.
if [[ -n $(git status --porcelain) ]]; then
    echo "Error: working tree is not clean." >&2
    exit 1
fi

# Resolve every supplied name as a commit before using it in ancestry or diff
# operations. The ^{commit} suffix also accepts tags that ultimately point to a
# commit, while rejecting paths and other non-commit Git objects.
verify_ref() {
    local ref=$1

    if ! git rev-parse --verify --quiet "${ref}^{commit}" >/dev/null; then
        echo "Error: ref '$ref' does not resolve to a commit." >&2
        exit 1
    fi
}

# Aggregate comparison is optional, but its two inputs are inseparable: the
# original WIP snapshot and the locally assembled result must be supplied
# together. A dash in both positions disables only this aggregate check.
if [[ "$source_snapshot" == "-" && "$assembled_ref" != "-" ]] ||
    [[ "$source_snapshot" != "-" && "$assembled_ref" == "-" ]]; then
    echo "Error: source snapshot and assembled ref must both be provided or both be '-'." >&2
    exit 1
fi

# Validate aggregate refs once before checking individual PR relationships.
if [[ "$source_snapshot" != "-" ]]; then
    verify_ref "$source_snapshot"
    verify_ref "$assembled_ref"
fi

# Consume each proposed PR independently. This supports a mixed graph where
# some branches target the integration branch and others target prerequisite
# package branches.
while (( $# > 0 )); do
    branch=$1
    base=$2
    shift 2

    verify_ref "$branch"
    verify_ref "$base"

    # A PR head must descend from its declared base. Failing this check usually
    # means the PR was based on the wrong branch or the dependency graph is stale.
    if ! git merge-base --is-ancestor "$base" "$branch"; then
        echo "Error: '$branch' does not descend from '$base'." >&2
        exit 1
    fi

    # The current workflow intentionally creates one cohesive commit per package.
    # Count commits reachable from the head but not from its declared base.
    commit_count=$(git rev-list --count "${base}..${branch}")

    if [[ "$commit_count" != "1" ]]; then
        echo "Error: '$branch' contains $commit_count commits over '$base'; expected 1." >&2
        exit 1
    fi

    # Check only the PR-shaped diff. Git reports whitespace errors and conflict
    # markers through a non-zero exit code, which strict mode propagates.
    git diff --check "${base}...${branch}"
    echo "Verified: $base -> $branch (1 commit, clean diff)."
done

# A verification branch may assemble several independent PR roots and terminal
# stack branches. Comparing its tree with the source snapshot proves that the
# partition preserved every intended change without adding extra content.
if [[ "$source_snapshot" != "-" ]]; then
    if ! git diff --quiet "$source_snapshot" "$assembled_ref"; then
        echo "Error: assembled ref '$assembled_ref' does not match source snapshot '$source_snapshot'." >&2
        exit 1
    fi

    echo "Verified: assembled tree matches '$source_snapshot'."
fi

# Reaching this line means every requested structural and completeness check
# completed successfully.
echo "PR set verification passed."
