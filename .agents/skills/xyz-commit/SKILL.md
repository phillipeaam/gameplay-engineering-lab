---
name: xyz-commit
description: Generate an XYZ Git commit message and commit the currently staged changes using that message.
---

# XYZ Commit

When invoked:

1. Read `../xyz-commit-review/SPEC.md`.
2. Inspect only the currently staged Git changes.
3. If there are no staged changes, report that no commit was created.
4. Generate the commit message according to the XYZ specification.
5. Never invent motivations, tests, validation, references, or performance results.
6. Preserve unstaged changes; commit only the staged changes.
7. Create the Git commit using exactly the generated message.
8. Report the resulting commit hash and message.
