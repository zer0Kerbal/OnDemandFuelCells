---
permalink: /RoadMap.html
title: Road Map
---

<!-- RoadMap - v 1.2.0.0
On Demand Fuel Cells (ODFC)
created: 26 Aug 19
updated: 20 Feb 2022 -->

# On Demand Fuel Cells (ODFC)

implement 'stalled' mode - with a setting in the difficulty settings menu: this will 'stall' the fuel cell if the vessel (at least reachable) reaches below a certain level of EC (like <= 0),
will not reset until the vessel has at least 0.5 EC

fix showing 'next' button when there is only one mode of operation
implement double slider in B9Partswitch
implement PAW status in group header

add to part module pulled from MODULE config nodes (use FSHORT code to read in)

implement and add autoSwitch fuel deprived auto mode switcher will be the most difficult.

 void huntPeck()
 {
     currentFuelMode++
   if (currentMode <= totalModes) // check for depleted
   else { currentmode = 0 }; // need to make sure not spamming autohunt
 }

 //MODULE variables
     double  threshold = 0.05f, //thresHoldSteps
             rateLimit = 1;

  byte    defaultMode = 1;

  bool    autoSwitch = false,
            enabled = true,
             UseSpecialistBonus = false;

eventually want to add the following for each fuel/byproducts:
 per FUEL/BYPRODUCT:
     double  reserveAmount = 0.0f, //(fuels)
             maximumAmount = 1.00f; // (byproducts)

  bool    ventExcess = True(byproducts, vent excess over maximum Amount)
    // flowMode = All;

<!-- this file CC BY-NC-ND 3.0 Unported by zer0Kerbal>