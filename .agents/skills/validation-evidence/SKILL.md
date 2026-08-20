---
name: validation-evidence
description: Collect and classify validation evidence for engineering artifacts without inferring unobserved test or manual verification results.
---

# Validation Evidence

Collect validation evidence from observable command or check results first. When
relevant validation may have happened outside the current session and the user
could reasonably know it, ask one concise, consolidated question before
finalizing the artifact. Request only details applicable to the change, such as:

- the manual procedure and observed result;
- the scene, prefab, fixture, platform, or input device exercised;
- the Unity version or visual or performance evidence when material.

Do not interrupt for trivial changes, when sufficient evidence is already
available, or when validation is irrelevant. Treat the question as optional:
never infer a result from silence, test code, changed files, or an intended
procedure. Distinguish evidence present in the change from checks actually run.

If no execution evidence is available after this collection step, use the
artifact's explicit unknown or not-run state. Preserve failed results and their
disposition instead of summarizing validation as successful.
