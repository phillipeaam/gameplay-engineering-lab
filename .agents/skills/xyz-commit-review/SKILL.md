---
name: xyz-commit-review
description: Generate a structured XYZ Git commit message from currently staged changes for review without creating a commit.
---

# XYZ Commit Review

When invoked:

1. Read `SPEC.md` in this skill directory.
2. Inspect only the currently staged Git changes.
3. Analyze the staged diff and repository context when necessary.
4. Generate the commit message according to the XYZ specification.
5. Never invent motivations, tests, validation, references, or performance results.
6. Do not modify files.
7. Do not create the commit.
8. Output only the final commit message.
