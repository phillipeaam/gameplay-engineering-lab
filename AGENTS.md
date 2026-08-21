# Project Instructions

## Unity workflow

When executing, testing, inspecting, or modifying Unity content:

1. First determine whether a Unity Editor instance is already open for this project.
2. If an Editor instance is open, prioritize:
   - Unity MCP to control the connected Editor;
   - Unity CLI connected to the existing Editor, when applicable.
3. Do not start a new Unity Editor instance before attempting Unity MCP and Unity CLI.
4. A project lock indicating that Unity is already open is not, by itself, a blocker. Treat it as a signal to locate and use the existing Editor instance.
5. Start a separate batch or headless Unity process only when:
   - no connected Editor exists;
   - Unity MCP and Unity CLI are unavailable or have failed;
   - or the user explicitly requests a separate instance.
6. Always include `-automated` when starting a new Unity Editor process.
7. Never remove the Unity project lock or terminate the user's Editor without explicit authorization.
8. If Unity MCP or Unity CLI fails, report which integration was attempted and the exact failure before using a fallback.
9. When Unity CLI reports no reachable instance inside a sandbox but a project
   lock or `unity pipeline list` shows a running Editor, repeat the connection
   check outside the sandbox before concluding that the Pipeline is unavailable.
   Sandboxed localhost restrictions can hide an otherwise healthy Pipeline
   server. In particular, compare `unity status --format json` and a read-only
   `unity command` listing outside the sandbox; do not restart or terminate the
   Editor merely because the sandboxed check returned `STATUS_NO_INSTANCES` or
   `COMMAND_FAILED`.

Use this fallback order:

1. Existing Editor through Unity MCP.
2. Existing Editor through Unity CLI.
3. Separate Unity batch or headless process with `-automated`.

## Python environment

When running Python, Python-based tools, tests, or scripts:

1. Inspect the repository for a project-managed virtual environment before using the system Python.
2. Prefer environments in this order when they exist:
   - `.venv.ai`;
   - `.venv`;
   - another virtual environment explicitly documented by the project.
3. Invoke tools through the selected environment directly, for example `.venv.ai/bin/python`, `.venv.ai/bin/pip`, or `.venv.ai/bin/pytest`. Do not assume that activating the environment in one shell command persists into later commands.
4. When a Python module or command is missing from the system environment, check the project virtual environments before reporting it as unavailable or attempting installation.
5. Do not install packages into the system Python when a project virtual environment is available.
6. If no suitable environment exists, report what paths were checked before proposing environment creation or dependency installation.

Use this fallback order:

1. `.venv.ai` when present.
2. `.venv` when present.
3. Another project-documented environment.
4. System Python only when no project environment applies.
