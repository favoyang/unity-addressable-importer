# Unity Addressable Importer
A simple rule based addressable asset importer.

The importer marks assets as addressable, by applying to files having a path matching the rule pattern.

## Usage

You should create a single AddressableImportSettings file located at `Assets/AddressableAssetsData/AddressableImportSettings.asset`. To create it, go to `Assets/AddressableAssetsData` folder, right click in your project window and choose `Create > Addressable Assets > Import Settings`.

If no settings file exists, an empty one will be created when importing any new asset.

Once the settings file selected, you can edit rules in the inspector window. Then click `File > Save Project` to apply the changes.

![AddressableImportSettings Insepctor](./Documentation~/AddressableImportSettings-Insepctor.png)

Create a rule
- Path, the path pattern
- Match type
  - Wildcard, `*` matches any number of characters, `?` matches a single character
  - Regex
- Group name, leaves blank for the default group
- Labels, the labels to add
- Simplified, simplify address to filename without extension

Rule Examples

| Type     | Example             |
|----------|---------------------|
| Wildcard | Asset/Sprites/Icons |
| Wildcard | Asset/Sprites/Level??/*.asset |
| Regex    | ^Assets/Models/.*\\.fbx |

Notices for moved or re-imported assets
- The importer will not override existing labels.
- The importer will only override address if it looks like a path (starts with `Assets/`). In another word, if you changed or simplified the address, then reimport or move it, the address remains no change.
