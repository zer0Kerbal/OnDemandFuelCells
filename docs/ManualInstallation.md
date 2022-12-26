---
permalink: /ManualInstallation.html
title: Manual Installation
description: the flat-pack Kiea instructions, written in Kerbalese, unusally present
tags: installation,directions,page,kerbal,ksp,zer0Kerbal,zedK
---

<!-- ManualInstallation.md v1.1.8.1
On Demand Fuel Cells (ODFC)
created: 01 Oct 2019
updated: 29 Jul 2022 -->

<!-- based upon work by Lisias -->

# On Demand Fuel Cells (ODFC)

[Home](./index.md)

On Demand Fuel Cells (ODFC) is a plugin to simulate fuel cells in Kerbal Space Program (KSP), and do a better job of it than stock's use of a resource converter.  The main difference is it only generates electricity when it's really needed (batteries almost empty), and otherwise lets electricity of a craft float up and down, as it might in a solar powered vehicle when the sun is eclipsed by another celestial body.  It also allows fuel cells to generate byproducts, aimed at supporting life support mods like TACLS.

## Installation Instructions

### Using CurseForge/OverWolf app or CKAN

You should be all good! (check for latest version on CurseForge)

### If Downloaded from CurseForge/OverWolf manual download

To install, place the `OnDemandFuelCells` folder inside your Kerbal Space Program's GameData folder:

* **REMOVE ANY OLD VERSIONS OF THE PRODUCT BEFORE INSTALLING**
  * Delete `<KSP_ROOT>/GameData/OnDemandFuelCells`
* Extract the package's `OnDemandFuelCells/` folder into your KSP's GameData folder as follows:
  * `<PACKAGE>/OnDemandFuelCells` --> `<KSP_ROOT>/GameData`
    * Overwrite any preexisting folder/file(s).
  * you should end up with `<KSP_ROOT>/GameData/OnDemandFuelCells`

### If Downloaded from SpaceDock / GitHub / other

To install, place the `GameData` folder inside your Kerbal Space Program folder:

* **REMOVE ANY OLD VERSIONS OF THE PRODUCT BEFORE INSTALLING**
  * Delete `<KSP_ROOT>/GameData/OnDemandFuelCells`
* Extract the package's `GameData` folder into your KSP's root folder as follows:
  * `<PACKAGE>/GameData` --> `<KSP_ROOT>`
    * Overwrite any preexisting file.
  * you should end up with `<KSP_ROOT>/GameData/OnDemandFuelCells`

## The following file layout must be present after installation

```markdown
<KSP_ROOT>
  + [GameData]
    + [OnDemandFuelCells]
      + [Agencies]
        ...
      + [Compatibility]
        ...
      + [Config]
        ...
      + [Flags]
        ...
      + [Localization]
        ...
      + [Plugins]
        ...
      * #.#.#.#.htm
      * Attributions.htm
      * changelog.md
      * GPL-2.0.txt
      * ManualInstallation.htm
      * OnDemandFuelCells.version
      * readme.htm
    ...
    * [Module Manager /L][mml] or [Module Manager][mm]
    * ModuleManager.ConfigCache
  * KSP.log
  ...
```

### Dependencies

* *either*
  * [Module Manager /L][mml]
  * [Module Manager][mm]

[mm]: https://forum.kerbalspaceprogram.com/index.php?/topic/50533-*/ "Module Manager"
[mml]: https://github.com/net-lisias-ksp/ModuleManager "Module Manager /L"
