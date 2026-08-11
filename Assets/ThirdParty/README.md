# Third-Party Assets

This directory contains assets maintained outside this repository. Its documentation records only provenance and licensing information; usage details belong to the feature or experiment that consumes each asset.

## Organization Standard

Each dependency must remain under its own unchanged root directory:

```text
Assets/ThirdParty/
├── README.md
├── Vendor Asset Name/
└── Another Vendor Asset/
```

When adding an asset:

1. Move its complete root directory into `Assets/ThirdParty/` from the Unity Editor.
2. Preserve its internal structure and all `.meta` files.
3. Do not edit vendor files directly. Place project-specific variants under the relevant project or experiment directory.
4. Keep imports and vendor updates isolated in their own commit whenever practical.
5. Add one row to the dependency table when provenance or licensing is not already documented by the asset itself.

> [!NOTE]
> **When to use `Packages/`**
>
> Keep imported art, audio, and project-specific assets under `Assets/ThirdParty/` by default. Use a Unity Package Manager package when a dependency is a reusable, independently versioned module—typically code, editor tooling, or a shared library—with its own `package.json` and clear ownership.
>
> Do not move an existing asset folder into `Packages/` only for organization. The migration can break GUID-based references, complicate Asset Store updates, and introduce licensing or redistribution concerns. If packaging is justified, preserve metadata, verify references and licenses, and test the migration in an isolated commit.

## Dependencies

| Asset | Publisher | Source | License |
| --- | --- | --- | --- |
| RPG Tiny Hero Duo PBR Polyart | DM Dungeon Mason | [Unity Asset Store](https://assetstore.unity.com/packages/3d/characters/humanoids/rpg-tiny-hero-duo-pbr-polyart-225148) | [Standard Unity Asset Store EULA](https://unity.com/legal/as-terms), Extension Asset |
