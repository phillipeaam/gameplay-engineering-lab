---
name: xyz-commit
description: Generate an XYZ Git commit message and commit the currently staged changes using that message.
---

# XYZ Commit

When invoked:

1. Read `../xyz-commit-review/SPEC.md`.
2. Read `../validation-evidence/SKILL.md` and apply its collection rule
   before generating a full XYZ message. In small commit mode, ask only when
   validation is material to understanding the change.
3. Inspect only the currently staged Git changes.
4. If there are no staged changes, report that no commit was created.
5. Generate the commit message according to the XYZ specification.
6. Never invent motivations, tests, validation, references, or performance results.
7. Preserve unstaged changes; commit only the staged changes.
8. Create the Git commit using exactly the generated message.
9. Report the resulting commit hash and message.
