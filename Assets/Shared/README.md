# Shared Assets

This directory contains project-owned assets intentionally reused by multiple features or experiments.

## Organization Standard

Group shared content by its source or domain, then by asset type:

```text
Assets/Shared/
├── README.md
├── Vendor Asset Name/
│   ├── Materials/
│   └── Prefabs/
└── Common/
    ├── Materials/
    └── Prefabs/
```

Use consistent type folders within each domain, such as `Materials`, `Prefabs`, `Animations`, and `Textures`.

## Guidelines

- Keep feature-specific assets with the feature that owns them.
- Add content here only after it is reused or deliberately designed for reuse.
- Store modified third-party variants here; never edit files under `Assets/ThirdParty/` directly.
- Reference original vendor textures, meshes, or animations by GUID when they do not require modification.
- Keep source-specific adaptations under a folder whose name matches the corresponding root in `Assets/ThirdParty/`; use `Common/` only for project-wide content with no vendor ownership.

## Third-Party Adaptations

When a vendor asset requires project-specific changes:

1. Duplicate materials that need a different render pipeline, shader, or configuration and update only the project-owned copies.
2. Create a Prefab Variant from the vendor prefab when the original hierarchy, rig, mesh, or Animator should remain inherited.
3. Apply converted materials and project components as overrides on the variant.
4. Duplicate the complete prefab only when it must evolve independently from the vendor hierarchy or inheritance would create fragile overrides.

Keep the adapted prefab with its owning feature until it has a second consumer or is intentionally designed as shared infrastructure. Move it to the source-specific folder under `Shared` only when reuse is established.

Use a render-pipeline suffix such as `_URP` only when multiple pipeline variants coexist. Otherwise, prefer a stable project-facing name.

> [!NOTE]
> `Shared` is an ownership and reuse boundary, not a default destination for assets whose final use is still unknown.
