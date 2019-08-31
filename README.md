<!-- Readme.md v1.6
On Demand Fuel Cells Refueled
created: 17 Jul 18
updated: 30 Aug 19 -->

<!-- Download on SpaceDock here or Github here.
Also available on CKAN. -->

# On Demand Fuel Cells Refueled (ODFCr)
![Mod v0.0.1.9](https://img.shields.io/badge/MOD%20version-0.0.1.9-orange.svg?style=flat-square)
![KSP 1.7.x](https://img.shields.io/badge/KSP%20version-1.7.x-66ccff.svg?style=flat-square)
![CKAN listed](https://img.shields.io/badge/CKAN-Indexed-brightgreen.svg)
#### Formerly known as On Demand Fuel Cells (ODFC)

Continuation of *On Demand Fuel Cells (ODFC)* by `**Orum**', now continued by the cooperative efforts of 4x4cheesecake, LinuxGuruGamer, and zer0Kerbal.

![ODFC PAW](https://i.postimg.cc/7Pj7gQQD/image.png)

On Demand Fuel Cells (ODFC) is a plugin to simulate fuel cells in Kerbal Space Program (KSP), and do a better job of it than stock's use of a resource converter.  The main difference is it only generates electricity when it's really needed (batteries almost empty), and otherwise lets electricity of a craft float up and down, as it might in a solar powered vehicle when the sun is eclipsed by another celestial body.  It also allows fuel cells to generate byproducts, aimed at supporting life support mods like TACLS.

The plugin comes with a patch that copies the stock Fuel Cell and Fuel Cell Array and replaces the stock modules with ODFC that has three modes (four if Community Resource Pack is installed correctly) of operation.

Features:

- adjustable fuel cell use - much more than just On/Off operation
- multiple fuel modes (serial usage - one mode at a time)
- variable activation threshold
- configurable to produce byproducts (so O+H2 = EC + H2O)
- very small memory footprint
- Brown and Black out protection assistance

### Installation Directions (assumes basic KSP mod installation knowledge):
- Extract to your KSP folder.
- Install related ModuleManager patches.

### Summary Changelog
*See ![ChangeLog](https://github.com/zer0Kerbal/ODFCr/blob/master/changelog.md) for full details of mod changes*
<hr>
#### STATUS:
 * ***Initial-Release***

####  v.0.0.1.8 (DEV BUILD)
 * ns renamed to newState
 * nmax renamed to newMax
 * add new recommended mod: HotBeverageReheated
 * add ODFC and AYA_FuelCell patches for HotBeverageReheated
 * add ODFC and AYA_FuelCell patches for UniversalStorage2
 * fixed typo in StockFuelCells.cfg (replaced '-' with '=')
 * [D][BUG 0.0.1.2b] must have some EC to function, if EC == 0 causes ODFC to hang - just had to move two lines of code to execute earlier. Reason this works, why it wasn't working if vessel EC == 0 was the order of execution. Needed to add the generated EC before handling byproducts, and remove fuel which triggered update state.
 * Release to Beta-Testers

## Known Issue Tracker
 + [WIP] Work In Progress
 * [BUG 0.0.1.6a] Does not seeming work with BackgroundProcessing or Background Resources mods (being looked at) (so ODFC doesn't work when doesn't have focus). Should not have both BackgroundProcessing and BackgroundResources installed.

## Feature Request Tracker
 + ![AllYAll](http://forum.kerbalspaceprogram.com/index.php?/topic/155858-ksp-122-all) integration
 + ![NearFuture](https://forum.kerbalspaceprogram.com/index.php?/topic/155465-16x-near-future-technologies-16x-fixes-jan-21/) integration
 + ![Dymanic Battery Storage](https://github.com/ChrisAdderley/DynamicBatteryStorage/releases/tag/2.0.5) integration
 + ![Fusebox](http://forum.kerbalspaceprogram.com/index.php?/topic/157896-122-fusebox-continued-electric-charge-tracker-and-build-helper/) integration
 + ![Ampyear](http://forum.kerbalspaceprogram.com/index.php?/topic/114991-105-ampyear-main-reserve-power-manager-ion-rcs-traffic-light-icons-edition-v1200-28-feb-2016/) integration
 + Add Heat production
 + Convert to On-Demand Resource Converter (still base of ODFC) by either adding or modifying
 + Determine if both BackgroundProcessing and Background Resources mods are installed and adjust.
 + add a vessel wide control panel (GUI) for all ODFC on vessel.
<hr>
 #### Requires:
 - ****Parts designed to use, or patches to modify existing parts**** *This mod (addon) does nothing by itself.*

 #### Dependencies
 - ![Kerbal Space Program](https://kerbalspaceprogram.com) v1.7.3, ***may*** work on earlier versions
 - ![Module Manager](http://forum.kerbalspaceprogram.com/index.php?/topic/50533-105-*)

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
 - ![]()
 - ![]()

 #### Does not work with parts from (because they use own generation MODULES)
 - Kethane
 - USI
 - @Angel-125's mods (Buffalo, Pathfinder, et al)
 -

 #### Conflicts:
 - ![ODFC - On Demand Fuel Cells by Orum](http://forum.kerbalspaceprogram.com/index.php?/topic/138431-111-*)
 <hr>
 #### Known issues:
 - ![Background Processing]()
 - ![Background Resources]()
 - ![B9Partswitch]()
 - ![Tweakscale]()
 - any mod that requires to use onLoad() instead of onStart() to update a part  

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

###### v0.0.1.9 original: 11 Aug 2018 zed'K | updated: 30 Aug 2019 zed'K & 4x4cheesecake & LinuxGuruGamer
<!--
CC BY-NC-SA-4.0
zer0Kerbal-->

*Stack/Stock fuel cells balance survey? Kindly do submit on ![github](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=stackcells&template=stock_report.md&title=BalanceSurvey)!*

*Have a patch to add? Kindly do submit on ![github](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=patches&template=feature_request.md&title=Patch)!*

*Have a bug to report? Kindly do submit on ![github](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=bug&template=bug_report.md&title=BUG_report)!*

*Have a feature to request? Kindly do submit on ![github](https://github.com/zer0Kerbal/ODFCr/issues/new?assignees=zer0Kerbal&labels=feature&template=feature_request.md&title=feature_request)!*
