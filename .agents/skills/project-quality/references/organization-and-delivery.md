# Organization and Delivery Standard

## Contents

- [Ownership boundaries](#1-ownership-boundaries)
- [Packages and build configuration](#2-packages-and-build-configuration)
- [Documentation and learning value](#3-documentation-and-learning-value)
- [Pull request delivery](#4-pull-request-delivery)

## 1. Ownership boundaries

- Keep feature-specific assets with their owning experiment or feature.
- Promote content to `Assets/Shared/` only after intentional reuse or explicit
  shared ownership.
- Do not edit files under `Assets/ThirdParty/` directly. Keep project-owned
  variants outside the vendor tree and preserve provenance and licensing.
- Vendor imports and upgrades SHOULD be isolated when practical to simplify
  review and rollback.

## 2. Packages and build configuration

- Dependency additions and upgrades MUST be intentional, versioned, compatible
  with the current Unity editor, and reflected in the lockfile.
- Experimental packages MUST state why their risk is acceptable.
- Remove unused dependencies when evidence shows no remaining consumer.
- Changes to player, quality, input, physics, time, graphics, or build settings
  MUST document behavioral impact and receive relevant build/runtime validation.
- Secrets, credentials, signing material, and machine-specific absolute paths
  MUST NOT enter version control.

## 3. Documentation and learning value

- Each substantial experiment SHOULD document the problem, approach, gameplay
  example, architecture, trade-offs, when not to use it, validation, and lessons
  learned.
- Documentation MUST explain decisions that code alone cannot preserve.
- Public or shared contracts SHOULD document non-obvious preconditions,
  ownership, lifetime, side effects, and failure behavior.
- Documentation changes MUST accompany behavior changes when existing guidance
  would otherwise become false.

## 4. Pull request delivery

- Each pull request MUST represent one cohesive objective. Unrelated cleanup,
  broad refactoring, vendor imports, generated changes, or dependency upgrades
  SHOULD be separated when combining them obscures review or rollback.
- The title MUST describe the result using the repository's Conventional Commit
  form: `<type>(<scope>): <summary>`.
- The description MUST preserve the reason, important decisions, impact, risk,
  and validation status relevant to the change.
- Validation MUST distinguish checks present in the diff from checks actually
  executed. Missing execution MUST be reported as `Not run`, with a reason.
- Player-facing visual changes SHOULD include representative visual evidence
  when tests and the diff cannot communicate the result adequately.
- Authored or serialized Unity changes MUST identify the relevant scene, prefab,
  platform, input, settings, or reference risks when applicable.
- Pull request size is a review-cost signal, not a universal numeric gate.
  Large cohesive generated or serialized diffs require context; small changes
  still require validation proportional to risk.
- Use `.github/pull_request_template.md` for the canonical description structure
  and `.agents/skills/prepare-pull-request/references/pull-request-standard.md`
  for operating guidance, metrics, and authoritative industry references.
