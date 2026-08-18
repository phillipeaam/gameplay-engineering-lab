---
name: prepare-pull-request
description: Prepare or review GitHub pull request titles and descriptions for this project. Use when Codex needs to draft, create, update, or assess a PR; summarize a branch for review; select validation evidence; identify Unity-specific risk; or check PR scope and metadata against the project quality standard.
---

# Prepare Pull Request

Prepare an evidence-based PR that makes one cohesive change easy to understand,
validate, review, and reverse.

## Authoritative inputs

1. Read `../project-quality/SKILL.md` and the quality modules it routes for
   the changed files. Treat that standard as authoritative for quality gates.
2. Read [pull-request-standard.md](references/pull-request-standard.md) for the
   PR contract, metrics, Unity guidance, and industry references.
3. Use `.github/pull_request_template.md` as the canonical body structure.

Do not copy quality rules into the PR when they are irrelevant to its risk.

## Workflow

1. Resolve the base and head branches, then inspect their complete diff,
   commits, existing PR metadata, and working-tree state.
2. Exclude unstaged or unrelated changes. If the branch mixes independent
   changes, recommend the smallest safe split before preparing the PR.
3. Classify the primary change type, domain scope, affected quality modules,
   user or developer impact, and risk.
4. Generate a Conventional Commit title:
   `<type>(<scope>): <result>`. Keep it concise and describe the outcome.
5. Fill only relevant template content. Remove comments, unused optional fields,
   empty bullets, and boilerplate before publishing.
6. Separate implementation evidence from execution evidence:
   - `Added` or `updated` describes files in the diff.
   - `Passed` requires an observed successful command or check result.
   - `Manually verified` requires an observed or user-provided procedure.
   - Otherwise state `Not run` with the reason.
7. For gameplay, UI, animation, scene, prefab, shader, input, physics, or
   platform-sensitive changes, include the applicable Unity verification from
   the reference. Never demand irrelevant fields.
8. Link issues or related work when available. Do not invent identifiers.
9. Call out material risks, compatibility concerns, migrations, generated or
   serialized assets, and the areas where reviewer attention is most valuable.
10. Create or update a draft PR only when the user authorizes publishing.
    Re-read the published title and body to verify the result.

## Output rules

- Prefer a brief PR that gives reviewers context the diff cannot preserve.
- Describe decisions and behavior; do not reproduce a file inventory.
- Scale detail to risk and scope. A trivial documentation PR may use a sentence;
  a cross-system gameplay change needs fuller context and validation.
- Do not claim coverage percentage, performance improvement, compatibility,
  build success, or test success without reproducible evidence.
- Treat size as a review signal, not a numeric pass/fail gate.
- Report missing evidence explicitly without presenting it as a defect.
- Preserve draft status while required validation or author decisions remain.
