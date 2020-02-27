## [0.5.2](https://github.com/favoyang/unity-addressable-importer/compare/v0.5.1...v0.5.2) (2020-02-27)


### Bug Fixes

* **ci:** tag already exists error when pushing tag ([5c4373f](https://github.com/favoyang/unity-addressable-importer/commit/5c4373f8713186055cc1898539e0b476e2308a9c))
* **ci:** trigger github actions ([e7592d6](https://github.com/favoyang/unity-addressable-importer/commit/e7592d6c10cf788cbefc7ded82f677ce435376d4))
* bump semantic-release to v2.1.3 to solve dependencies issue ([c59b986](https://github.com/favoyang/unity-addressable-importer/commit/c59b9865c744b01cec73e7bc1013301c67c630ed))
* exclude MacOS .DS_Store file ([332a769](https://github.com/favoyang/unity-addressable-importer/commit/332a769ca88a20c856212167eb48c88ab3f20cee))

## [0.5.1](https://github.com/favoyang/unity-addressable-importer/compare/v0.5.0...v0.5.1) (2020-02-02)


### Bug Fixes

* bump semantic-release to v2.1.3 to solve dependencies issue ([c59b986](https://github.com/favoyang/unity-addressable-importer/commit/c59b9865c744b01cec73e7bc1013301c67c630ed))
* no warnings for missing AddressableImportSettings [#15](https://github.com/favoyang/unity-addressable-importer/issues/15) ([30dd56a](https://github.com/favoyang/unity-addressable-importer/commit/30dd56a0823e0f370d775acbce06736a92c0dafb))

# [0.5.0](https://github.com/favoyang/unity-addressable-importer/compare/v0.4.2...v0.5.0) (2020-01-10)


### Features

* deploy semantic-release ([d1c4c1b](https://github.com/favoyang/unity-addressable-importer/commit/d1c4c1b79117b4eab55021d2485c606a2d9f4974))

# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.4.2] - 2019-12-25
 - Updated context menu path to create settings.
 - Added alternative way to install the package via openupm.

## [0.4.1] - 2019-09-29
 - Updated contributors.

## [0.4.0] - 2019-09-29
 - Added option to apply group template.

## [0.3.2] - 2019-09-20
 - Bug fix: internal editor script enables multiselect edition on all MonoBehaviour and ScriptableObjects.

## [0.3.1] - 2019-09-12
 - Added media files.

## [0.3.0] - 2019-08-08
 - Added context menu for faster asset import/update.

## [0.2.2] - 2019-08-05
 - Fixed simplified behavior.
 - Added unit tests.

## [0.2.1] - 2019-08-04
 - Added address/group replacement. Working with regex rule to build the address based on information captured from the path.
 - Added path elements extraction for address/group replacement (i.e. `${PATH[0]}`)
 - Added option to automatically create groups if not exist, default False.
 - Added option to remove empty Asset Groups except the default group, default False.
 - Added option to define if labels from ruleset are added or replace the current ones.
 - Added save, documentation buttons.
 - Improved documentation.

## [0.1.1] - 2019-07-19
 - Updated to addressable v1.1.5.

## [0.1.0-preview] - 2019-06-30
 - Initial submission for package distribution.
