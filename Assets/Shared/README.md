# Shared Assets

This directory contains project-owned assets intentionally reused by multiple features or experiments.

Shared vendor adaptations belong under `Assets/Shared/ThirdParty/`, grouped by
their original source. The original vendor packages remain under
`Assets/ThirdParty/`.

## Organization Standard

Group shared content by its source or domain, then by asset type:

```text
Assets/Shared/
├── README.md
├── ThirdParty/
│   └── Vendor Asset Name/
└── Common/
    └── Project-wide Feature or Infrastructure/
```

Use consistent type folders within each domain, such as `Scripts`, `Materials`,
`Prefabs`, `Animations`, and `Textures`.

## Guidelines

- Keep feature-specific assets with the feature that owns them.
- Add content here only after it is reused or deliberately designed for reuse.
- Keep original vendor content under `Assets/ThirdParty/` and project-owned adaptations under `Assets/Shared/ThirdParty/`, using a folder name that matches the corresponding vendor root.
- Never edit files under `Assets/ThirdParty/` directly when a project-owned adaptation is needed; duplicate or create the adaptation in the appropriate `Assets/Shared/ThirdParty/` folder.
- Reference original vendor textures, meshes, or animations by GUID when they do not require modification.
- Use `Common/` only for project-wide content with no vendor ownership.

## Third-Party Adaptations

When a vendor asset requires project-specific changes, keep the result under
the corresponding vendor folder in `Assets/Shared/ThirdParty/`:

1. Duplicate materials that need a different render pipeline, shader, or configuration and update only the project-owned copies.
2. Create a Prefab Variant from the vendor prefab when the original hierarchy, rig, mesh, or Animator should remain inherited.
3. Apply converted materials and project components as overrides on the variant.
4. Duplicate the complete prefab only when it must evolve independently from the vendor hierarchy or inheritance would create fragile overrides.

Use a render-pipeline suffix such as `_URP` only when multiple pipeline variants coexist. Otherwise, prefer a stable project-facing name.

> [!NOTE]
> `Shared` is an ownership and reuse boundary for project-owned content. Use `Shared/ThirdParty` for project-owned vendor adaptations and `Shared/Common` for project-wide infrastructure with no vendor ownership.
