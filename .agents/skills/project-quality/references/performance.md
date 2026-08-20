# Performance and Memory Standard

For allocation-specific review, read
[allocation-analysis.md](allocation-analysis.md) in addition to this module.
It defines the static heuristics, execution-path classification, evidence
protocol, and graduated finding severity for managed allocations and GC pressure.

- Do not claim a performance improvement without comparative measurements.
- Define the target platform, representative scenario, metric, and budget before
  treating optimization as complete.
- Profile in the Editor for diagnosis only; final performance conclusions MUST
  use a development build on a representative target platform/device.
- Treat `Update`, `LateUpdate`, `FixedUpdate`, rendering callbacks, tight loops,
  and high-frequency event paths as potential hot paths, then confirm by
  measurement.
- Established hot paths MUST avoid unnecessary managed allocations, repeated
  lookups, closure creation, boxing, and algorithmically avoidable work.
- Pooling, caching, jobs, Burst, or data-oriented structures MUST be justified
  by evidence and include lifecycle/ownership costs in the decision.
- Native containers and unmanaged resources MUST be disposed by their owner.
- Performance regressions MUST include before/after evidence captured under
  comparable conditions.
