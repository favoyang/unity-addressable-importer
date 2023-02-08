<p align="center">
  <img width="180" src="https://raw.githubusercontent.com/favoyang/unity-addressable-importer/master/Media~/icon-512.png" alt="logo">
</p>
<h1 align="center">Unity Addressable Importer</h1>
<p align="center">
  <a href="https://openupm.com/packages/com.littlebigfun.addressable-importer/">
    <img src="https://img.shields.io/npm/v/com.littlebigfun.addressable-importer?label=openupm&amp;registry_uri=https://package.openupm.com" />
  </a>
  <a href="#contributors">
    <img src="https://img.shields.io/badge/all_contributors-4-orange.svg?style=flat-square"/>
  </a>
</p>

A simple rule-based addressable asset importer. The importer marks assets as addressable, by applying to files having a path matching the rule pattern.

Table of Contents

- [Install Package](#install-package)
  - [Install via OpenUPM](#install-via-openupm)
  - [Install via Git URL](#install-via-git-url)
- [How to Use](#how-to-use)
- [Contributors ✨](#contributors-)
- [Media](#media)

## Install Package

### Install via OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.littlebigfun.addressable-importer
```

### Install via Git URL

Open *Packages/manifest.json* with your favorite text editor. Add the following line to the dependencies block.

```json
    {
        "dependencies": {
            "com.littlebigfun.addressable-importer": "https://github.com/favoyang/unity-addressable-importer.git"
        }
    }
```

Notice: Unity Package Manager records the current commit to a lock entry of the *manifest.json*. To update to the latest version, change the hash value manually or remove the lock entry to resolve the package.

```json
    "lock": {
      "com.littlebigfun.addressable-importer": {
        "revision": "master",
        "hash": "..."
      }
    }
```

## How to Use

See [usage](./Documentation~/AddressableImporter.md)

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="http://littlebigfun.com"><img src="https://avatars2.githubusercontent.com/u/125390?v=4?s=100" width="100px;" alt="Favo Yang"/><br /><sub><b>Favo Yang</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=favoyang" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/AlkisFortuneFish"><img src="https://avatars2.githubusercontent.com/u/43749706?v=4?s=100" width="100px;" alt="AlkisFortuneFish"/><br /><sub><b>AlkisFortuneFish</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=AlkisFortuneFish" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://www.insanegames.com.br"><img src="https://avatars0.githubusercontent.com/u/2972924?v=4?s=100" width="100px;" alt="Danilo Nishimura"/><br /><sub><b>Danilo Nishimura</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=danilonishi" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/K-Dark"><img src="https://avatars2.githubusercontent.com/u/44504098?v=4?s=100" width="100px;" alt="K-Dark"/><br /><sub><b>K-Dark</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=K-Dark" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://www.cnblogs.com/tudas"><img src="https://avatars0.githubusercontent.com/u/1911170?v=4?s=100" width="100px;" alt="caochao"/><br /><sub><b>caochao</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=caochao" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://light11.hatenadiary.com/"><img src="https://avatars0.githubusercontent.com/u/47441314?v=4?s=100" width="100px;" alt="Haruki Yano"/><br /><sub><b>Haruki Yano</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=Haruma-K" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://www.fireboltgames.com"><img src="https://avatars3.githubusercontent.com/u/123872?v=4?s=100" width="100px;" alt="Edwin Lyons"/><br /><sub><b>Edwin Lyons</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=eAi" title="Code">💻</a></td>
    </tr>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="http://greenbuttongames.com"><img src="https://avatars1.githubusercontent.com/u/7457166?v=4?s=100" width="100px;" alt="Mefodei"/><br /><sub><b>Mefodei</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=Mefodei" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://giuseppetoto.it"><img src="https://avatars.githubusercontent.com/u/6715157?v=4?s=100" width="100px;" alt="Giuseppe Toto"/><br /><sub><b>Giuseppe Toto</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=gtoto007" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://laicasaane.xyz"><img src="https://avatars.githubusercontent.com/u/1594982?v=4?s=100" width="100px;" alt="Laicasaane"/><br /><sub><b>Laicasaane</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=laicasaane" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/JVinceW"><img src="https://avatars.githubusercontent.com/u/11038182?v=4?s=100" width="100px;" alt="JVince"/><br /><sub><b>JVince</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=JVinceW" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/fantasyz"><img src="https://avatars.githubusercontent.com/u/3223242?v=4?s=100" width="100px;" alt="Nick Mok"/><br /><sub><b>Nick Mok</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/issues?q=author%3Afantasyz" title="Bug reports">🐛</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/KarolGu"><img src="https://avatars.githubusercontent.com/u/81965750?v=4?s=100" width="100px;" alt="KarolGu"/><br /><sub><b>KarolGu</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/issues?q=author%3AKarolGu" title="Bug reports">🐛</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://kotobank.ch/~vaartis"><img src="https://avatars.githubusercontent.com/u/14316128?v=4?s=100" width="100px;" alt="vaartis"/><br /><sub><b>vaartis</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=vaartis" title="Code">💻</a></td>
    </tr>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/teck124"><img src="https://avatars.githubusercontent.com/u/21362117?v=4?s=100" width="100px;" alt="Takumi Katsuraya"/><br /><sub><b>Takumi Katsuraya</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=teck124" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/ese9"><img src="https://avatars.githubusercontent.com/u/38282199?v=4?s=100" width="100px;" alt="Roman Novikov"/><br /><sub><b>Roman Novikov</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=ese9" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://blog.uzutaka.com"><img src="https://avatars.githubusercontent.com/u/525643?v=4?s=100" width="100px;" alt="Takanori Uzuka"/><br /><sub><b>Takanori Uzuka</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=takanori" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/vanifatovvlad"><img src="https://avatars.githubusercontent.com/u/26966368?v=4?s=100" width="100px;" alt="VladV"/><br /><sub><b>VladV</b></sub></a><br /><a href="https://github.com/favoyang/unity-addressable-importer/commits?author=vanifatovvlad" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!

## Media

Icons made by [Freepik](https://www.flaticon.com/authors/freepik) from [flaticon.com](http://www.flaticon.com)
