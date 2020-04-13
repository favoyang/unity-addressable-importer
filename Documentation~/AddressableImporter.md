<h1 align="center">Addressable Importer Usage</h1>

Table of Contents
- [Setup the Importer](#setup-the-importer)
- [Define Rules](#define-rules)
- [Rule Examples](#rule-examples)
- [Group Replacement](#group-replacement)
- [Address Replacement](#address-replacement)
- [Notice for moved or re-imported assets](#notice-for-moved-or-re-imported-assets)
- [Quick assets import](#quick-assets-import)
- [About prefab mode](#about-prefab-mode)

## Setup the Importer

You should create a single AddressableImportSettings file located at `Assets/AddressableAssetsData/AddressableImportSettings.asset`. To create it, go to `Assets/AddressableAssetsData` folder, right click in your project window and choose `Create > Addressable Assets > Import Settings`.

![AddressableImportSettings Create](AddressableImportSettings-Create.png)

Once the settings file selected, you can edit rules in the inspector window. Then click `File > Save Project` to apply the changes.

![AddressableImportSettings Inspector](AddressableImportSettings-Insepctor.png)

## Define Rules

- `Path`, the path pattern.
- `Match Type`
  - Wildcard, `*` matches any number of characters, `?` matches a single character.
  - Regex.
- `Group Name`, leaves blank for the default group. For dynamic group see [Group Replacement](#group-replacement).
- `Label Mode`, defines if labels will be added or replaced.
- `Label Refs`, the labels to add.
- `Address Simplified`, simplify address to filename without extension.
- `Address Replacement`, leaves blank to use asset path as address. For dynamic address see [Address Replacement](#address-replacement).

## Rule Examples

| Type     | Example                                                                         |
|----------|---------------------------------------------------------------------------------|
| Wildcard | `Asset/Sprites/Icons`                                                           |
| Wildcard | `Asset/Sprites/Level??/*.asset`                                                 |
| Regex    | `^Assets/Models/.*\.fbx`                                                        |
| Regex    | `Assets/Weapons/(?<prefix>(?<category>[^/]+)/(.*/)*)(?<asset>.*_Data.*\.asset)` |

## Group Replacement

Dynamic group is supported by replacing `${name}` with extracted value from asset path, via the use of regex capture groups. Named capture groups can be referred to in `Group Name` via `${group}`. If groups are not named, they can be referred to numerically, via `$1`, `$2` and so on. For more information, refer to [Microsoft Docs - Substitutions in Regular Expressions](https://docs.microsoft.com/en-us/dotnet/standard/base-types/substitutions-in-regular-expressions). This only works for match type Regex.

For convenience, path elements can be referred via `${PATH[index]}`. This works for all match types.

| Asset Path               | Path                                          | Group Name               | Result         |
|--------------------------|-----------------------------------------------|--------------------------|----------------|
| `Assets/Sprites/cat.png` | `Assets/Sprites/*.png`                        | `${PATH[1]}`             | Sprites        |
| `Assets/Sprites/cat.png` | `Assets/Sprites/*.png`                        | `${PATH[-1]}`            | Sprites        |
| `Assets/Sprites/cat.png` | `Assets/Sprites/*.png`                        | `${PATH[1]}-Group`       | Sprites-Group  |
| `Assets/Sprites/cat.png` | `Assets/Sprites/*.png`                        | `${PATH[0]}-${PATH[1]}`  | Assets-Sprites |
| `Assets/cat/cat01.png`   | `Assets/(?<category>[^/]+)/(?<asset>.*)\.png` | `${PATH[0]}:${category}` | Assets:cat     |

## Address Replacement
![AddressableImportSettings Inspector Regex](AddressableImportSettings-Insepctor2.png)

Similar to [Group Replacement](#group-replacement), address replacement is also supported.

| Asset Path             | Path                                          | Address Replacement               | Result           |
|------------------------|-----------------------------------------------|-----------------------------------|------------------|
| `Assets/cat/cat01.png` | `Assets/(?<category>[^/]+)/(?<asset>.*)\.png` | `${category}-${asset}`            | cat-cat01        |
| `Assets/cat/cat01.png` | `Assets/(?<category>[^/]+)/(?<asset>.*)\.png` | `${PATH[0]}:${category}-${asset}` | Assets:cat-cat01 |

## Notice for moved or re-imported assets
- The importer always overrides existing labels if `LabelMode = Replace`.
- The importer always overrides existing address if
  - The address looks like a path (starts with `Assets/`).
  - `Address Simplified` is ticked.
  - `Address Replacement` is in use.
  - In another word, if you intent to manually change the address later, leave `Address Simplified` unticked, `Address Replacement` blank, and do not use `Assets/` prefix for the customized address name.

## Quick assets import

The importer will apply your rules whenever an asset being imported, moved or deleted. However you may want to apply new or modified rules to existing assets. To quickly apply importer rules, select target folder(s) in project view, right click to open the context menu, and click `AddressablesImporter: Check Folder(s)`. The action is more efficient than reimport asset(s).

![AddressableImport Context Menu](AddressableImportSettings-ContextMenu.png)

## About prefab mode

When both prefab mode (the preview scene for editing a prefab) and the auto save feature are enabled, every modification will cause the asset to be saved and trigger the importer, leads to slow response. For performance reason, the importer will ignore current editing asset.
