<!-- Readme.md v1.2.0.0
On Demand Fuel Cells Refueled
created: 17 Jul 18
updated: 29 Nov 19 -->

<!-- Download on SpaceDock here or Github here.
Also available on CKAN. -->

# On Demand Fuel Cells Refueled (ODFCr)
![Mod Version](https://img.shields.io/github/v/release/zer0Kerbal/ODFCr?include_prereleases) 
![KSP 1.7.x](https://img.shields.io/badge/KSP%20version-1.7.x-66ccff.svg?style=flat-square) 
![CKAN listed](https://img.shields.io/badge/CKAN-Indexed-brightgreen.svg) 
![Software License GPLv3](https://img.shields.io/badge/SoftwareLicense-GPLv3-red) 
![NonSoftware License CC 4.0 BY-NC-SA](https://img.shields.io/badge/NonSoftwareLicense-CC--4.0--BY--SA-lightgrey)

#### Formerly known as On Demand Fuel Cells (ODFC)

zer0Kerbal adopts for curation a continuation of *On Demand Fuel Cells (ODFC)* by `**Orum**', now continued by the cooperative efforts of 4x4cheesecake, LinuxGuruGamer, and zer0Kerbal.

![ODFC PAW](https://i.postimg.cc/7Pj7gQQD/image.png)

On Demand Fuel Cells (ODFC) is a plugin to simulate fuel cells in Kerbal Space Program (KSP), and do a better job of it than stock's use of a resource converter.  The main difference is it only generates electricity when it's really needed (batteries almost empty), and otherwise lets electricity of a craft float up and down, as it might in a solar powered vehicle when the sun is eclipsed by another celestial body.  It also allows fuel cells to generate byproducts, aimed at supporting life support mods like TACLS.

The plugin requires a set of Module Manager patches to function, as it does not do anything unless integrated into a part. There are two different sets of patches available on CKAN or SpaceDock.
One set that copies the fuel cells from Stock, Universal Storage 2, Jatwaa Demolitions Co, and Solid Fuel Cells (soon more) and replaces the stock modules with ODFC with three modes (four if Community Resource Pack is installed correctly) of operation. Also adds a 0.5 EC/s multimode fuel cell to all stock command pods (easily disabled since in separate patch)
Another set that modifies the same set of parts instead of copying them.

### Features:
- adjustable fuel cell use - much more than just On/Off operation
- multiple fuel modes (serial usage - one mode at a time)
- variable activation threshold
- configurable to produce byproducts (so O+H2 = EC + H2O)
- very small memory footprint
- Brown and Black out protection assistance
- PAW (Part Action Window / Right Click Menu) grouping with auto collapse, *click the down arrow to drop the ODFC control panel down (KSP 1.7.1).*
- Two new features from the game settings:
   - Stall: fuel cell stops working if vessel total electric charge falls to close to zero (0f) and will not start until there is more electric charge. Fuel cells require EC to work.
   - autoSwitch: automatically switched fuel mode looking for fuel if the current mode becomes fuel deprived.
- more features coming soon

#### Installation Directions (assumes basic KSP mod installation knowledge):
- Extract to your KSP folder.
- Install related ModuleManager patches.

### Changelog Summary
*See ![ChangeLog](https://github.com/zer0Kerbal/ODFCr/blob/master/changelog.md) for full details of mod changes*

#### Known Issue Tracker
* [NEW][BUG 1.1.2.1a] AmpYear doesn't seem to recognize ODFC
* [BUG 1.1.2.0a] TweakScale will not scale module ODFC
* [BUG 1.1.2.0b] Kerbalism doesn't recognize ODFC, ODFC still functions correctly
* [BUG 0.0.1.9a] B9 doesn't work on ODFC enabled parts
* [BUG 0.0.1.9b] next fuel mode should not be visible when only one mode
* [BUG 0.0.1.6a] BackgroundProcessing or Background Resources mods don't see ODFC, so ODFC doesn't work when doesn't have focus. Should not have both BackgroundProcessing and BackgroundResources installed.
* *any mod that requires to use onLoad() instead of onStart() to update a part*

<hr>

 #### Requires:
 - ****Parts designed to use, or patches to modify existing parts**** *This mod (addon) does nothing by itself.*

 #### Dependencies
 - ![Kerbal Space Program](https://kerbalspaceprogram.com) v1.8.1, ***may*** work on earlier versions
 - ![Module Manager](http://forum.kerbalspaceprogram.com/index.php?/topic/50533-105-*)
 - ![ODFC Modify Patches](https://spacedock.info/mod/2220/On%20Demand%20Fuel%20Cells%20-%20Modify%20Patches) and/or - ![ODFC Copy Patches](https://spacedock.info/mod/2221/On%20Demand%20Fuel%20Cells%20-%20Copy%20Patches)  

 #### Supports
 - ![AllYAll](http://forum.kerbalspaceprogram.com/index.php?/topic/155858-ksp-122-all) - supports by removing
 - ![Community Resource Pack](https://forum.kerbalspaceprogram.com/index.php?/topic/166314-131-*)
 - ![BackgroundProcessing](http://forum.kerbalspaceprogram.com/index.php?/topic/88777-102-*) *(exclusive to BackgroundResources) (see known issues list)*
 - ![Background Resources](https://github.com/KSP-RO/TacLifeSupport/wiki) *(exclusive to BackgroundProcessing) (see known issues list)*
 - ![Kerbal Change Log](https://forum.kerbalspaceprogram.com/index.php?/topic/179207-*)

 #### Suggests *(These mods have Fuel Cells that can benefit from ODFC)*:
 - ![Hot Beverages Irradiated](https://github.com/zer0Kerbal/HotBeverageIrradiated)
 - ![Bluedog Design Bureau](http://forum.kerbalspaceprogram.com/index.php?/topic/122020-*)
 - ![Mining Expansion](http://forum.kerbalspaceprogram.com/index.php?/topic/130325-105-*)
 - ![Univeral Storage II](https://forum.kerbalspaceprogram.com/index.php?/topic/177385-*)
 - ![Universl Storage](https://forum.kerbalspaceprogram.com/index.php?/topic/68043-*)
 - ![RLA Reborn](https://forum.kerbalspaceprogram.com/index.php?/topic/175512-14-*)
 - ![Solid Fuel Cells](https://forum.kerbalspaceprogram.com/index.php?/topic/187776-%E2%89%A510-solid-fuel-cell)
 - ![Jatwaa Demolitions Co (coming soon)]()
 - ![KGEx (coming soon)]()

 #### Does not work with parts from (because they use own generation MODULES)
 - Kethane
 - USI
 - @Angel-125's mods (Buffalo, Pathfinder, et al)
 -

 #### Conflicts:
 - ![ODFC - On Demand Fuel Cells by Orum](http://forum.kerbalspaceprogram.com/index.php?/topic/138431-111-*) >-- ORIGINAL (outdated)--<

 <hr>
 ## links to original:  
 On Demand Fuel Cells (ODFC) by `Orum'  
 Licensed under CC BY-NC-SA-4.0  
  * ![KSP Forums](https://forum.kerbalspaceprogram.com/index.php?/topic/138431-112-*)
  * ![Spacedock](https://spacedock.info/mod/618/ODFC%20-%20On%20Demand%20Fuel%20Cells)
  * ![Dropbox](https://www.dropbox.com/s/0rpp4138jumvaxq/ODFC_v1.1.zip?dl=0)
 <hr>
 ## License

 ![[CC 4.0 BY-NC-SA](https://creativecommons.org/licenses/by-nc-sa/4.0)](https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png "CC 4.0 BY-NC-SA") content licensed under a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

 > You may modify for personal use. You may redistribute content with attribution to original author nli2work, plus any other attribution where required. You must redistribute under identical license, (CC BY-NC-SA 4.0)(https://creativecommons.org/licenses/by-nc-sa/4.0).

 @Orum (the mod's original author) has given permission to release this under CC-BY-NC-SA-4.0.

 *Be Kind: Lithobrake, not jakebrake! Keep your Module Manager up to date*

##### All bundled mods are distributed under their own licenses
##### All art assets (textures, models, animations) are distributed under an All Rights Reserved License.

###### v1.1.2.1 original: 11 Aug 2018 zed'K | updated: 21 Sep 2019 zed'K & 4x4cheesecake & LinuxGuruGamer
<!--
CC BY-NC-SA-4.0
zer0Kerbal-->

* ![Stack/Stock fuel cells balance survey](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=stackcells&template=stock_report.md&title=BalanceSurvey)

* ![Have a patch to add?](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=patches&template=feature_request.md&title=Patch)

* ![Bug report?](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=bug&template=bug_report.md&title=BUG_report)

* ![feature request?](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=feature&template=feature_request.md&title=feature_request)
