---
name: xyz-commit
description: Generate structured XYZ Git commit messages from currently staged changes.
---

# XYZ Commit

When invoked:

1. Read `SPEC.md` in this skill directory.
2. Inspect only the currently staged Git changes.
3. Analyze the staged diff and repository context when necessary.
4. Generate the commit message according to the XYZ specification.
5. Never invent motivations, tests, validation, references, or performance results.
6. Do not modify files.
7. Do not create the commit unless explicitly requested.
8. Output only the final commit message.