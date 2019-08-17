# SGEx  
### Proudly Adopts(HOPEFULLY) - for private/personal use  

cooperative efforts of 4x4cheesecake, LinuxGuruGamer, and zer0Kerbal to continue original *On Demand Fuel Cells (ODFC)* by `**Orum**'  
![ODFC Refueled](https://i.postimg.cc/HLZt1bq1/1.png) 
 
## ODFC  
### ***On Demand Fuel Cells Refueled (ODFCr)***  

### Changelog 
#### STATUS:
 * ***PRE-Release***

####  v.0.0.1.5 
 * [D][NEW][BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
 * corrected changlog bug numbering (missing .)
 * started converting changelog to include markup
 * [BUG 0.0.1.4b] zer0Kerbal left the capslock on when typing WATER, which the code didn't like.
 * removed code supporting FSHORT, instead using game references, which will also bring in localization
 * removed FSHORT from StockFuelCells.cfg
 * 

## Issue Tracker
 + [WIP] Work In Progress
 * [BUG 0.0.1.4a] fuel cell doesn't switch to the "Fuel Deprived" state if you run out of any of the currently used resources and continues to produce EC
 * [BUG 0.0.1.4c] if ECneed > ECsupply & ECtotal = 0 will make the PAW fluctuate in size.
 * [BUG 0.0.1.3e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?
 * [BUG 0.0.1.2b] must have some EC to function, if EC == 0 does not charge
 + Swatted with the big can of KAID bug zapper
 * [D][BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
 * [D][BUG 0.0.1.4b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with resourceAbbreviations
 * [D][BUG 0.0.1.3a] upon vehicle load, PAW showing errors that go away with activation/EC production
 * [D][BUG 0.0.1.3b] missing 'prev' fuel button
 * [D][BUG 0.0.1.3d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
 * [D][BUG 0.0.1.3c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
 * [D][BUG 0.0.1.2a] log spam - Val was singing offkey in the OnStart method
 * [D][BUG 0.0.1.2c] Does not decrement fuel (all or any)
 
 #### v.0.0.1.4 (DEV BUILD)
	* 4x4cheesecake 
		* variable sweep and decrypt (continued)
		* typo correction
		* removed animation code
		* [BUG 0.0.12a] fixed log spam
	* variable sweep and decrypt (continued)
	* [BUG 0.0.13d] corrected misplaced decimal point in StockFuelCells.cfg (LF consumption 1/10 what it should have been in LF+IA mode)
	* Added H+0 operational mode to StockFuelCells
	* 

#### v.0.0.1.3a - RELEASED
	* created StockFuelCells.cfg which copies both stock fuel cells and adds ODFC
	* 4x4cheesecake variable sweep and decrypt
	* comment out all emmitter and light code, left animation
	* zer0Kerbal variable sweep and decrypt
	* [D][BUG 0.0.12b] zedEC bug - fixed (variable name)
	* [D][BUG 0.0.12c] fuel decrement - fixed (variable name)
	BUG:
	* [BUG 0.0.12a] log spam
	* [NEW][BUG 0.0.13a] Upon vehicle load, PAW showing errors that go away with activation/EC production
	* [NEW][BUG 0.0.13b] missing 'prev' fuel button
	* [NEW][BUG 0.0.13c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
	* [NEW][BUG 0.0.13d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
	* [NEW][BUG 0.0.13e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?
  

## Dependencies 
 * Kerbal Space Program
 * Module Manager
 
## Supports 
 * [WIP] Stock Mining Enhancements
 * [WIP] Near Future Electionics
 * [WIP] Bon Voyage
 * [WIP] Universal Storage II
 
## Suggests 
 * 
 * 
 
## License  
![[CC 4.0 BY-NC-SA](https://creativecommons.org/licenses/by-nc-sa/4.0/)](https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png "CC 4.0 BY-NC-SA")

[CC 4.0 BY-NC-SA](https://creativecommons.org/licenses/by-nc-sa/4.0/)

*a part of the **TWYLLTR** (Take What You Like, Leave the Rest) collection.*  
 
📌v0.0.1.5-beta  
 
## links to original:  
On Demand Fuel Cells (ODFC) by `Orum'  
Licensed under CC BY-NC-SA-4.0  
 * ![KSP Forums](https://forum.kerbalspaceprogram.com/index.php?/topic/138431-112-on-demand-fuel-cells-odfc-v11/) 
 * ![Spacedock](https://spacedock.info/mod/618/ODFC%20-%20On%20Demand%20Fuel%20Cells) 
 * ![Dropbox](https://www.dropbox.com/s/0rpp4138jumvaxq/ODFC_v1.1.zip?dl=0) 
 
 
![Jeb's Rule #1"](https://ic.pics.livejournal.com/asaratov/25113347/1448500/1448500_original.jpg   "Jeb's Rule #1") 
  
##### All bundled mods are distributed under their own licenses
##### All art assets (textures, models, animations) are distributed under an All Rights Reserved License.

###### v0.0.1.5 original: 11 Aug 2018 zed'K updated: 17 Aug 2019 zed'K & 4x4cheesecake & LinuxGuruGamer
