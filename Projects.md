// On Demand Fuel Cells Refueled
// Projects - v 1.0
// created: 26 Aug 19
// updated: 26 Aug 19

# On Demand Fuel Cells Refueled
## next steps and projects
### this is speculative and merely a possible to do list
#### best laid plans of mice and kerbals alike...

Plan is to release first week of September 2019, coinsiding with Hot Beverages initial dev release(at least the fuel cell parts).

##### add to part module (use FSHORT code to read in)

  - enabled = True
  - threshold = 0.05
  - rateLimit = 1.00
  - defaultMode = 1
  - autoSwitch = False
  - stallEC = False
  - storedChargeFirst = False *(NearFuture)*
  - researvePowerFirst = False *(Ampyear)*
  - megajoulesFirst = False *(KSP Interstellar)*

##### implement into code.
  - autoSwitching fuel mode will be the most difficult.
  - stallEC = if vessel EC = 0f stall the fuelcell until EC > 0f (realism)

##### stretch goal add the following two for each fuel/byproducts:
  - minimumAmount = 0.0 (fuels)
  - maximumAmount = 1.00 (byproducts)
  - ventExcess = True (byproducts, vent excess over maximum Amount)
  - flowMode = All


// zer0Kerbal
// CC-BY-NC-SA-4.0 
// CC-BY-NC-SA-4.0 