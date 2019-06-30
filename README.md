# Unity Addressable Importer
A simple rule based addressable asset importer.

The importer marks assets as addressable, by applying to files having a path matching the rule pattern.

## Install package

### Install as a git package

This is the recommended way to track update. Open Packages/manifest.json with your favorite text editor. Add the following line to the dependencies block.

    {
        "dependencies": {
            "com.littlebigfun.addressable-importer": "https://github.com/favoyang/unity-addressable-importer.git"
        }
    }

### Install as an embbed package via submodule

This way gives you more control if you want to modify the package based on your purpose. Fork the repo, and checkout to your Packages folder as submodule.

    git submodule add https://github.com/[YOURNAME]/unity-addressable-importer.git Packages/unity-addressable-importer
    git add -A
    git ci -m "Imported unity-addressable-importer as embbed package"

## How to use

See [usage](./Documentation~/AddressableImporter.md)
