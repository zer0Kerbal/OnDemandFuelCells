// **
// * Based on the ODFC mod by Orum for Kerbal Space Program.
// * 
// * 
// * 
// * 
// * 
// * 
// * 
// */

#region Current Changes:
/* 
     * resourceLa -> ResourceLabel (string)
     * HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().globalScalingFacotr (kommit)
     * 
     *  public static int ElectricChargeID -> private static int ElectricChargeID;
     *  public double CurrentVesselChargeState; and supporting code
     *  
     *  to add Vessel EC % display in PAW - eventually merge into another line?
     *  
     *  added additional debug.log code
     *  
     *  added private const string GroupName = "ODFC";
     *  changed groupName = "ODFC" --> groupName = GroupName
     *  change string status --> statusStr
     *  comment out private bool ns = true
       
     *   No need to guard the code under "#if DEBUG", as the ConditionalAtribute automatically omits the code when DEBUG is not active - and without breaking the callers! :)
     
     * better logging code use string placeholders {}, 0
      
     * Fixed the Solution file (the DEBUG modes were short-circuited to Release 

     *  to be used to format the PAW consumption/production rate
     *  private const string FuelRateFormat = "0.######";
     
     *  changed 
            thresHoldMax = 1 to 0.85f

     *  added to FixedUpdate() - REMOVE WHEN RELEASING
        try
        {
            base.OnFixedUpdate();
            ...
        }
        catch (Exception e) { DebugLog(m: e); }
     *  
     *  added basic backgroundProcessing code structure and supporting docs
     *  
     *  hide fuel mode in PAW
     *  
     *  
*/

#endregion
#region ROADMAP TODO:
/*
NEW:
    * add scaleFactor to part.cfg
    * add scaleFactor to PAW with slider (internal module scaling)
        * EDITOR only 
        * min 0.05 
        * max 1000
        * step 0.05
        * have both < > and << >> buttons and allow for freeScale
+   * add globalScalingFactor to Settings
        * add to PAW?
    * add Math - (part.cfg - PAW) * globalScalingFactor (Settings)
+    * double globalScalingFactor
+        * Difficulty settings:
+            * Easy = x globalScalingFacotr = 2.0 
+            * Normal = x globalScalingFacotr = 1.0
+            * Moderate = x globalScalingFacotr = 0.75
+            * Hard = x globalScalingFacotr = 0.5
    * allows for the scaling (up/down) of (fuel) cell module without scaling the part attached to
    * add * scaleFactor to code
    * only available in part and only in editor.
    * stretch - should affect the mass/cost
    * really stretchy - limited by tech tree
    * would enable one set of 'standard' modes (still have to supply for each part) and just 'scale' everything
        * so standard set of consumptions for producing 1 EC/s
        * scaleFactor then set to default of 1
        * if the module is supposed to produce 10 EC/s set scaleFactor to 10
        * conversely, if module is supposed to produce 0.5 EC/s; set scaleFactor to 10

// Most Wanted:
+   * PAW isn't showing consumption / production fuel_consumption and byproducts
        * add formatting to be private const string FuelTransferFormat = "0.######"; //FuelTransferFormat?
    * fix showing 'next' button when there is only one mode of operation
    * add ANSI vessel EC % graphical display to PAW (just for fun)
    * 

NEXT BIG THING (Project):
    * Background Processing 

* implement double slider like in B9Partswitchsupport
    * UI_MinMaxRange(
    * KSPAxisField(
    * minThreshold is when ODFC will attempt to start (50%)
    * maxThreshold is when ODFC will rate limit production (85%)
    * minProduction is the minimum EC/s ODFC will produce (0.01%) (idle)
    * maxProduction is the maximim EC/s ODFC will produce (100%)
    * 

* DONE: implement PAW statusStr in group header
* DONE: add page to game difficulty settings
* DONE: add stall variable and code
* DONE: implement 'stalled' mode - with a setting in the difficulty settings menu: this will 'stall' the fuel cell if the vessel (at least reachable) reaches below a certain level of EC (like <= 0),
* DONE: will not reset until the vessel has at least 0.5 EC
* DONE: implement and add autoSwitch fuel deprived auto mode switcher

add to part module pulled from MODULE config nodes(use FSHORT code to read in)
//MODULE variables
double threshold = 0.05f, //thresHoldSteps
        rateLimit = 1;

byte defaultMode = 1;

bool autoSwitch = false,
          enabled = true,
          ProducesHeat = false

* bool UseSpecialistBonus = true
	*	SpecialistEfficiencyFactor = 0.2
	*	SpecialistBonusBase = 0.05
	*	Specialty = Engineer
	*	EfficiencyBonus = 1

eventually want to add the following for each fuel/ byproducts:
 per FUEL / BYPRODUCT:
     double  reserveAmount = 0.0f, //(fuels)
             maximumAmount = 1.00f; // (byproducts)

bool ventExcess = True(byproducts, vent excess over maximum Amount)
    // flowMode = All;
 */
#endregion

// #define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODFC
{
    /// <summary>
    /// On Demand Fuel Cells (ODFC) part module
    /// </summary>
    /// <seealso cref="PartModule" />
    public class ODFC : PartModule
    {
#region Enums Vars
        /// <summary>
        /// States: enum list of fuel cell operational states
        /// </summary>
        public enum states : byte { error, off, nominal, fuelDeprived, noDemand, stalled }; // deploy, retract,        
        /// <summary>
        /// The states string: ERROR!, Off, Nominal, Fuel Deprived, No Demand, Stalled
        /// </summary>
        // Thank you to (Lisias) for this upgrade
        private static readonly string[] STATES_STR = { "ERROR!", "Off", "Nominal", "Fuel Deprived", "No Demand", "Stalled" };
        /// <summary>
        /// The states color: orange, black, green, blue, red
        /// </summary>
        // Thank you to (Lisias) for this upgrade
        private static readonly string[] STATES_COLOUR = { "<color=orange>", "<color=black>", "<#ADFF2F>", "<color=yellow>", "<#6495ED>", "<color=red>" };
        //                                                                                      (Green)                          (Blue)

        /// <summary>
        /// The fuel transfer format
        /// </summary>
        private const string FuelTransferFormat = "0.##";

        /// <summary>
        /// The fuel rate format
        /// this formats the fuel/byproducts rate display in the PAW
        /// </summary>
        private const string FuelRateFormat = "0.########";

        /// <summary>
        /// The PAW group name
        /// </summary>
        private const string GroupName = "KGEx.ODFC";

        private const float
            thresHoldSteps = 0.05f, // increment the rate by this amount (default is 5)
            thresholdMin = thresHoldSteps,
            thresHoldMax = 1f; // 0.85f;

        private Double
            lastGen = -1,
            lastMax = -1,
            lastTF = -1;

        /// <summary>
        /// The last fuel mode
        /// </summary>
        private int lastFuelMode = -1;

        /// <summary>
        /// Editor module information
        /// </summary>
        private string info = string.Empty;

        /// <summary>The configuration node</summary>
        public ConfigNode configNode;

        /// <summary>List: The last resource</summary>
        public static List<ResourceLabel> lastResource = new List<ResourceLabel>();

        /// <summary>ElectricCharge identification number</summary>
        public static int ElectricChargeID;

        /// <summary>
        /// vessel's current total ElectricCharge(EC)
        /// </summary>
        public PartResource _electricCharge;

        /// <summary>The SCN</summary>
        public string scn;

        /// <summary>The ns</summary>
        // private bool ns = true;

        /// <summary>The odfc configuration</summary>
        public Config ODFC_config;

        /// <summary>The state of the Fuel Cell (nominal, off et al)</summary>
        public states state = states.error;
#endregion

        // added PAW grouping, set to autocollapse - introduced in KSP 1.7.1
        // would really like the PAW to remember if the group was open
#region Fields Events Actions

        /*
        /// <summary>scaleFactor is optional scaling set in part.config module declaration</summary>
        [KSPField(isPersistant = true, 
            guiActive = false,
            guiActiveEditor = true,
            guiName = "scaleFactor")]
        private double scaleFactor = 1f; // allows for scaling of ODFC elements
        */

        [KSPField(isPersistant = false,
            guiActive = true,
            guiActiveEditor = true,
            guiName = " ")]
        public string PAWStatus = "ODFC: booting up. FCOS 0.42... ";

        [KSPField(isPersistant = true,
            guiActive = true,
            guiActiveEditor = true,
            groupName = GroupName,
            groupDisplayName = "On Demand Fuel Cells Control",
            groupStartCollapsed = true)]
        public int fuelMode = 0;

        [KSPField(isPersistant = false,
            guiActive = false,
            guiActiveEditor = false,
            guiName = "Status",
            groupName = GroupName)]
        public string statusStr = "ERROR!";

        // for display purposes only    
        /// <summary>
        ///   The current ElectricCharge %
        /// </summary>
        // [UI_Label(scene = UI_Scene.All)]
        [KSPField(
           // advancedTweakable = true,
            isPersistant = false,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Vessel EC %",
            guiFormat = "F2",
            guiUnits = "%",
            groupName = GroupName)]
        public double CurrentVesselChargeState;

        [KSPField(isPersistant = false,
            guiActive = true,
            guiActiveEditor = false,
            guiName = "EC/s Production (cur/max)",
            groupName = GroupName)]
        public string ECs_status = "ERROR!";

        [KSPField(isPersistant = false,
            guiActive = false,
            guiActiveEditor = true,
            guiName = "Max EC/s",
            groupName = GroupName)]
        public string maxECs_status = "ERROR!";

        [KSPField(isPersistant = false,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Fuels:",
            groupName = GroupName)]
        public string fuel_consumption = "ERROR!";

        [KSPField(isPersistant = false,
            guiActive = false,
            guiActiveEditor = false,
            guiName = "Byproducts:", //"\nByproducts:",
            groupName = GroupName)]
        public string byproducts = "ERROR!";

        [KSPField(isPersistant = true,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Enabled:",
            groupName = GroupName),
            UI_Toggle(disabledText = "No",
            enabledText = "Yes")]
        public bool fuelCellIsEnabled = true;

        // changed from false to true
        [KSPEvent(guiActive = true,
            guiActiveEditor = true,
            guiName = "Next Fuel Mode",
            groupName = GroupName)]
        public void nextFuelMode()
        {
            if (++fuelMode >= ODFC_config.modes.Length)
                fuelMode = 0;

            UpdateEditor(); // updateFT();
        }

        [KSPEvent(guiActive = false,
            guiActiveEditor = false,
            guiName = "Previous Fuel Mode",
            groupName = GroupName)]
        public void previousFuelMode()
        {
            if (--fuelMode < 0)
                fuelMode = ODFC_config.modes.Length - 1;

            UpdateEditor(); // updateFT();
        }
        /*
        future: convert rateLimit and threshold to optionally be set in both Settings page and be overridden by hard values in part.cfg
        KSP 1.7.1 Added a new type for PAW fields, a double slider to set ranges with a min and max values
        UI_MinMaxRange
        */
        /// <summary>The rate limit
        /// (max % production)</summary>
        [KSPField(isPersistant = true,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Rate Limit:",
            guiFormat = "P0",
            groupName = GroupName),
            UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
        public float rateLimit = 1f;

        /// <summary>
        /// The current threshold (%) which needs to be equal or below before production begins.
        /// </summary>
        [KSPField(isPersistant = true,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Threshold:",
            guiFormat = "P0",
            groupName = GroupName),
            UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
        public float threshold = 0.85f; // thresholdMin;

        /// <summary>
        /// Toggles the fuel cell on/off.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Toggle Fuel Cell")]
        public void toggleAction(KSPActionParam kap)
        { fuelCellIsEnabled = !fuelCellIsEnabled; }

        /// <summary>
        /// Enables the fuel cell.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Enable Fuel Cell")]
        public void enableAction(KSPActionParam kap)
        { fuelCellIsEnabled = true; }

        /// <summary>
        /// Disables the fuel cell.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Disable Fuel Cell")]
        public void disableAction(KSPActionParam kap)
        { fuelCellIsEnabled = false; }

        /// <summary>
        /// Nexts (fuel) mode action.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Next Fuel Mode")]
        public void nextFuelmodeAction(KSPActionParam kap)
        { nextFuelMode(); }

        /// <summary>
        /// Previouses the (fuel) mode action.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Previous Fuel Mode")]
        public void previousFuelModeAction(KSPActionParam kap)
        { previousFuelMode(); }

        /// <summary>
        /// Decreases the rate limit action.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Decrease Rate Limit")]
        public void decreaseRateLimitAction(KSPActionParam kap)
        { rateLimit = Math.Max(rateLimit - thresHoldSteps, thresholdMin); }

        /// <summary>
        /// Increases the rate limit action.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Increase Rate Limit")]
        public void increaseRateLimitAction(KSPActionParam kap)
        { rateLimit = Math.Min(rateLimit + thresHoldSteps, thresHoldMax); }

        /// <summary>
        /// Decreases the threshold action.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Decrease Threshold")]
        public void decreaseThresholdAction(KSPActionParam kap)
        { threshold = Math.Max(threshold - thresHoldSteps, thresholdMin); }

        /// <summary>
        /// Increases the threshold action.
        /// </summary>
        /// <param name="kap">The kap.</param>
        [KSPAction("Increase Threshold")]
        public void increaseThresholdAction(KSPActionParam kap)
        { threshold = Math.Min(threshold + thresHoldSteps, thresHoldMax); }
        #endregion

#region Private Functions
        /// <summary>Updates the Fuels String.</summary>
        /// <param name="s">The string to be used to update the fuels list.</param>
        /// <param name="fuels">The fuels list.</param>
        public static void UpdateFuelsString(out string s, Fuel[] fuels)
        {

            string fuelColorStr = "";
            string fuelRateColorStr = "";
            string endStr = "";

            // would like to have and if s = fuel_consumption then else byproducts then 
            // ie different colors for fuel_consumption and byproducts
            if (HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().coloredPAW)
            {
                fuelColorStr = "<#FFFF00>";
                fuelRateColorStr = "<#FFFF00>";
                endStr = "</color>";
            }

            if (fuels.Length == 0)
            {
                s = "None";
                return;
            }

            s = "";
           // bool plus = false;

            foreach (Fuel fuel in fuels)
            {
                ResourceLabel abr = lastResource.Find(x => x.resourceID == fuel.resourceID);
                /*
                if (plus)
                {
                    s += " "; // /s
                    plus = true;
                }
                */

                // THIS IS WHERE have to add code to include consumption/production #
                s += "\n" + fuelColorStr + PartResourceLibrary.Instance.GetDefinition(fuel.resourceID).name + ": " + fuelRateColorStr + RateString(fuel.rate) + endStr;
                // add code to verify found exists to prevent nullref 
                Log.dbg("[ODFC PAW] Fuels (string): " + s);
            }
            //s += "\n";
        }

        /// <summary>
        /// Formats fuel rate into a string
        /// first converts into /s /m /h
        /// and then into string
        /// </summary>
        /// <param name="Rate">The rate.</param>
        /// <returns></returns>
        public static string RateString(double Rate)
        {
            //  double rate = double.Parse(value.value);
            string sfx = "/s";

            if (Rate <= 0.004444444f)
            {
                Rate *= 3600;
                sfx = "/h";
            }
            else if (Rate < 0.2666667f)
            {
                Rate *= 60;
                sfx = "/m";
            }

            // limit decimal places to 10 and add sfx
            //return String.Format(FuelRateFormat, Rate, sfx);
            return Rate.ToString() + sfx;
        }

        /// <summary>Updates the FT (FuelType?).</summary>
        public void UpdateFT()
        { //updateFT?
            UpdateFuelsString(out fuel_consumption, ODFC_config.modes[fuelMode].fuels);
            UpdateFuelsString(out byproducts, ODFC_config.modes[fuelMode].byproducts);
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="newstate">The newstate.</param>
        /// <param name="gen">The gen.</param>
        /// <param name="max">The maximum.</param>
        public void UpdateState(states newstate, Double gen, Double max)
        {
            if (gen != lastGen || max != lastMax)
            {
                lastGen = gen;
                lastMax = max;

                //ECs_status = double.ToString(FuelTransferFormat, gen) + " / " + double.ToString(FuelTransferFormat, max);
                ECs_status = gen.ToString(FuelTransferFormat) + " / " + max.ToString(FuelTransferFormat);
            }

            if (state != newstate)
            {
                state = newstate;
                statusStr = STATES_STR[(int)state];
            }

            Double tf = gen / max * rateLimit;
            if (tf != lastTF || fuelMode != lastFuelMode)
            {
                lastTF = tf;
                lastFuelMode = fuelMode;
            }
            UpdatePAWLabel();
        }

        /// <summary>
        /// Gets the resource rates.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public string GetResourceRates(ConfigNode node)
        {
            if (node == null || node.values.Count < 1)
                return "\n - None";

            string resourceRates = "";

            foreach (ConfigNode.Value value in node.values)
            {
                double rate = double.Parse(value.value);
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

                // resourceRates = String.Format("\n - {0} : {1}{2,15:0.###########}", value.name, rate.ToString(), sfx);
                resourceRates += "\n - " + value.name + ": " + rate.ToString() + sfx;
            }

            return resourceRates;
        }

        /// <summary>
        /// Kommits the changes to specified fuels from operating the (fuel) cell.
        /// This is the meat and potatoes of this module - it actually does the heavy lifting.
        /// </summary>
        /// <param name="fuels">Which fuels are adjusted.</param>
        /// <param name="adjr">The adjustement.</param>
        private void Kommit(Fuel[] fuels, Double adjr)
        {
            foreach (Fuel fuel in fuels)
                part.RequestResource(fuel.resourceID, fuel.rate * adjr);
                // part.RequestResource(fuel.resourceID, fuel.rate * adjr * HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().globalScalingFactor);
        }
        #endregion

#region Public Functions
        /// <summary>Called when part is added to the craft.</summary>
        public override void OnAwake()
        {
            Log.dbg("OnAwake for {0}", this.name);
        }
        
        /// <summary>Called when part is loaded [load].</summary>
         /// <param name="configNode">The configuration node.</param>
        public override void OnLoad(ConfigNode configNode)
        {
            if (string.IsNullOrEmpty(scn))
            {
                this.configNode = configNode;           // Needed for GetInfo()
                scn = configNode.ToString();            // Needed for marshalling
            }
        }

        /// <summary>
        ///   Called by unity API on game start.
        /// </summary>
        /// <param name="state">The state.</param>
        public override void OnStart(StartState state)
        {
            Log.dbg("OnStart {0}", state);

            // obtain the ElectricCharge (EC) resourceID
            if (ElectricChargeID == default(int))
                ElectricChargeID = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;

            // obtain the EC resource
            // warn if not found, this module cannot function without.
            _electricCharge = part.Resources?.Get(name: "ElectricCharge");
            if  (_electricCharge == null) Log.dbg("[Error: ODFC failed to obtain ElectricCharge resource ID" +  _electricCharge);

// One puppy will explode for every question you ask about this code.  Please, think of the puppies.

				UpdateConfig();

            if (state != StartState.Editor)
                part.force_activate();
        }

        /// <summary>Formats the information for the part information in the editors.</summary>
        /// <returns>info</returns>
        public override string GetInfo()
        {
            // this is what is show in the editor
            // As annoying as it is, pre-parsing the config MUST be done here, because this is called during part loading.
            // The config is only fully parsed after everything is fully loaded (which is why it's in OnStart())
            //if (info == string.Empty)
            if (string.IsNullOrEmpty(info))
            {
                ConfigNode[] mds = configNode.GetNodes("MODE");
                info += "Modes: " + mds.Length.ToString();

                for (byte n = 0; n < mds.Length; n++)
                    info += "\n\n<color=#99FF00>Mode: " + n.ToString() + "</color> - Max EC: " + mds[n].GetValue("MaxEC") +
                        "/s\n<color=#FFFF00>Fuels:</color>" + GetResourceRates(mds[n].GetNode("FUELS")) +
                        "\n<color=#FFFF00>Byproducts:</color>" + GetResourceRates(mds[n].GetNode("BYPRODUCTS"));
            }

            return info;
        }

        private void UpdateConfig()
        {
            Log.dbg("Updating config");

            ODFC_config = new Config(ConfigNode.Parse(scn).GetNode("MODULE"), part);
            Log.dbg("Modes.Length: {0}", ODFC_config.modes.Length);
            UpdateFT();
            UpdatePAWLabel();

            if (ODFC_config.modes.Length < 2)
            {   // Disable unnecessary UI elements if we only have a single mode
                Events["nextFuel"].guiActive = false;
                Events["nextFuel"].guiActiveEditor = false;
                Fields["fuel_consumption"].guiActive = true; // false;
                Fields["fuel_consumption"].guiActiveEditor = true; // false;
                Actions["previousFuelModeAction"].active = false;
                // Actions["nextFuelModeAction"].active = false;
            }
            else
            {                       // If we have at least 2 modes
                if (ODFC_config.modes.Length > 2)
                {       // If we have at least 3 modes
                    Events["previousFuelMode"].guiActive = true;
                    Events["previousFuelMode"].guiActiveEditor = true;
                }
                else
                {                           // If we have exactly 2 modes
                    Actions["previousFuelModeAction"].active = false;
                }

                foreach (mode m in ODFC_config.modes)
                {   // Show byproducts tweakable if at least one mode has at least one byproduct
                    if (m.byproducts.Length > 0)
                    {
                        Fields["byproducts"].guiActive = true;
                        Fields["byproducts"].guiActiveEditor = true;
                        break;
                    }
                }
            }
		  }

        /// <summary>Called when [fixed update].</summary>
			public override void OnFixedUpdate()
			{
				if (_electricCharge == null ) //|| _resourceConverter == null)
				{
					Log.dbg("Something is null");
					return; // already checked for this in OnStart()
				}
				CurrentVesselChargeState = _electricCharge.amount / _electricCharge.maxAmount * 100; // compute current EC, in percent
				//Log.dbg("Current Charge = {CurrentVesselChargeState:F2}" + _electricCharge.amount + " / " + _electricCharge.maxAmount * 100);
				// spams the log

				states ns = fuelCellIsEnabled ? states.nominal : states.off;

				if (ns != states.nominal)
				{
					UpdateState(ns, 0, 0);
					return;
				}

				Double amount = 0, maxAmount = 0;
				part.GetConnectedResourceTotals(ElectricChargeID, out amount, out maxAmount);

				foreach (PartResource resource in this.part.Resources)
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

				// Determine activity based on supply/demand
				cfTime = Math.Min(cfTime, ECNeed / fuelModeMaxECRateLimit);
				if (cfTime <= 0)
				{
					UpdateState(states.noDemand, 0, fuelModeMaxECRateLimit);
					return;
				}

				// Determine activity based on available fuel
				foreach (Fuel fuel in ODFC_config.modes[fuelMode].fuels)
				{
					amount = 0;
					part.GetConnectedResourceTotals(fuel.resourceID, out amount, out maxAmount);

					foreach (PartResource r in this.part.Resources) 
						amount += r.amount;

					cfTime = Math.Min(cfTime, amount / (fuel.rate * rateLimit));
				}

                // Determine activity based on available space to put byproduct
                foreach (Fuel byproduct in ODFC_config.modes[fuelMode].byproducts)
                {
                // future code
                }

				if (cfTime == 0)
				{
					UpdateState(states.fuelDeprived, 0, fuelModeMaxECRateLimit);
					// this looks for another fuel mode that isn't deprived if autoSwitch == true
					if (HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().autoSwitch) nextFuelMode();
					return;
				 }

				// Calculate usage based on rate limiting and duty cycle
				Double adjr = rateLimit * cfTime;
				Double ECAmount = fuelModeMaxECRateLimit * cfTime;

				// Don't forget the most important part (add ElectricCharge (EC))
				part.RequestResource(ElectricChargeID, -ECAmount);

				// Commit changes to fuel used
				Kommit(ODFC_config.modes[fuelMode].fuels, adjr);

				// Handle byproducts
				Kommit(ODFC_config.modes[fuelMode].byproducts, adjr);

				UpdateState(states.nominal, ECAmount / TimeWarp.fixedDeltaTime, fuelModeMaxECRateLimit);
			}

        /// <summary>Updates this instance.</summary>
        public void Update()
        {
            if (HighLogic.LoadedSceneIsEditor) {
                Double newMax = ODFC_config.modes[fuelMode].maxEC * rateLimit;

                if (lastMax != newMax) {
                    lastMax = newMax;
                    maxECs_status = lastMax.ToString(FuelTransferFormat);
                }

                states newState = fuelCellIsEnabled ? states.nominal : states.off;
                UpdateState(newState, newState == states.nominal ? 1 : 0, 1);
            }
        }
#endregion
#region Update PAW code
        /// <summary>
        /// Updates the PAW label.
        /// </summary>
        private void UpdatePAWLabel()
        {
            string colorStr = "<#ADFF2F>";
            string begStr = "<size=+1><b>";
            string endStr = "</color></b></size>";

            if (HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().coloredPAW)
            { colorStr = STATES_COLOUR[(int)state]; }

//            if (HighLogic.LoadedSceneIsFlight) PAWStatus = begStr + colorStr + "Fuel Cell: " + statusStr + " - " + ECs_status + " EC/s" + endStr;
            // if (HighLogic.LoadedSceneIsFlight) PAWStatus = ("{0}{1}Fuel Cell: {2} - {3} EC/s{4}", begStr, colorStr, statusStr, ECs_status, endStr);
            if (HighLogic.LoadedSceneIsEditor) PAWStatus = begStr + colorStr + "Fuel Cell: " + fuel_consumption + " - " + maxECs_status + " EC/s:" + endStr;
            if (HighLogic.LoadedSceneIsEditor) PAWStatus = begStr + colorStr + "Fuel Cell: " + statusStr + " - " + maxECs_status + " EC/s:" + endStr;
        }
#endregion
#region TweakScale Support
        /// <summary>Updates the PAW with scaleFactor and advises KSP that the ship has changed</summary>
        private void UpdateEditor()
        {

            UpdateFT();
            // following needed to advise KSP that the ship has been modified and it needs to update itself. (Lisias)
            GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
        }

        /// <summary>
        ///   <para>
        ///  Called when [rescale].
        /// </para>
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        internal void OnRescale(TweakScale.ScalingFactor.FactorSet scaleFactor)
        {
            Log.dbg("scaleFactor: {0}", scaleFactor);

            /// <summary>this scales any resources on the part with ODFC:  </summary>    
            Log.dbg(" part {0}", this.part);
            Log.dbg(" part.Resources {0}", this.part.Resources);
            foreach (PartResource resource in this.part.Resources)
            {
                Log.dbg("scaleFactor: {0}, {1}", scaleFactor, scaleFactor.quadratic);
                Log.dbg("unscaled resource: {0}: {1} / {2}", resource.resourceName, resource.amount, resource.maxAmount);
                resource.maxAmount *= scaleFactor.quadratic; // .cubic;
                resource.amount *= scaleFactor.quadratic; // cubic;
                Log.dbg("scaled resource: {0}: {1} / {2}", resource.resourceName, resource.amount, resource.maxAmount);
            }

            /// <summary><para>
            /// this scales the actual fuel cell, fuels, byproducts, and maxEC
            /// shouldn't scale rateLimit and threshold because are percentages
            ///</para></summary>
            Log.dbg("ODFC_config {0}", ODFC_config.modes.Length - 1);
            Log.dbg(" ODFC_config.modes {0} / {2}", ODFC_config.modes.Length - 1, ODFC_config.modes); 
            for (int m = 0; m <= ODFC_config.modes.Length - 1; m++)
            {
                Log.dbg("mode/modes: {0} / {1}", (m + 1), ODFC_config.modes.Length);
                // scale MaxEC
                Log.dbg("unscaled maxEC: {0}", ODFC_config.modes[m].maxEC);
                ODFC_config.modes[m].maxEC *= scaleFactor.quadratic;
                Log.dbg("scaled maxEC: {0}", ODFC_config.modes[m].maxEC);

                // scale fuels in ODFC_config.modes
                Log.dbg("Fuels in mode: {0} / {1}", (m + 1), ODFC_config.modes[m].fuels.Length + 1);
                for (int n = 0; n <= ODFC_config.modes[m].fuels.Length - 1; n++)

                {
                    Log.dbg("unscaled Fuel: {0} = {1}", PartResourceLibrary.Instance.GetDefinition(ODFC_config.modes[m].fuels[n].resourceID).name, ODFC_config.modes[m].fuels[n].rate);
                    ODFC_config.modes[m].fuels[n].rate *= scaleFactor.quadratic;
                    Log.dbg("scaled Fuel: {0} = {1}" + PartResourceLibrary.Instance.GetDefinition(ODFC_config.modes[m].fuels[n].resourceID).name, ODFC_config.modes[m].fuels[n].rate);
                }

                // scale byproducts in ODFC_config.modes
                Log.dbg("Byproducts in mode: {0} / {1}", (m + 1), ODFC_config.modes[m].byproducts.Length + 1);
                for (int n = 0; n <= ODFC_config.modes[m].byproducts.Length - 1; n++)
                {
                    Log.dbg("unscaled byproduct: {0} = {1}", PartResourceLibrary.Instance.GetDefinition(ODFC_config.modes[m].byproducts[n].resourceID).name, ODFC_config.modes[m].byproducts[n].rate);
                    ODFC_config.modes[m].byproducts[n].rate *= scaleFactor.quadratic;
                    Log.dbg("scaled byproduct: {0} / {1}", PartResourceLibrary.Instance.GetDefinition(ODFC_config.modes[m].byproducts[n].resourceID).name, ODFC_config.modes[m].byproducts[n].rate);
                }
            }
            this.UpdateEditor(); // updateFT();
        }
#endregion
#region Background Processing Code
        /// <summary>
        /// Background Processing FixedBackgroundUpdate (A)
        /// </summary>
        /// <param name="v"></param>
        /// <param name="partFlightID"></param>
        /// <param name="data"></param>
        public static void FixedBackgroundUpdate(Vessel v, uint partFlightID, ref System.Object data)
        {
            Log.dbg("ODFC BackgroundProcessing: FixedBackgroundUpdate overload a");
        }

        /// <summary>
        /// Background Processing FixedBackgroundUpdate (B)
        /// </summary>
        /// <param name="v"></param>
        /// <param name="partFlightID"></param>
        /// <param name="resourceRequest"></param>
        /// <param name="data"></param>
        public static void FixedBackgroundUpdate(Vessel v, uint partFlightID, Func<Vessel, float, string, float> resourceRequest, ref System.Object data)
        {
            Log.dbg("ODFC BackgroundProcessing: FixedBackgroundUpdate overload b");
        }

        /* These two functions are the 'simple' and 'complex' background update functions. If you implement either of them, BackgroundProcessing will call it 
         * at FixedUpdate() intervals (if you implement both, only the complex version will get called). The function will only be called for unloaded vessels, 
         * and it will be called once per part per partmodule type - so if you have more than one of the same PartModule on the same part you'll only get one 
         * update for all of those PartModules. Vessel v is the Vessel object that you should update - be careful, it's quite likely unloaded and very little 
         * of it is there. partFlightID is the flightID of the Part this update was associated with. resourceRequest is a function that provides an analog of 
         * Part.RequestResource. It takes a vessel, an amount of resource to take, and a resource name, and returns the amount of resource that you got. Like 
         * Part.RequestResource, you can ask for a negative amount of some resource to fill up the tanks. The resource is consumed as if it can be reached from the entire vessel - be very careful with resources like liquid fuel that should only flow through crossfeeds. data is arbitrary per-part-per-partmodule-type storage - you can stash anything you want there, and it will persist between FixedBackgroundUpdate calls. 
        */

        /// <summary>
        /// BackgroundLoad 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="partFlightId"></param>
        /// <param name="data"></param>
        public static void BackgroundLoad(Vessel v, uint partFlightId, ref System.Object data)
        {
            Log.dbg("ODFC BackgroundProcessing: Background Load");
        }

        /* This function will be called once prior to FixedBackgroundUpdate, and it gives you a chance to load data out of the ConfigNodes on the vessel's 
         * protovessel into the storage BackgroundProcessing manages for you.
        */

        /// <summary>
        /// BackgroundSave
        /// </summary>
        /// <param name="v"></param>
        /// <param name="partFlightId"></param>
        /// <param name="data"></param>
        public static void BackgroundSave(Vessel v, uint partFlightId, System.Object data)
        {
            Log.dbg("ODFC BackgroundProcessing: Background Save");
        }
                     
        /* This function will be called prior to the game scene changing or the game otherwise being saved.Use it to persist background data to the vessel's confignodes.
         * Note that System.Object data is *not* a ref type here, unlike the other functions that have it as an argument.
        */
 
        /// <summary>
        /// GetInterestingResources
        /// </summary>
        /// <returns></returns>
        public static List<string> GetInterestingResources()
        {
            Log.dbg("ODFC BackgroundProcessing: GetInterestingResources");
            return null;
        }
                     
        /* Implement this function to return a list of resources that your PartModule would like BackgroundProcessing to handle in the background. It's okay if multiple 
         * PartModules say a given resource is interesting.
        */ 

         /// <summary>
         /// GetBackgroundResourceCount
         /// </summary>
         /// <returns></returns>
        public static int GetBackgroundResourceCount()
        {
            Log.dbg("ODFC BackgroundProcessing: GetBackgroundResourceCount");
            return 0;
        }

        /// <summary>
        /// GetBackgroundResource
        /// </summary>
        /// <param name="index"></param>
        /// <param name="resourceName"></param>
        /// <param name="resourceRate"></param>
        public static void GetBackgroundResource(int index, out string resourceName, out float resourceRate)
        {
            resourceName = "none";
            resourceRate = 0f;
            Log.dbg("ODFC BackgroundProcessing: GetBackgroundResource: " + resourceName + " : " + resourceRate);
            return;
        }

        /* Implement these functions to inform BackgroundProcessing that your PartModule should be considered to produce or consume a resource in the background.
         * GetBackgroundResourceCount() should return the number of different resources that your PartModule produces/consumes.GetBackgroundResource() will then 
         * be called with each index from 0 up to one less than the count you indicated, and you should set resourceName and resourceRate to the appropriate values 
         * for the index-th resource your PartModule produces.resourceRate is the amount of resource your part should produce each second - if your part consumes 
         * resources, it should be negative.If your part only consumes or produces resources in some situation, it's better to implement the complex background 
         * update and use the resource request function.

        How do I distribute a mod that uses BackgroundProcessing?

          * Having multiple copies of the BackgroundProcessing DLL in a GameData directory is fine - only the most recent version will run.If your mod absolutely 
          * needs BackgroundProcessing present to be useful, consider including the BackgroundProcessing DLL in your mod's zip file, the same way ModuleManager is 
          * handled.

        If BackgroundProcessing isn't central to your mod, feel free not to distribute it at all. If players don't have this mod installed, they just won't get your off-rails features.

        Are there any caveats when writing code that uses BackgroundProcessing?

            * BackgroundProcessing works optimally with PartModules that are present in prefab parts.The list of modules that has BackgroundProcessing handling is 
            * constructed by walking all the AvailablePart objects in PartLoader.LoadedPartLists at the main menu.If your partmodule isn't in that set, very little will 
            * work. Similarly, if your PartModule is added to a part's module list after part instantiation, and isn't present in the part's prefab module list, resource 
            * handling will likely not work. This is all only relevant if your PartModule is added dynamically to parts, after construction. It isn't relevant to PartModules 
            * that are present in config files, or to PartModules added by ModuleManager (which appropriately modifies prefab parts). The takeaway is that if you intend 
            * to dynamically add PartModules to parts, and those dynamically-added PartModules should have BackgroundProcessing behaviour, make sure you add them to the 
            * appropriate part prefab before the main menu, like ModuleManager.

        Edited August 3, 2016 by jamespicone
        */

#endregion
    }
}
#region notes
/*            UIPartActionWindow window = UIPartActionController.Instance.GetItem(part, false);
            UIPartActionGroup group = window.parameterGroups["ODFC"];
            bool collapsed = GameSettings.PAW_COLLAPSED_GROUP_NAMES.Contains("ODFC");
            group.Initialize("ODFC", colorStr + "ODFC: " + statusStr + newDisplayName + colorStrEnd, false, window);*/

/*            UIPartActionWindow window = UIPartActionController.Instance.GetItem(part, false);
                    UIPartActionGroup group = window.parameterGroups["ODFC"];
                    bool collapsed = false; //GameSettings.collpasedPAWGroups.Contains("ODFC");
                    group.Initialize("ODFC", newDisplayName, collapsed, window);*/

/*            ScreenMessages.PostScreenMessage(info, 1, ScreenMessageStyle.UPPER_CENTER, true);
            Debug.Log(info);*/
// DEBUG
//ScreenMessages.PostScreenMessage("a: " + fuels.Length.ToString(), 1, ScreenMessageStyle.LOWER_CENTER, true);
#endregion