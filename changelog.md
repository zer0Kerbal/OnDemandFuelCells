# ODFC  
## On Demand Fuel Cells Refueled (ODFCr)  

### Changelog
#### STATUS:
 * ***Release***
#### 1.1.2.1 - Set SCE to AUX
 * [BUG 1.1.2.0a] TweakScale
 * [NEW][BUG 1.1.2.1a] AmpYear doesn't seem to recognize ODFC
 * BUG 1.1.2.0b] Kerbalism is not compatible with ODFC - Kerbalism developers have chosen to not integrate since they have their own version.
 * [D][BUG 1.1.2.1b] Found the problem, Bill left a torque converter, which was set to moar, in the innards. Plus Val thought it would be fun to hide her signature in the code of the FCOS (Fuel Cell Operating System). In doing so, she overwrote the code to remember the editor settings when translating over to flight. This has been fixed by moving her signature into another mod's code. She's still happy.
 * Updated to automated build (deploy.bat,buildRelease.bat/Assembly.tt)

#### 1.1.2.0 - Stall Alert! aka Stall Tactics
 * ***[NEW]*** Optional difficulty settings for moderate and hard play - stall. Don't run out of EC or the fuel cells will stop functioning since they need electricity to function. Thank you to LinuxGuruGamer for the Setting.cs code and assistance.
 * ***[NEW]*** feature: autoSwitch (disabled for hard play). If current fuel mode is lacking fuel, will automatically look for another that has fuel.
 * ***[NEW]*** the group label on the PAW/RMB part window now reflects status of the fuel cell so you don't have to expand the group to peek in to see if the refrigerator light is on. thank you to @Blowfish, @Dmagic, and @LinuxGuruGamer for the assist. That is one LONG piece of code.
 * PAW group label now in *living color*
 * removed attempt at adding color to PAW group label.  those coders have been sacked.
 * continued working on [BUG 0.0.1.9b]
 * /*CODE*/ udfs - updateFS
 * /*CODE*/ udft - updateFT
 * [NEW][BUG 1.1.2.0a] TweakScale will not scale module ODFC because onLoad() vs onStart issues?
 * reversed removal of color coding PAW label. those coders have been sacked, and those sackers of the previously sacked are now extra-judicially *sacked*
 * PAW label now acts as a color coded STATUS label - dynamically updating
 * [NEW][BUG 1.1.2.0b] Kerbalism doesn't seem to recognize the unique brilliance of ODFC, not their fault hard to see when its that bright. :D

####  0.0.1.9 (this is actually 1.1.1.9 and next release will switch to 1.1.2.0)
0.0.1.9 (this is actually 1.1.1.9 and next release will switch to 1.1.2.0)
 * added item grouping in PAW.
 * [NEW][BUG 0.0.1.9a] B9 module swapping - needs onLoad etc update to make work
 * [NEW][BUG 0.0.1.9b] next fuel mode should not be visible when only one mode
 * [D][BUG 0.0.1.9c] mangled config caused this. added error checking in cfg.cs - thank you LGG for this code.
 * [NEW][BUG 0.0.1.9c] ERROR!'s out when there is only one fuel mode. Stock pod patch only adds one mode (monoprop - because pods usually have monoprop if they have any fuel). This bug was temporarily fixed by added a second mode(it can be the same as the first so it appears like there only one fuel mode) in the patch (LFO).
 * Split patches into two categories, copy (green text) and modify (blue text)
 * Copy Patches now automatically rename the part with an ODFC prefix
 * Copy/Modify patches all add 50 cost, 0.001 mass, 5 EC battery, and 5 MP tank to all parts, even if part already has a battery / monoPropellant tank.
 * Added support for the following: JatwaaDemolitionsCo, SolidFuelCell, StockPods, UniversalStorage2,
 * Patches coming for the following: Bluedog Design Bureau, RLA, MiningExpansion, UniversalStorage
 * ad hoc, ergo promptus hoc: dropping the 'v' on all future version numbering.

#### v.0.0.1.8
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
 * [BUG 0.0.1.6a] Doesn't seem to work with BackgroundResources mod (so ODFC doesn't work when doesn't have focus)

 + Swatted with the big can of KAID bug zapper
 * [D][BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
 * [D][BUG 0.0.1.4a] fuel cell doesn't switch to the "Fuel Deprived" state if you run out of any of the currently used resources and continues to produce EC
 * [D][BUG 0.0.1.4b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with resourceAbbreviations
 * [D][BUG 0.0.1.4c] if ECneed > ECsupply & ECtotal = 0 will make the PAW fluctuate in size.
 * [D][BUG 0.0.1.3a] upon vehicle load, PAW showing errors that go away with activation/EC production
 * [D][BUG 0.0.1.3b] missing 'prev' fuel button
 * [D][BUG 0.0.1.3c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
 * [D][BUG 0.0.1.3d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
 * [R][BUG 0.0.1.3e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?
 * [D][BUG 0.0.1.2a] log spam - Val was singing offkey in the OnStart method
 * [D][BUG 0.0.1.2b] must have some EC to function, if EC == 0 causes ODFC to hang
 * [D][BUG 0.0.1.2c] Does not decrement fuel (all or any)

## Feature Request Tracker
 + AYA integration
 + Add Heat production
 + Convert to On-Demand Resource Converter (still base of ODFC) by either adding or modifying
 * Copy/Modify patches add 5 storedCharge (and DischargeCapacitor module) if Near Future Electrical installed
 * Copy/Modify patches add 5 ReservePower if AmpYear installed
 * Copy/Modify patches add 0.01 megaJoules if InterstellarFuelSwitch installed
 +

####  v.0.0.1.7
 * [D][BUG 0.0.1.4c] other fixes seems to have fixed this.
 * [D][BUG 0.0.1.4a] someone tried to hotwire the fuel tanks with EC (**ElectricChargeID** needed to be replaced with **fuel.resourceID** )
 * removed ScreenMessages #DEBUG code

 ####  v.0.0.1.6
  * commented Scale related code out
  * removed commented code from StockFuelCells.cfg to fix [BUG 0.0.1.6b]
  * Added AllYAll-Removal.cfg (AllYAll doesn't currently work with ODFC, and the AYA_FuelCell module carries over with +PART)
  * Bug tracking now implemented via Github (all current and prior known bugs have been added and updated)
  * [BUG 0.0.1.3e] changed to Future Feature Request (heat)
  * [NEW][BUG 0.0.1.6a] Doesn't seem to work with BackgroundResources mod (so ODFC doesn't work when doesn't have focus)
  * [D][NEW][BUG 0.0.1.6b] AllYAll module being added

####  v.0.0.1.5
 * [D][NEW][BUG 0.0.1.5] the paw label is created from the FSHORT node name in the part.cfg (or patch)
 * corrected changlog bug numbering (missing .)
 * started converting changelog to include markup
 * [D][BUG 0.0.1.4b] zer0Kerbal left the capslock on when typing WATER, which the code didn't like.
 * removed code supporting FSHORT, instead using game references, which will also bring in localization
 * removed FSHORT from StockFuelCells.cfg

####  v.0.0.1.4
 * 4x4cheesecake
	 * variable sweep and decrypt (continued)
	 * typo correction
	 * removed animation code
	 * [D][BUG 0.0.12a] fixed log spam
 * variable sweep and decrypt (continued)
 * [D][BUG 0.0.13d] corrected misplaced decimal point in StockFuelCells.cfg (LF consumption 1/10 what it should have been in LF+IA mode)
 * [WIP]Added H+0 operational mode to StockFuelCells :NEEDS(Community Resource Kit)
 * Decyphered variables
   * cft = cfTime (temp)
   * mecrl = fuelModeMaxECRateLimit
   * ecn = ECneed
   * eca - ECAmount
	BUG:
   * [NEW][BUG 0.0.1.4a] fuel cell doesn't switch to the "Fuel Deprived" state if you run out of any of the currently used resources and continues to produce EC
   * [NEW][BUG 0.0.1.4b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with resourceAbbreviations
   * [NEW][BUG 0.0.1.4c] [BUG] if ECneed > ECsupply & ECtotal = 0 will make the PAW fluctuate in size.

####  v.0.0.1.3
 * created StockFuelCells.cfg which copies both stock fuel cells and adds ODFC
 * 4x4cheesecake variable sweep and decrypt
 * comment out all emmitter and light code, left animation
 * zer0Kerbal variable sweep and decrypt
 * [D][BUG 0.0.1.2b] zedEC bug - fixed (variable name)
 * [D][BUG 0.0.1.2c] fuel decrement - fixed (variable name)
	BUG:
   * [BUG 0.0.1.2a] log spam
   * [NEW][BUG 0.0.1.3a] upon vehicle load, PAW showing errors that go away with activation/EC production
   * [NEW][BUG 0.0.1.3b] missing 'prev' fuel button
   * [NEW][BUG 0.0.1.3c] current/max display just showing ECRateLimit/ECRateLimit instead of ECRateLimit/maxECRateLimit
   * [NEW][BUG 0.0.1.3d] when in LiquidFuel+IntakeAir (LF+AI) mode - LF consumption too low
   * [NEW][BUG 0.0.1.3e] byproduct (heat) missing from StockFuelCells.cfg or should be in .dll?

{
   VERSION
   {
    version = v.0.0.1.2
		change = released:  
		change = [REVERTED] updated .version to 1.7.3.1  
		change = recompiled against KSP 1.7.3  
		change = converted FLOAT to DOUBLE - might be slower, but KSP seems to be going this way
		change = updated Assembles.cs
		change = updated entire project
		change = BUG:
		change = [NEW][BUG 0.0.1.2a] log spam - Val was singing offkey in the OnStart method
		change = [NEW][BUG 0.0.1.2b] Must have some EC to function, if EC == 0 does not charge
		change = [NEW][BUG 0.0.1.2c] Does not decrement fuel (all or any)
  }
// >-- ORIGINAL --<
	VERSION
	{
		version = v1.1
		change = >-- ORIGINAL --<
		change = ODFC - change log
		change = Released on 2016-04-29
		change = Release split into two pieces; plug-in is now distributed separately from the OI parts, to ease CKAN support
		change = Updated to support KSP v1.1 (KSP 1.0 was not and will not be supported)
		change = UI will now hide certain elements if they are not used (mode switching and fuel used is hidden for those with only a single mode; Byproducts is hidden unless there are at least 2 modes, at least one of which has a byproduct)
		change = Info screen will now report units in an easier to read and interpret manner (e.g. 0.36/h instead of 0.0001/s)
		change = Fixed a bug where floats were truncated to integers when reading the MaxEC value
		change = Changed code to use public KSP/Unity functions where applicable
		change = Very small code optimizations and organization
	}
	VERSION
	{
		version = v1.0
		change = Initial release (for KSP v0.90)
	}
}
