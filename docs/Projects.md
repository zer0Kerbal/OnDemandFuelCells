d// On Demand Fuel Cells Refueled
// Projects - v 1.1
// created: 26 Aug 19
// updated: 12 Aug 19

# On Demand Fuel Cells Refueled
## next steps and projects
### this is speculative and merely a possible to do list
#### best laid plans of mice and kerbals alike...

Plan is to release first week of September 2019, coinsiding with Hot Beverages initial dev release(at least the fuel cell parts).
#### PAW and Grouping
  - [x] color code type of patch - green for copy and blue for modify
  - [x] patches need to be MODULE,0 to keep ODFC near top
  - [x] color code RMB/PAW label with same color code
  - [x] implement grouping and auto collapse
  - [x] implement summary status on grouping label (ODFC: *status* | EC/*s*: *currentEC/*maxEX*
  - [ ] have PAW remember collapse status of group
  - [ ] convert to using double slider (KSP 1.7.1)

##### add to part module (use FSHORT code to read in)

  - [ ] enabled = True
  - [ ] threshold = 0.05
  - [ ] rateLimit = 1.00
  - [ ] defaultMode = 1
  - [ ] autoSwitch = False
  - [ ] stallEC = False
  - [ ] storedChargeFirst = False *(NearFuture)* (double slider for min/max/%)
  - [ ] researvePowerFirst = False *(Ampyear)* (double slider for min/max/%)
  - [ ] megajoulesFirst = False *(KSP Interstellar)* (double slider for min/max/%)

##### Mod Compatability
  - [ ] :NEEDS[Kethane] Kethane uses its own converter mode, but Kethane the resource (Kethane in, EC +xenonGas out)

##### implement into code.
  - [x] #0.0.2.0 autoSwitching fuel mode will be the most difficult.
  - [x] #0.0.2.0 stallEC = if vessel EC = 0f stall the fuelcell until EC > 0f (realism)
  - [x] #0.0.2.0 difficulty settings - moderate and hard turn on stall automatically
  - [x] #0.0.2.0 put stall mode option into difficulty settings

##### stretch goal add the following two for each fuel/byproducts:
  - [ ] minimumAmount = 0.0 (fuels)
  - [ ] maximumAmount = 1.00 (byproducts)
  - [ ] ventExcess = True (byproducts, vent excess over maximum Amount)
  - [ ] flowMode = All


// zer0Kerbal
// CC-BY-NC-SA-4.0 
