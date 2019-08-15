# ODFC  
## On Demand Fuel Cells Refueled (ODFCr)  

### Changelog 
v.0.0.1.4
	* 4x4cheesecake 
		* variable sweep and decrypt (continued)
		* typo correction
		* removed animation code
		* [BUG 0.0.12a] fixed log spam
	* variable sweep and decrypt (continued)
	* [BUG 0.0.13d] corrected misplaced decimal point in StockFuelCells.cfg (LF consumption 1/10 what it should have been in LF+IA mode)
	* [WIP]Added H+0 operational mode to StockFuelCells :NEEDS(Community Resource Kit)
	* cft = cfTime (temp)
	* mecrl = fuelModeMaxECRateLimit
	* ecn = ECneed
	* eca - ECAmount
	BUG:
	* [BUG 0.0.14a] fuel cell doesn't switch to the "Fuel Deprived" state if you run out of any of the currently used resources and continues to produce EC
	* [BUG 0.0.14b] the H2O+water mode causes the part to mis-function (ERROR) - probably has to do with resourceAbbreviations
	* [BUG 0.0.14c] [BUG] if ECneed > ECsupply & ECtotal = 0 will make the PAW fluctuate in size.

v.0.0.1.3
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

v.0.0.1.2 
	* released:  
	* [REVERTED] updated .version to 1.7.3.1  
	* recompiled against KSP 1.7.3  
	* converted FLOAT to DOUBLE - might be slower, but KSP seems to be going this way
	* updated Assembles.cs
	* updated entire project
	BUG:
	* [NEW][BUG 0.0.12a] log spam
	* [NEW][BUG 0.0.12b] Must have some EC to function, if EC == 0 does not charge
	* [NEW][BUG 0.0.12c] Does not decrement fuel (all or any)


>-- ORIGINAL --<
ODFC - change log

v1.1
 - Released on 2016-04-29
 - Release split into two pieces; plug-in is now distributed separately from the OI parts, to ease CKAN support
 - Updated to support KSP v1.1 (KSP 1.0 was not and will not be supported)
 - UI will now hide certain elements if they are not used (mode switching and fuel used is hidden for those with only a single mode; Byproducts is hidden unless there are at least 2 modes, at least one of which has a byproduct)
 - Info screen will now report units in an easier to read and interpret manner (e.g. 0.36/h instead of 0.0001/s)
 - Fixed a bug where floats were truncated to integers when reading the MaxEC value
 - Changed code to use public KSP/Unity functions where applicable
 - Very small code optimizations and organization

v1.0
 - Initial release (for KSP v0.90)
