# Changelog  
  
| modName    | On Demand Fuel Cells (ODFC)                                       |
| ---------- | ----------------------------------------------------------------- |
| license    | GPL-2.0                                                           |
| author     | Orum and zer0Kerbal                                               |
| forum      | (https://forum.kerbalspaceprogram.com/index.php?/topic/187625-*/) |
| github     | (https://github.com/zer0Kerbal/OnDemandFuelCells)                 |
| curseforge | (https://www.curseforge.com/kerbal/ksp-mods/OnDemandFuelCells)    |
| spacedock  | (https://spacedock.info/mod/2223)                                 |
| ckan       | OnDemandFuelCells                                                 |

## 1.2.99.0-prerelease - `<NextMode>` edition

* 05 Aug 2022
* Released for Kerbal Space Program 1.12.x

### Summary 1.2.99.0

* Recompiled for KSP 1.12.3 (NET 4.5.2 - C# 5.0)
* internal updates and changes to prepare for further updates
* Now includes modify patches and no longer compatible with
  * modify patches
  * copy patches
  * will update those releases if enough demand

### Code 1.2.99.0

* Recompiled
  * .NET 4.5.2
  * C# 5.0
  * KSP 1.12.3
  * <OnDemandFuelCells.dll>
    * file version 1.2.99.100
* Renamed
  * <ODFC.dll> to <OnDemandFuelCells.dll>

### Config 1.2.99.0

* Update
  * all configs to reflect dll namechange
  * AllYAll-Removal.cfg v1.2.0.0
  * OnDemandFuelCells.cfg v1.0.0.0
  * <OnDemandFuelCells.version>
    * remove "KSP_VERSION_MAX"

### Compatibliity 1.2.99.0

* Update
  * all patches to reflect dll namechange
  * StockPods v1.9.0.0
  * StockFuelCells.cfg v1.3.0.0

### Status 1.2.99.0

* Issues
  * closes #65 - OnDemandFuelCells <ODFC> 1.2.99.0-prerelease `<EDITION>`
  * closes #66 - 1.2.99.0 Verify Legal Mumbo Jumbo
  * closes #67 - 1.2.99.0 Update Documentation
  * closes #68 - 1.2.99.0 Update Social Media

---

## Version 1.2.0.95 - for KSP 1.12.3 [06-Apr-2020]

* #55 - Tweak scale support - contributed by zer0Kerbal
* #56 - Dev/tweak scale support lisias 4 - contributed by zer0Kerbal

### 1.2.0.18 dev build

* added < internal double _fuelModeMaxECRateLimit = 0f; >
* added < public double OnDemandFuelCellsEC { get { return this._fuelModeMaxECRateLimit; }; } >
* corrected many <Double> -> <double> in code
* changed < double int timeOut = 1; > to internal  
* changed updateFT() -> updateFuelTexts
* changed updateFS() -> updateFuelString
* changed scn -> ConfigNodeString

### 1.2.0.17 dev build

* slight tweaks to autobuild process
* slight code tweaks - working on TweakScale issue.
* #49 - NRE related to capacitors

### 1.2.0.16 dev build

* made public Double fuelModeMaxECRateLimit = 0f; into global variable to be used by AmpYear et al.

### 1.2.0.15 dev build

* split off backgroundProcessing code into separate file

### 1.2.0.14 dev build

### 1.2.0.13 dev build

### 1.2.0.12 dev build

### 1.2.0.11 dev build

### 1.2.0.10 dev build

* PAW updated
  * fixed formatting issues breaking the PAW
  * added suffix to fuel rate (/s /m /h)
  * should now be rounding to 6 decimal places
* initial code changes to implement min/max on/off thresholds (turn on %, turn off %)
* initial code changes to implement min/max EC production rate (min %, max %)

### 1.2.0.9 dev build

* started to add boilerplate basic backgroundProcessing code structure and supporting docs
* added try {} exception handling code to see if better best practices
* added additional debug.log code (so screen message, in game mail, and now ksp.log)
* added CurrentVesselChargeState to the PAW - not final placement - shows % current/max vessel EC
* started moving specific related code sections into#regions
* continued added / editing / clarifying /// <summary> sections
* contined updating code variables from public to private

### 1.2.0.8 dev build

* added and adjusted fuel_consumption and byproducts strings to include rate
* honors PAW color settings
* need to figure out how to format the number string to limit the max# of characters (MAYBE)
* found an issue where color HexDec codes included extra FF at end - so instead of being 6 characters long, they were 8. Okay, who tried signing the code?
* GlobalScalingFactor now included on Game Settings Page, and difficulty settings.
* Code: resourceLa -> resourceLabel (string)

### 1.2.0.7 dev build

* TweakScale working in editor, not flight
* fixed the issue causing ODFC to error out in flight
* started adding GlobalScalingFactor (added on settings page)

### 1.2.0.6 dev build

* added revised tweakscale support by adding private void UpdateEditor();

## Known Issue Tracker

* [BUG 1.1.2.1a] AmpYear doesn't seem to recognize ODFC
* [BUG 1.1.2.0b] Kerbalism is not compatible with ODFC - Kerbalism developers have chosen to not integrate since they have their own version.
* [BUG 0.0.1.6a] Doesn't seem to work with BackgroundResources mod (so ODFC doesn't work when doesn't have focus)

* Swatted with the big can of KAID bug zapper
  [WIP] Work In Progress[BUG 1.1.2.0a] TweakScale
  [BUG 1.2.0.0a] fuel consumption and byproduct production should be seen on PAW - it is not
  [D][BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
  [D][BUG 0.0.1.4a] fuel cell doesn't switch to the "Fuel Deprived" state if you run out of any of the currently used   ources and continues to produce EC
  [D][BUG 0.0.1.4b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with resourceAbbreviations
  [D][BUG 0.0.1.4c] if ECneed > ECsupply & ECtotal = 0 will make the PAW fluctuate in size.
  [D][BUG 0.0.1.3a] upon vehicle load, PAW showing errors that go away with activation/EC production
  [D][BUG 0.0.1.3b] missing 'prev' fuel button
  [D][BUG 0.0.1.3c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
  [D][BUG 0.0.1.3d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
  [R][BUG 0.0.1.3e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?
  [D][BUG 0.0.1.2a] log spam - Val was singing offkey in the OnStart method
  [D][BUG 0.0.1.2b] must have some EC to function, if EC == 0 causes ODFC to hang
  [D][BUG 0.0.1.2c] Does not decrement fuel (all or any)

## Feature Request Tracker

* AYA integration
* Add Heat production
* Convert to On-Demand Resource Converter (still base of ODFC) by either adding or modifying
* Copy/Modify patches add 5 storedCharge (and DischargeCapacitor module) if Near Future Electrical installed
* Copy/Modify patches add 5 ReservePower if AmpYear installed
* Copy/Modify patches add 0.01 megaJoules if InterstellarFuelSwitch installed

---

## Version 1.2.0.3

* KSP 1.12.3
* 29-Nov-2019

* #50 - Dev/tweak scale support

---

## Version 1.2.0.0 - To Go Boldly

* <mark>Recompiled for update to Kerbal Space Program (KSP) 1.8.1</mark>
* Using .NET Franework 4.8
* Using Unity 2019.2.2f1
* update .csproj: <code>Reference Include="$(DevDir)\KSP_x64_Data\Managed\UnityEngine*.dll" /</code>
* now can enter numbers instead of using slider by using the# on PAW
* Continued working on [BUG 1.1.2.0a] TweakScale
* #46 - [NEW][BUG 1.1.2.1b] ODFC forgets its editor settings when moving to flight

---

## Version 1.1.2.1 - Set SCE to AUX

* v
  * [BUG 1.1.2.0b] Kerbalism is not compatible with ODFC - Kerbalism developers have chosen to not integrate since they have their own version.
  * [D][BUG 1.1.2.1b] Found the problem, Bill left a torque converter, which was set to moar, in the innards. Plus Val thought it would be fun to hide her signature in the code of the FCOS (Fuel Cell Operating System). In doing so, she overwrote the code to remember the editor settings when translating over to flight. This has been fixed by moving her signature into another mod's code. She's still happy.
  * Updated to automated build (deploy.bat,buildRelease.bat/Assembly.tt)
* #47 - 1.1.2.1 - contributed by zer0Kerbal
* #33 - PAW grouping label
* #44 - [BUG 1.1.2.0b] Kerbalism

---

## Version 1.1.2.0 - Stall Alert! aka Stall Tactics

* ***[NEW]*** Optional difficulty settings for moderate and hard play - stall. Don't run out of EC or the fuel cells will stop unctioning since they need electricity to function. Thank you to LinuxGuruGamer for the Setting.cs code and assistance.
* ***[NEW]*** feature: autoSwitch (disabled for hard play). If current fuel mode is lacking fuel, will automatically look for nother that has fuel.
* ***[NEW]*** the group label on the PAW/RMB part window now reflects status of the fuel cell so you don't have to expand the roup to peek in to see if the refrigerator light is on. thank you to @Blowfish, @Dmagic, and @LinuxGuruGamer for the assist. hat is one LONG piece of code.
* PAW group label now in *living color*
* removed attempt at adding color to PAW group label.  those coders have been sacked.
* continued working on [BUG 0.0.1.9b]
* /*CODE*/ udfs - updateFS
* /*CODE*/ udft - updateFT
* [NEW][BUG 1.1.2.0a] TweakScale will not scale module ODFC because onLoad() vs onStart issues?
* reversed removal of color coding PAW label. those coders have been sacked, and those sackers of the previously sacked are ow extra-judicially *sacked*
* PAW label now acts as a color coded STATUS label - dynamically updating
* [NEW][BUG 1.1.2.0b] Kerbalism doesn't seem to recognize the unique brilliance of ODFC, not their fault hard to see when its that bright. :D
* #38 - Pr/36 - contributed by zer0Kerbal
* #42 - 0.0.2.0 - contributed by zer0Kerbal
* #27 - Mode/Fuel Autoswitch
* #35 - Add 'Stalled' Mode to Difficulty:Hard in settings menu
* #41 - [BUG 0.0.1.9c] ERROR!'s out when there is only one fuel mode.

---

## Version 0.0.1.9 (this is actually 1.1.1.9 and next release will switch to 1.1.2.0)

* 0.0.1.9 (this is actually 1.1.1.9 and next release will switch to 1.1.2.0)
* added item grouping in PAW.
* [NEW][BUG 0.0.1.9a] B9 module swapping - needs onLoad etc update to make work
* [NEW][BUG 0.0.1.9b] next fuel mode should not be visible when only one mode
* [D][BUG 0.0.1.9c] mangled config caused this. added error checking in cfg.cs - thank you LGG for this code.
* [NEW][BUG 0.0.1.9c] ERROR!'s out when there is only one fuel mode. Stock pod patch only adds one mode (monoprop - because ods usually have monoprop if they have any fuel). This bug was temporarily fixed by added a second mode(it can be the same as he first so it appears like there only one fuel mode) in the patch (LFO).
* Split patches into two categories, copy (green text) and modify (blue text)
* Copy Patches now automatically rename the part with an ODFC prefix
* Copy/Modify patches all add 50 cost, 0.001 mass, 5 EC battery, and 5 MP tank to all parts, even if part already has a attery / monoPropellant tank.
* Added support for the following: JatwaaDemolitionsCo, SolidFuelCell, StockPods, UniversalStorage2,
* Patches coming for the following: Bluedog Design Bureau, RLA, MiningExpansion, UniversalStorage
* ad hoc, ergo promptus hoc: dropping the 'v' on all future version numbering.
* #32 - V.0.0.1.9 - contributed by zer0Kerbal
* #34 - V.0.0.1.9 - contributed by zer0Kerbal
* #25 - AllYAll integration
* #31 - When EC=0 - can't switch mode

---

## Version 0.0.1.8

* ns renamed to newState
* nmax renamed to newMax
* add new recommended mod: HotBeverageReheated
* add ODFC and AYA_FuelCell patches for HotBeverageReheated
* add ODFC and AYA_FuelCell patches for UniversalStorage2
* fixed typo in StockFuelCells.cfg (replaced '-' with '=')
* [D][BUG 0.0.1.2b] must have some EC to function, if EC == 0 causes ODFC to hang - just had to move two lines of code to xecute earlier. Reason this works, why it wasn't working if vessel EC == 0 was the order of execution. Needed to add the enerated EC before handling byproducts, and remove fuel which triggered update state.
* Release to Beta-Testers
* #29 - V.0.0.1.8 - contributed by zer0Kerbal
* #30 - Update README.md - contributed by zer0Kerbal
* #12 - [BUG 0.0.1.2b] Must have some EC to function, if EC == 0 does not charge
* #19 - [BUG 0.0.1.4a] fuel cell doesn't switch to the &quot;Fuel Deprived&quot; state if you run out of any of the currently used resources and continues to produce EC

---

## Version 0.0.1.7

* [D][BUG 0.0.1.4c] other fixes seems to have fixed this.
* [D][BUG 0.0.1.4a] someone tried to hotwire the fuel tanks with EC (**ElectricChargeID** needed to be replaced with **fuel.esourceID** )
* removed ScreenMessages#DEBUG code
* #26 - V.0.0.1.7 - contributed by zer0Kerbal
* #21 - [BUG 0.0.1.4c] [BUG] if ECneed &gt; ECsupply &amp; ECtotal = 0 will make the PAW fluctuate in size.

---

## Version 0.0.1.6

* commented Scale related code out
* removed commented code from StockFuelCells.cfg to fix [BUG 0.0.1.6b]
* Added AllYAll-Removal.cfg (AllYAll doesn't currently work with ODFC, and the AYA_FuelCell module carries over with +PART)
* Bug tracking now implemented via Github (all current and prior known bugs have been added and updated)
* [BUG 0.0.1.3e] changed to Future Feature Request (heat)
* [NEW][BUG 0.0.1.6a] Doesn't seem to work with BackgroundResources mod (so ODFC doesn't work when doesn't have focus)
* [D][NEW][BUG 0.0.1.6b] AllYAll module being added
* #8 - git maintenance
* #10 - update 
* #11 - 0.0.1.2a - log spam
* #13 - [BUG 0.0.1.2c] Does not decrement fuel (all or any)
* #14 - [BUG 0.0.1.3d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
* #15 - [BUG 0.0.1.3e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?
* #16 - [BUG 0.0.1.3b] missing 'prev' fuel button
* #17 - [BUG 0.0.1.3c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
* #18 - [BUG 0.0.1.3a] upon vehicle load, PAW showing errors that go away with activation/EC production
* #20 - [BUG 0.0.1.4b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with resourceAbbreviations
* #22 - [BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
* #24 - [BUG 0.0.1.6b] AllYAll module being added

---

## Version 0.0.1.5

* [D][NEW][BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
* corrected changlog bug numbering (missing .)
* started converting changelog to include markup
* [D][BUG 0.0.1.4b] zer0Kerbal left the capslock on when typing WATER, which the code didn't like.
* removed code supporting FSHORT, instead using game references, which will also bring in localization
* removed FSHORT from StockFuelCells.cfg
* #7 - v.0.0.1.5

---

## Version 0.0.1.4

* 4x4cheesecake
  * variable sweep and decrypt (continued)
  * typo correction
  * removed animation code
  * [D][BUG 0.0.12a] fixed log spam
* variable sweep and decrypt (continued)
* [D][BUG 0.0.13d] corrected misplaced decimal point in StockFuelCells.cfg (LF consumption 1/10 what it should have been in LFIA mode)
* [WIP]Added H+0 operational mode to StockFuelCells :NEEDS(Community Resource Kit)
* Decyphered variables
  * cft = cfTime (temp)
  * mecrl = fuelModeMaxECRateLimit
  * ecn = ECneed
  * eca - ECAmount
BUG:
  * [NEW][BUG 0.0.1.4a] fuel cell doesn't switch to the "Fuel Deprived" state if you run out of any of the currently used esources and continues to produce EC
  * [NEW][BUG 0.0.1.4b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with esourceAbbreviations
  * [NEW][BUG 0.0.1.4c] [BUG] if ECneed > ECsupply & ECtotal = 0 will make the PAW fluctuate in size.

---

## Version 0.0.1.3

* created StockFuelCells.cfg which copies both stock fuel cells and adds ODFC
* 4x4cheesecake variable sweep and decrypt
* comment out all emmitter and light code, left animation
* zer0Kerbal variable sweep and decrypt
* [D][BUG 0.0.1.2b] zedEC bug - fixed (variable name)
* [D][BUG 0.0.1.2c] fuel decrement - fixed (variable name)
* BUG:
  * [BUG 0.0.1.2a] log spam
  * [NEW][BUG 0.0.1.3a] upon vehicle load, PAW showing errors that go away with activation/EC production
  * [NEW][BUG 0.0.1.3b] missing 'prev' fuel button
  * [NEW][BUG 0.0.1.3c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
  * [NEW][BUG 0.0.1.3d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
  * [NEW][BUG 0.0.1.3e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?
* #2 - Rename fields and methods - contributed by 4x4cheesecake
* #3 - v0.0.1.3
* #4 - Fix log spam - contributed by 4x4cheesecake
* #5 - update readme
* #6 - v.0.0.1.4

---

## Version 0.0.1.2

* released:  
* [REVERTED] updated .version to 1.7.3.1  
* recompiled against KSP 1.7.3  
* converted FLOAT to DOUBLE - might be slower, but KSP seems to be going this way
* updated Assembles.cs
* updated entire project
* BUG:
* [NEW][BUG 0.0.1.2a] log spam - Val was singing offkey in the OnStart method
* [NEW][BUG 0.0.1.2b] Must have some EC to function, if EC == 0 does not charge
* [NEW][BUG 0.0.1.2c] Does not decrement fuel (all or any)
* #1 - initial successful compile.

---

## Version 1.1

* >-- ORIGINAL --<
* ODFC - change log
* Released on 2016-04-29
* Release split into two pieces; plug-in is now distributed separately from the OI parts, to ease CKAN support
* Updated to support KSP v1.1 (KSP 1.0 was not and will not be supported)
* UI will now hide certain elements if they are not used (mode switching and fuel used is hidden for those with *gle mode; Byproducts is hidden unless there are at least 2 modes, at least one of which has a byproduct)
* Info screen will now report units in an easier to read and interpret manner (e.g. 0.36/h instead of 0.0001/s)
* Fixed a bug where floats were truncated to integers when reading the MaxEC value
* Changed code to use public KSP/Unity functions where applicable
* Very small code optimizations and organization

---

## Version 1.0

* Initial release (for KSP v0.90)

---
