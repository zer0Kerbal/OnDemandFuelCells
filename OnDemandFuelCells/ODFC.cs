/* ROADMAP TODO:

fix showing 'next' button when there is only one mode of operation
implement double slider in B9Partswitch
DONE: implement PAW status in group header

add to part module pulled from MODULE config nodes(use FSHORT code to read in)
PAW isn't showing consumption / production fuel_consumption and byproducts

DONE: add page to game difficulty settings
DONE: add stall variable and code
DONE: implement 'stalled' mode - with a setting in the difficulty settings menu: this will 'stall' the fuel cell if the vessel (at least reachable) reaches below a certain level of EC (like <= 0),
DONE: will not reset until the vessel has at least 0.5 EC
DONE: implement and add autoSwitch fuel deprived auto mode switcher will be the most difficult.

//MODULE variables
double threshold = 0.05f, //thresHoldSteps
        rateLimit = 1;

byte defaultMode = 1;

bool autoSwitch = false,
          enabled = true,
           UseSpecialistBonus = false;

eventually want to add the following for each fuel/ byproducts:
 per FUEL / BYPRODUCT:
     double  reserveAmount = 0.0f, //(fuels)
             maximumAmount = 1.00f; // (byproducts)

bool ventExcess = True(byproducts, vent excess over maximum Amount)
    // flowMode = All;
 */

#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODFC
{
    public class ODFC : PartModule
    {
        #region Enums Vars
        // add stalled state
        public enum states : byte { error, off, nominal, fuelDeprived, noDemand, stalled }; // deploy, retract,
        private const string FuelTransferFormat = "0.##"; //FuelTransferFormat?
        private const float
            thresHoldSteps = 0.05f, // increment the rate by this amount (default is 5)
            thresholdMin = thresHoldSteps,
            thresHoldMax = 1;
  

        private Double
            lastGen = -1,
            lastMax = -1,
            lastTF = -1;
        private int lastFuelMode = -1;
        private string info = string.Empty;

        public ConfigNode configNode;
        public static List<resourceLa> lastResource = new List<resourceLa>();
        public static int ElectricChargeID;
        public string scn;
        public bool ns = true;
        public cfg ODFC_config;
        public states state = states.error;
        #endregion
        // added PAW grouping, set to autocollapse - introduced in KSP 1.7.1
        // would really like the PAW to remember if the group was open
        #region Fields Events Actions
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public int fuelMode = 0;
        
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Status", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public string status = "ERROR!";

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "EC/s (cur/max)", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public string ECs_status = "ERROR!";

        [KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Max EC/s", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public string maxECs_status = "ERROR!";

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Fuel Used", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public string fuel_consumption = "ERROR!";

        [KSPField(isPersistant = false, guiActive = false, guiActiveEditor = false, guiName = "Byproducts", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public string byproducts = "ERROR!";

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Enabled:", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true), UI_Toggle(disabledText = "No", enabledText = "Yes")]
        public bool fuelCellIsEnabled = true;

        // changed from false to true
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Next Fuel Mode", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public void nextFuelMode()
        {
            if (++fuelMode >= ODFC_config.modes.Length)
                fuelMode = 0;

            udft();
        }

        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Previous Fuel Mode", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true)]
        public void previousFuelMode()
        {
            if (--fuelMode < 0)
                fuelMode = ODFC_config.modes.Length - 1;

            udft();
        }
        /*
        future: convert rateLimit and threshold to use 
        KSP 1.7.1 Added a new type for PAW fields, a double slider to set ranges with a min and max values
        https://kerbalspaceprogram.com/api/class_u_i_part_action_min_max_range.html
        TMPro.TMP_InputField UIPartActionMinMaxRange.inputFieldMax
        TMPro.TMP_InputField UIPartActionMinMaxRange.inputFieldMin
        */
   
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rate Limit:", guiFormat = "P0", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true), UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
        public float rateLimit = 1f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Threshold:", guiFormat = "P0", groupName = "ODFC", groupDisplayName = "On Demand Fuel Cells", groupStartCollapsed = true), UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
        public float threshold = thresholdMin;

        [KSPAction("Toggle")]
        public void toggleAction(KSPActionParam kap)
        {
            fuelCellIsEnabled = !fuelCellIsEnabled;
        }

        [KSPAction("Enable")]
        public void enableAction(KSPActionParam kap)
        {
            fuelCellIsEnabled = true;
        }

        [KSPAction("Disable")]
        public void disableAction(KSPActionParam kap)
        {
            fuelCellIsEnabled = false;
        }

        [KSPAction("Next Fuel Mode")]
        public void nextFuelmodeAction(KSPActionParam kap)
        {
            nextFuelMode();
        }

        [KSPAction("Previous Fuel Mode")]
        public void previousFuelModeAction(KSPActionParam kap)
        {
            previousFuelMode();
        }

        [KSPAction("Decrease Rate Limit")]
        public void decreaseRateLimitAction(KSPActionParam kap)
        {
            rateLimit = Math.Max(rateLimit - thresHoldSteps, thresholdMin);
        }

        [KSPAction("Increase Rate Limit")]
        public void increaseRateLimitAction(KSPActionParam kap)
        {
            rateLimit = Math.Min(rateLimit + thresHoldSteps, thresHoldMax);
        }

        [KSPAction("Decrease Threshold")]
        public void decreaseThresholdAction(KSPActionParam kap)
        {
            threshold = Math.Max(threshold - thresHoldSteps, thresholdMin);
        }

        [KSPAction("Increase Threshold")]
        public void increaseThresholdAction(KSPActionParam kap)
        {
            threshold = Math.Min(threshold + thresHoldSteps, thresHoldMax);
        }

// add action for action groups
        [KSPAction("Toggle Fuel Auto Mode")]
        public void toggleAutoSwitch(KSPActionParam kap)
        {
            HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().autoSwitch = !HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().autoSwitch;
        }
        #endregion

        #region Private Functions
        private void udfs(out string s, Fuel[] fuels)
        {
            // DEBUG
            //ScreenMessages.PostScreenMessage("a: " + fuels.Length.ToString(), 1, ScreenMessageStyle.LOWER_CENTER, true);

            if (fuels.Length == 0)
            {
                s = "None";
                return;
            }

            s = "";
            bool plus = false;

            foreach (Fuel fuel in fuels)
            {
                if (plus)
                    s += " + ";
                // add code to verify found exists to prevent nullref
                plus = true;
                resourceLa abr = lastResource.Find(x => x.resourceID == fuel.resourceID);

                s += PartResourceLibrary.Instance.GetDefinition(fuel.resourceID).name;
                
                // DEBUG
                //ScreenMessages.PostScreenMessage(s, 1, ScreenMessageStyle.UPPER_CENTER, true);
            }
        }

        private void udft()
        { //udft?
            udfs(out fuel_consumption, ODFC_config.modes[fuelMode].fuels);
            udfs(out byproducts, ODFC_config.modes[fuelMode].byproducts);
        }

        private void UpdateState(states newstate, Double gen, Double max)
        {
            if (gen != lastGen || max != lastMax)
            {
                lastGen = gen;
                lastMax = max;

                ECs_status = gen.ToString(FuelTransferFormat) + " / " + max.ToString(FuelTransferFormat);
            }

            if (state != newstate)
            {
                state = newstate;

                switch (state)
                {
                    case states.fuelDeprived:
                        {
                            status = "Fuel Deprived";
                            break;
                        }
                    case states.noDemand:
                        {
                            status = "No Demand";
                            break;
                        }
                    case states.nominal:
                        {
                            status = "Nominal";
                            break;
                        }
                    case states.off:
                        {
                            status = "Off";
                            break;
                        }
                    case states.stalled:
                        {
                            status = "Stalled";
                            break;
                        }
#if DEBUG
                    default:
                        {
                            status = "ERROR!";
                            break;
                        }
#endif
                }
            }

            Double tf = gen / max * rateLimit;
            if (tf != lastTF || fuelMode != lastFuelMode)
            {
                lastTF = tf;
                lastFuelMode = fuelMode;
            }
        }

        private string GetResourceRates(ConfigNode node)
        {
            if (node == null || node.values.Count < 1)
                return "\n - None";

            string resourceRates = "";

            foreach (ConfigNode.Value value in node.values)
            {
                float rate = float.Parse(value.value);
                string sfx = "/s";

                if (rate <= 0.004444444f)
                {
                    rate *= 3600;
                    sfx = "/h";
                }
                else if (rate < 0.2666667f)
                {
                    rate *= 60;
                    sfx = "/m";
                }

                resourceRates += "\n - " + value.name + ": " + rate.ToString() + sfx;
            }

            return resourceRates;
        }

        private void kommit(Fuel[] fuels, Double adjr)
        {
            foreach (Fuel fuel in fuels)
                part.RequestResource(fuel.resourceID, fuel.rate * adjr);
        }
#endregion

#region Public Functions
        public override void OnLoad(ConfigNode configNode)
        {
            if (string.IsNullOrEmpty(scn))
            {
                this.configNode = configNode;           // Needed for GetInfo()
                scn = configNode.ToString();    // Needed for marshaling
            }
        }

        public override void OnStart(StartState state)
        {

            if (ElectricChargeID == default(int))
                ElectricChargeID = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;

            configNode = ConfigNode.Parse(scn).GetNode("MODULE");
            ODFC_config = new cfg(configNode, part);

            // One puppy will explode for every question you ask about this code.  Please, think of the puppies.

            udft();

            // everything off unless more than one mode
            Events["nextFuel"].guiActive = false;
            Events["nextFuel"].guiActiveEditor = false;
            Events["previousFuel"].guiActive = false;
            Events["previousFuel"].guiActiveEditor = false;
            Actions["previousFuelModeAction"].active = false;
            Actions["nextFuelModeAction"].active = false;

            Fields["fuel_consumption"].guiActive = true; //false;
            Fields["fuel_consumption"].guiActiveEditor = true; // false;

            // if two modes, turn on next only
            if (ODFC_config.modes.Length > 1)
            {
                Events["nextFuel"].guiActive = true;
                Events["nextFuel"].guiActiveEditor = true;
                Actions["nextFuelModeAction"].active = true;
            }
            // if more than two modes, turn on prev
            else if (ODFC_config.modes.Length >2)
            {
                Events["previousFuel"].guiActive = true;
                Events["previousFuel"].guiActiveEditor = true;
                Actions["previousFuelModeAction"].active = true;
            }

/*            if (ODFC_config.modes.Length < 2)
            {   // Disable unnecessary UI elements if we only have a single mode
                Events["nextFuel"].guiActive = false;
                Events["nextFuel"].guiActiveEditor = false;

                // added next two lines for v0.0.1.9 because if only one mode, don't need any prev/next functions.
                Events["previousFuel"].guiActive = false;
                Events["previousFuel"].guiActiveEditor = false;

                Actions["previousFuelModeAction"].active = false;
                Actions["nextFuelModeAction"].active = false;

                Fields["fuel_consumption"].guiActive = true; //false;
                Fields["fuel_consumption"].guiActiveEditor = true; // false;
            }
            else
            {                       // If we have at least 2 modes
                if (ODFC_config.modes.Length > 1)
                {       // If we have at least 3 modes
                    Events["previousFuelMode"].guiActive = true;
                    Events["previousFuelMode"].guiActiveEditor = true;
                }
                else
                {                           // If we have exactly 2 modes
                    Actions["previousFuelModeAction"].active = false;
                }*/

                foreach (mode m in ODFC_config.modes)
                {   // Show byproducts tweakable if at least one mode has at least one byproduct
                    if (m.byproducts.Length > 0)
                    {
                        Fields["byproducts"].guiActive = true;
                        Fields["byproducts"].guiActiveEditor = true;
                        break;
                    }
                }
           // }

            if (state != StartState.Editor)
                part.force_activate();
        }

        public override string GetInfo()
        {
            // this is what is show in the editor
            // As annoying as it is, pre-parsing the config MUST be done here, because this is called during part loading.
            // The config is only fully parsed after everything is fully loaded (which is why it's in OnStart())
            if (info == string.Empty)
            {
                ConfigNode[] mds = configNode.GetNodes("MODE");
                info += "Modes: " + mds.Length.ToString();

                for (byte n = 0; n < mds.Length; n++)
                    info += "\n\n<color=#99FF00FF>Mode: " + n.ToString() + "</color> - Max EC: " + mds[n].GetValue("MaxEC") +
                        "/s\n<color=#FFFF00FF>Fuels:</color>" + GetResourceRates(mds[n].GetNode("FUELS")) +
                        "\n<color=#FFFF00FF>Byproducts:</color>" + GetResourceRates(mds[n].GetNode("BYPRODUCTS"));
            }

            ScreenMessages.PostScreenMessage(info, 1, ScreenMessageStyle.UPPER_CENTER, true);
            Debug.Log(info);
            return info;
        }

        public override void OnFixedUpdate()
        {
            states ns = fuelCellIsEnabled ? states.nominal : states.off;

            if (ns != states.nominal)
            {
                UpdateState(ns, 0, 0);
                return;
            }

            Double amount = 0, maxAmount = 0;
            List<PartResource> resources = new List<PartResource>();
            part.GetConnectedResourceTotals(ElectricChargeID, out amount, out maxAmount);

            foreach (PartResource resource in resources)
            {
                maxAmount += resource.maxAmount;
                amount += resource.amount;
            }

            Double
                cfTime = TimeWarp.fixedDeltaTime,
                ECNeed = (Double)(maxAmount * threshold - amount),
                fuelModeMaxECRateLimit = ODFC_config.modes[fuelMode].maxEC * rateLimit;

            // add stall code
            if (HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().needsECtoStart && amount == 0f)
            {
                UpdateState(states.stalled, 0, fuelModeMaxECRateLimit);
                return;
            }

            cfTime = Math.Min(cfTime, ECNeed / fuelModeMaxECRateLimit); // Determine activity based on supply/demand

            if (cfTime <= 0)
            {
                UpdateState(states.noDemand, 0, fuelModeMaxECRateLimit);
                return;
            }

            //Boolean Empty = false;

            foreach (Fuel fuel in ODFC_config.modes[fuelMode].fuels) {	// Determine activity based on available fuel
				amount = 0;
                part.GetConnectedResourceTotals(fuel.resourceID , out amount, out maxAmount);

                foreach (PartResource r in resources)
                    amount += r.amount;

                cfTime = Math.Min(cfTime, amount / (fuel.rate * rateLimit));
            }

            if (cfTime == 0) // (Empty) // (Math.Round(cfTime, MidpointRounding.ToEven) == 0)
            {

                UpdateState(states.fuelDeprived, 0, fuelModeMaxECRateLimit);
                
                // this looks for another fuel mode that isn't deprived if autoSwitch == true
                if (HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().autoSwitch) nextFuelMode();
                return;
            }
            Double adjr = rateLimit * cfTime;           // Calculate usage based on rate limiting and duty cycle
            Double ECAmount = fuelModeMaxECRateLimit * cfTime;
            part.RequestResource(ElectricChargeID, -ECAmount);   // Don't forget the most important part

            kommit(ODFC_config.modes[fuelMode].fuels, adjr);            // Commit changes to fuel used
            kommit(ODFC_config.modes[fuelMode].byproducts, adjr);           // Handle byproducts

            UpdateState(states.nominal, ECAmount / TimeWarp.fixedDeltaTime, fuelModeMaxECRateLimit);

            updateGroupLabel(status, " - EC: " + ECs_status);

        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                Double newMax = ODFC_config.modes[fuelMode].maxEC * rateLimit;

  		
				if(lastMax != newMax) {
					lastMax = newMax;
					maxECs_status = lastMax.ToString(FuelTransferFormat);
				}

                states newState = fuelCellIsEnabled ? states.nominal : states.off;
                UpdateState(newState, newState == states.nominal ? 1 : 0, 1);

			}
            updateGroupLabel(status, " - EC: " + ECs_status);
		}

        private void updateGroupLabel(string statusStr, string newDisplayName)
        {
            string colorStr = "<#ADFF2F>";

            switch (state)
            {
                case states.fuelDeprived:
                {
                    colorStr = "<color=yellow>";
                    break;
                }
                case states.noDemand:
                {
                    colorStr = "<#6495ED>"; // blue
                    break;
                }
                case states.nominal:
                {
                    colorStr = "<#ADFF2F>"; // green
                    break;
                }
                case states.off:
                {
                    colorStr = "<color=black>"; // black
                    break;
                }
                case states.stalled:
                {
                    colorStr = "<color=red>";
                    break;
                    }
                case states.error:
                    {
                        colorStr = "<color=orange>";
                        break;
                    }
#if DEBUG
                default:
                {
                    colorStr = "<color=orange>";
                    break;
                }
#endif
            }

            UIPartActionWindow window = UIPartActionController.Instance.GetItem(part, false);
            UIPartActionGroup group = window.parameterGroups["ODFC"];
            bool collapsed = GameSettings.PAW_COLLAPSED_GROUP_NAMES.Contains("ODFC");
            group.Initialize("ODFC", colorStr + "ODFC: " + statusStr + newDisplayName + "</color>", false, window);

        }
#endregion
	}

}
            /*            UIPartActionWindow window = UIPartActionController.Instance.GetItem(part, false);
                        UIPartActionGroup group = window.parameterGroups["ODFC"];
                        bool collapsed = false; //GameSettings.collpasedPAWGroups.Contains("ODFC");
                        group.Initialize("ODFC", newDisplayName, collapsed, window);*/
