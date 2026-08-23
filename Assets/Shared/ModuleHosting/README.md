# Module Hosting

Shared infrastructure for hosting Unity gameplay modules behind a small,
fault-contained lifecycle boundary.

## Lifecycle

Create the host with a module factory and failure handler, then compose it once:

```csharp
var host = new ModuleHost<MyModule>(CreateModule, HandleFailure);
host.Compose();
```

When composition succeeds, the host forwards `Enable`, `Disable`, and `Tick`
to the module. A failure during composition or lifecycle execution moves the
host to `Faulted` and reports a `ModuleFailure` through the configured handler.

Failed lifecycle operations are isolated from the caller where possible. The
host also attempts to disable a module after an `Enable` or `Tick` failure.

`Compose` is intended to be called once. Calls made after the host leaves the
`Uninitialized` state are ignored.

## Flow

```mermaid
flowchart LR
    classDef state fill:#1f2937,stroke:#94a3b8,color:#f8fafc,stroke-width:1px
    classDef ready fill:#14532d,stroke:#4ade80,color:#f0fdf4,stroke-width:2px
    classDef fault fill:#7f1d1d,stroke:#f87171,color:#fef2f2,stroke-width:2px
    classDef action fill:#1e3a5f,stroke:#60a5fa,color:#eff6ff
    classDef handler fill:#4c1d95,stroke:#c084fc,color:#faf5ff

    subgraph Lifecycle[ModuleHost lifecycle]
        U([Uninitialized]):::state
        C[Compose]:::action
        R([Ready]):::ready
        O[Enable / Tick / Disable]:::action
        F([Faulted]):::fault
    end

    subgraph Failure[Failure handling]
        D[Attempt local Disable]:::action
        N[Create ModuleFailure]:::action
        H[Failure handler]:::handler
    end

    U --> C
    C -->|success| R
    C -->|exception| N
    R --> O
    O -->|success| R
    O -->|Enable or Tick exception| D
    O -->|Disable exception| N
    D --> N
    N --> F
    F --> H
```
