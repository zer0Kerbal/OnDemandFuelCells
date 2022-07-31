using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;
using UnityEngine.UI;

namespace OnDemandFuelCells
{
    public class ODFC : PartModule
    /// <summary>
    /// On Demand Fuel Cells (ODFC) part module
    /// </summary>
    /// <seealso cref="PartModule" />
    {
#region Enums Vars

        public enum states : byte { error, off, nominal, fuelDeprived, noDemand, stalled }; // deploy, retract,
        // private static readonly string[] STATES_STR = {  Localizer.Format("#ODFC-PAW-err"), "Off", "Nominal", "Fuel Deprived", "No Demand", "Stalled" };
        private static readonly string[] STATES_STR = { "ERROR!", "Off", "Nominal", "Fuel Deprived", "No Demand", "Stalled" };
        private static readonly string[] STATES_COLOUR = { "<color=orange>", "<color=black>", "<#ADFF2F>", "<color=yellow>", "<#6495ED>", "<color=red>" };
        //                                                                                      (Green)                          (Blue)
        //                                                                                                  (Green)                          (Blue)
        private const string FuelTransferFormat = "0.##"; //FuelTransferFormat?

        private const float
            thresHoldSteps = 0.05f, // increment the rate by this amount (default is 5)
            thresholdMin = thresHoldSteps,
            thresHoldMax = 1;

        /// <summary>current counter for responseTime</summary>
        private int timeOut = 1;

        /// <summary>[internal]The fuel mode current ElectricCharge (EC) production. seealso OnDemandFuelCellsEC</summary>
        private double _fuelModeMaxECRateLimit = 0f;
        public double fuelModeMaxECRateLimit = 0f;

        /// <summary>
        /// Gets the On Demand Fuelcells(ODFC) Electric Charge (EC) Production.
        /// AMPYear / JPRepo / Background?
        /// allows AMPYear and others to see current EC/s production
        /// </summary>
        /// <value>
        /// The On Demand Fuelcells(ODFC) Electric Charge (EC) Production.
        /// </value>
        public double OnDemandFuelCellsEC { get { return this._fuelModeMaxECRateLimit; } }

        private double
            lastGen = -1,
            lastMax = -1,
            lastTF = -1;

        private int lastFuelMode = -1;
        /// <summary>Module information shown in editors</summary>
        private string info = string.Empty;

        /// <summary>The configuration node</summary>
        public ConfigNode configNode;

        /// <summary>The internalized ConfigNode String</summary>
        public string ConfigNodeString;

        /// <summary>List: The last resource</summary>
        public static List<ResourceLabel> lastResource = new List<ResourceLabel>();

        /// <summary>ElectricCharge identification number</summary>
        public static int ElectricChargeID;

        private PartResource _electricCharge;
        
        private const string GroupName = "ODFC";

        /// <summary>The ns</summary>
        public bool ns = true;

        /// <summary>The ODFC configuration</summary>
        public Config ODFC_config;

        /// <summary>The state of the Fuel Cell (nominal, off et al)</summary>
        public states state = states.error;

#endregion Enums Vars
#region KSPFields
        //x added PAW grouping, set to autocollapse - introduced in KSP 1.7.1
        //TODO: would really like the PAW to remember if the group was open
        //TODO: This is to be used to scale ODFC in the part.cfg. also might allow independant scaling of the module from the part as a whole.

        /// <summary>Not Implemented Yet. The internal part.cfg scalingFactor.</summary>
       [UI_Label(scene = UI_Scene.None)]
       [KSPField(  isPersistant = true,   guiActive = false,  guiActiveEditor = false)]
       public float scalingFactor = 0f; // allows for scaling of ODFC elements


        [UI_Label(scene = UI_Scene.Flight)]
        [KSPField(  isPersistant = false, guiActive = false, guiActiveEditor = false,
                    guiName = " ")]
        public string PAWGraph = "";

        [KSPField(  isPersistant = false,  guiActive = true, guiActiveEditor = true,
                    guiName = " ")]
        public string PAWStatus = Localizer.Format("#ODFC-PAW-boot");

        [KSPField(  isPersistant = false,  guiActive = true,  guiActiveEditor = true, groupName = GroupName,
                    groupDisplayName = "On Demand Fuel Cells v" + Version.Text, groupStartCollapsed = true,
                    guiName = ""),
            UI_Label(scene = UI_Scene.Flight)]
        public string status = Localizer.Format("#ODFC-PAW-err");
        
        [KSPField(  isPersistant = true,  guiActive = false, guiActiveEditor = false),
            UI_Label(scene = UI_Scene.None)]
        /// <summary>The fuel mode</summary>
        public int fuelMode = 0;

        /// <summary>The current ElectricCharge %</summary>
        //[UI_Label(scene = UI_Scene.Flight)]
        [KSPField(  isPersistant = false,  guiActive = true, guiActiveEditor = false, groupName = GroupName,
                    guiName = "Vessel EC %", guiFormat = "F2", guiUnits = "%"),
        UI_ProgressBar(minValue = 0f, maxValue = 1f, scene = UI_Scene.Flight)]
        public float CurrentVesselChargeState;

        [KSPField(  isPersistant = false,  guiActive = false, guiActiveEditor = false, groupName = GroupName,
                    guiName = "EC/s (cur/max)")]
        public string ECs_status = Localizer.Format("#ODFC-PAW-err");

        [KSPField(  isPersistant = false, guiActive = false, guiActiveEditor = false, groupName = GroupName,
                    guiName = "Max EC/s")]
        public string maxECs_status = Localizer.Format("#ODFC-PAW-err");

        [KSPField(  isPersistant = false, guiActive = true, guiActiveEditor = true, groupName = GroupName,
                    guiName = "Fuels")]
        public string fuel_consumption = Localizer.Format("#ODFC-PAW-err");

        [KSPField(  isPersistant = false, guiActive = false, guiActiveEditor = false, groupName = GroupName,
                    guiName = "Byproducts")]
        public string byproducts = Localizer.Format("#ODFC-PAW-err");

        [KSPField(  isPersistant = true,  guiActive = true,  guiActiveEditor = true, groupName = GroupName,
                    guiName = "Enabled:"),
            UI_Toggle(  invertButton = true,
                        requireFullControl = false,
                        disabledText = "No",
                        enabledText = "Yes")]
        public bool fuelCellIsEnabled = true;

#endregion KSPFields
#region KSPFields (sliders)

        /*
        TODO: future: convert rateLimit and threshold to use
        KSP 1.7.1 Added a new type for PAW fields, a double slider to set ranges with a min and max values
        UI_MinMaxRange
        */

        /// <summary>The rate limit(max % production)</summary>
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, groupName = GroupName,
                    guiName = "Rate Limit:", guiFormat = "P0"),
            UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
        public float rateLimit = 1f;

        /// <summary>The current threshold (%) which needs to be equal or below before production begins.</summary>
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, groupName = GroupName,
                    guiName = "Threshold:", guiFormat = "P0"),
            UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
        public float threshold = 0.85f; //x thresholdMin;

#endregion KSPFields (sliders)
#region KSPEvents
       
        [KSPEvent(  guiActive = true, guiActiveEditor = true, guiName = "Next Fuel Mode", groupName = GroupName)]
        public void nextFuelMode()
        {
            if (++fuelMode >= ODFC_config.modes.Length) 
                fuelMode = 0;
            updateEditor();
        }

        [KSPEvent(  guiActive = false,  guiActiveEditor = false,  guiName = "Previous Fuel Mode",  groupName = GroupName)]
        public void previousFuelMode()
        {
            if (--fuelMode < 0)
                fuelMode = ODFC_config.modes.Length - 1;
            updateEditor();
        }

#endregion KSPEvents
#region KSPActions

        [KSPAction("Toggle Fuel Cell")]
        public void toggleAction(KSPActionParam kap)
        { fuelCellIsEnabled = !fuelCellIsEnabled; }

        [KSPAction("Enable Fuel Cell")]
        public void enableAction(KSPActionParam kap)
        { fuelCellIsEnabled = true; }

        [KSPAction("Disable Fuel Cell")]
        public void disableAction(KSPActionParam kap)
        { fuelCellIsEnabled = false; }

        [KSPAction("NextFuelMode")]
        public void nextFuelmodeAction(KSPActionParam kap)
        { nextFuelMode(); }

        [KSPAction("PreviousFuelMode")]
        public void previousFuelModeAction(KSPActionParam kap)
        { previousFuelMode(); }

        [KSPAction("Decrease Rate Limit")]
        public void decreaseRateLimitAction(KSPActionParam kap)
        { rateLimit = Math.Max(rateLimit - thresHoldSteps, thresholdMin); }

        [KSPAction("Increase Rate Limit")]
        public void increaseRateLimitAction(KSPActionParam kap)
        { rateLimit = Math.Min(rateLimit + thresHoldSteps, thresHoldMax); }

        [KSPAction("Decrease Threshold")]
        public void decreaseThresholdAction(KSPActionParam kap)
        { threshold = Math.Max(threshold - thresHoldSteps, thresholdMin); }

        [KSPAction("Increase Threshold")]
        public void increaseThresholdAction(KSPActionParam kap)
        { threshold = Math.Min(threshold + thresHoldSteps, thresHoldMax); }
#endregion KSPActions

#region Private Functions

        private void updateFuelString(out string s, Fuel[] fuels, byte Type)
        /// <summary><para> Updates the fuel string.</para></summary>
        /// <param name="s">The s.</param>
        /// <param name="fuels">The fuels.</param>
        {
            s = String.Empty;

            string 
                fuelColorStr = String.Empty,
                fuelRateColorStr = String.Empty,
                endStr = String.Empty;

            if (fuels.Length == 0)
            {
                s = "None\n";
                return;
            }

            //TODO would like to have and if s = fuel_consumption then else byproducts then 
            //? ie different colors for fuel_consumption and byproducts
            if (HighLogic.CurrentGame.Parameters.CustomParams<Options>().coloredPAW)
            {
                switch (Type)
                {
                    case 0: // fuels
                        {
                            fuelColorStr = "<#FFFF00>";
                            break;
                        }
                    case 1: // byproducts
                        {
                            fuelColorStr = "<#ADFF2F>";
                            break;
                        }
                    default:
                        {
                            fuelColorStr = "";
                        }
                        break;
                }
                fuelRateColorStr = "</color>";
            }

            foreach (Fuel fuel in fuels)
            {
               // ResourceLabel abr = lastResource.Find(x => x.resourceID == fuel.resourceID);
                //? THIS IS WHERE have to add code to include consumption/production #
                s += String.Format("\n{0}{1,-16}:{2,-14}{3}{4}", fuelColorStr, PartResourceLibrary.Instance.GetDefinition(fuel.resourceID).name, fuelRateColorStr, RateString(fuel.rate), endStr);
                // add code to verify found exists to prevent nullref 
            }
        }

        private static string RateString(double Rate)
        {
            string sfx = "/s ";

            if (Rate <= 0.004444444f)
            {
                Rate *= 3600;
                sfx = "/h ";
            }
            else if (Rate < 0.2666667f)
            {
                Rate *= 60;
                sfx = "/m ";
            }
            // limit decimal places to 10 and add sfx
            //return String.Format(FuelRateFormat, Rate, sfx);
            return Rate.ToString("0.##########") + sfx;
        }

        private void updateFuelTexts()
        /// <summary>Updates the fuel texts.</summary>
            {
                updateFuelString(out fuel_consumption, ODFC_config.modes[fuelMode].fuels, 0);
                updateFuelString(out byproducts, ODFC_config.modes[fuelMode].byproducts, 1);
            }

        private void UpdateState(states newstate, double gen, double max)
        {
               // Log.Info(String.Format("UpdateState {0} gen/max {1}:{2}", newstate.ToString(), gen, max));
            if (gen != lastGen || max != lastMax)
            {
                lastGen = gen;
                lastMax = max;

                ECs_status = gen.ToString(FuelTransferFormat) + " / " + max.ToString(FuelTransferFormat);
            }

            if (state != newstate)
            {
                state = newstate;
                status = STATES_STR[(int)state];
            }

            double tf = gen / max * rateLimit;
            if (tf != lastTF || fuelMode != lastFuelMode)
            {
                lastTF = tf;
                lastFuelMode = fuelMode;
            }
            updatePAWLabel();
        }

        private void kommit(Fuel[] fuels, double adjr)
        {
            foreach (Fuel fuel in fuels)
                part.RequestResource(fuel.resourceID, fuel.rate * adjr);
        }

        private void updateConfig(StartState state)
        {
            ODFC_config = new Config(ConfigNode.Parse(ConfigNodeString).GetNode("MODULE"), part);
            switch (ODFC_config.modes.Length)
            {
                case 0:
                    {
                            Log.Error("No ODFC_config.modes");
                        break;
                    }
                case 1:
                    {
                            Log.Info("updateConfig switch 1");
                        Events["nextFuelMode"].guiActive = false;
                        Events["nextFuelMode"].guiActiveEditor = false;
                        Events["previousFuelMode"].guiActive = false;
                        Events["previousFuelMode"].guiActiveEditor = false;

                        Actions["nextFuelmodeAction"].active = false;
                        Actions["previousFuelModeAction"].active = false;
                        break;
                    }
                case 2:
                    {
                            Log.Info("updateConfig switch 2");
                        Events["previousFuelMode"].guiActive = false;
                        Events["previousFuelMode"].guiActiveEditor = false;
                        Actions["PreviousFuelMode"].active = false;
                        break;
                    }
                default:
                    {
                            Log.Info("updateConfig switch default");
                        Events["nextFuelMode"].guiActive = true;
                        Events["nextFuelMode"].guiActiveEditor = true;
                        Events["previousFuelMode"].guiActive = true;
                        Events["previousFuelMode"].guiActiveEditor = true;

                        Actions["nextFuelmodeAction"].active = true;
                        Actions["previousFuelModeAction"].active = true;
                        break;
                    }
            }
            foreach (mode m in ODFC_config.modes)
            {   
                if (m.byproducts.Length > 0) // Show byproducts tweakable if at least one mode has at least one byproduct
                {
                    Fields["byproducts"].guiActive = true;
                    Fields["byproducts"].guiActiveEditor = true;
                    break;
                }
            }

            if (state != StartState.Editor)
                part.force_activate();

            updateEditor();
        }

#endregion Private Functions
#region internal Public updateFunctions

        internal void updateEditor() // private
        /// <summary><para>Updates the PAW with scaleFactor and advises KSP that the ship has changed</para></summary>
        {
            updateFuelTexts();

            //? following needed to advise KSP that the ship has been modified and it needs to update itself. (Lisias)
            if (HighLogic.LoadedSceneIsEditor) GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
        }

        internal void updatePAWLabel() // private
        /// <summary><para></para>Updates the PAW label.</para></summary>
        {
            //string colorStr = "<#ADFF2F>";
            string colorStr = String.Empty;
            string begStr = String.Empty;
            string endStr = String.Empty;

            if (HighLogic.CurrentGame.Parameters.CustomParams<Options>().coloredPAW)
            {
                begStr = "<size=+1><b>";
                colorStr = STATES_COLOUR[(int)state];
                endStr = "</color></b></size>";
            }

            switch (HighLogic.LoadedScene)
            {
                case GameScenes.FLIGHT:
                    {
                        PAWStatus = begStr + colorStr + "Fuel Cell: " + status + " - " + ECs_status + " EC/s" + endStr;
                        break;
                    }
                case GameScenes.EDITOR:
                    {
                        PAWStatus = begStr + colorStr + "Fuel Cell: " + maxECs_status + "/" + ODFC_config.modes[fuelMode].maxEC + " EC/s:" + endStr;
                    //PAWStatus = begStr + colorStr + "Fuel Cell: " + _fuelModeMaxECRateLimit + "/" + maxECs_status + " EC/s:" + endStr;
                        break;
                    }
/*                case GameScenes.LOADING:
                    break;
                case GameScenes.LOADINGBUFFER:
                    break;
                case GameScenes.MAINMENU:
                    break;
                case GameScenes.SETTINGS:
                    break;
                case GameScenes.CREDITS:
                    break;
                case GameScenes.SPACECENTER:
                    break;
                case GameScenes.TRACKSTATION:
                    break;
                case GameScenes.PSYSTEM:
                    break;
                case GameScenes.MISSIONBUILDER:
                    break;
                default:
                    break;*/
            }
        }
#endregion Public Functions
#region OnReScale

        /// <summary>Updates the PAW with scaleFactor and advises KSP that the ship has changed</summary>

        /// <summary> <para> Called when [rescale].</para> </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        internal void OnRescale(TweakScale.ScalingFactor.FactorSet scaleFactor)
        {
           // Log.Info(String.Format("scaleFactor: {0}:{1}", scaleFactor.ToString(), scaleFactor.quadratic));

            /// <summary>this scales any resources on the part with ODFC:  </summary>
            foreach (PartResource resource in this.part.Resources)
            {
             //       Log.Info(String.Format("unscaled resource: {0}: {1} / {2}", resource.resourceName, resource.amount, resource.maxAmount));
                resource.maxAmount *= scaleFactor.quadratic; // .cubic;
                resource.amount *= scaleFactor.quadratic; // cubic;
               //     Log.Info(String.Format("scaled resource: {0}: {1} / {2}", resource.resourceName, resource.amount, resource.maxAmount));
            }

            /// <summary><para> this scales the actual fuel cell, fuels, byproducts, and maxEC
            /// shouldn't scale rateLimit and threshold because are percentages </para></summary>
            for (int m = 0; m <= ODFC_config.modes.Length - 1; m++)
            {
                 //   Log.Info(String.Format("mode/modes: {0} / {1}", (m + 1), ODFC_config.modes.Length));
                 //   Log.Info(String.Format("unscaled maxEC: {0}", ODFC_config.modes[m].maxEC));
                //? scale MaxEC
                ODFC_config.modes[m].maxEC *= scaleFactor.quadratic;
                 //   Log.Info(String.Format("scaled maxEC: {0}", ODFC_config.modes[m].maxEC));

                  //  Log.Info(String.Format("Fuels in mode: {0} / {1}", (m + 1), ODFC_config.modes[m].fuels.Length + 1));
                //? scale fuels in ODFC_config.modes
                for (int n = 0; n <= ODFC_config.modes[m].fuels.Length - 1; n++)
                    ODFC_config.modes[m].fuels[n].rate *= scaleFactor.quadratic;

                   // Log.Info(String.Format("Byproducts in mode: {0} / {1}", (m + 1), ODFC_config.modes[m].byproducts.Length + 1));
                //? scale byproducts in ODFC_config.modes
                for (int n = 0; n <= ODFC_config.modes[m].byproducts.Length - 1; n++)
                    ODFC_config.modes[m].byproducts[n].rate *= scaleFactor.quadratic;
            }
            updateEditor(); // updateFuelTexts(); // removed.this
        }

#endregion OnRescale
#region on events
/*        public override void OnAwake()
        /// <summary>Called when part is added to the craft.</summary>
        {
                Log.Info(String.Format("OnAwake for {0}", this.name));
            base.OnAwake();
        }*/

        public override void OnLoad(ConfigNode configNode)
        /// <summary>Called when [load].</summary>
        /// <param name="configNode">The configuration node.</param>
        {
            if (string.IsNullOrEmpty(ConfigNodeString))
            {
                this.configNode = configNode;                        // Needed for GetInfo()
                ConfigNodeString = configNode.ToString();            // Needed for marshalling
                //Log.Info("ConfigNodeString:\n " + ConfigNodeString);
            }
        }

        public override void OnStart(StartState state)
        /// <summary>Called when [start].</summary>
        /// <param name="state">The state.</param>
        {
            Log.Info(String.Format("OnStart {0}", state));

            if (ElectricChargeID == default(int)) ElectricChargeID = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;
                Log.Info("ElectricChargeID: " + ElectricChargeID.ToString());
            if (ElectricChargeID == default(int)) Log.Error("OnStart Error: failed to obtain ElectricChargeID"); // warn if not found, this module cannot function without.

            _electricCharge = part.Resources?.Get(id: ElectricChargeID); // obtain the EC resource
                Log.Info("_electricCharge: " + _electricCharge.resourceName);
            if (_electricCharge == null) Log.Error("OnStart Error: failed to obtain _electricCharge resource"); // warn if not found, this module cannot function without.

            Log.Info("OnStart: updateConfig");
                updateConfig(state);

            Log.Info("OnStart: force_activate");
            if (state != StartState.Editor)
                part.force_activate();
        }

        public override void OnFixedUpdate()
        /// <summary>Called when [fixed update].</summary>
        {
            states ns = fuelCellIsEnabled ? states.nominal : states.off;

            if (ns != states.nominal)
            {
                UpdateState(ns, 0, ODFC_config.modes[fuelMode].maxEC);
                return;
            }

            if (_electricCharge == null)
            {
                Log.Error("OnFixedUpdate: _electricCharge is null");
                return; // already checked for this in OnStart()
            }
            CurrentVesselChargeState = (float)(_electricCharge.amount / _electricCharge.maxAmount * 100); // compute current EC, in percent
            //TODO: update KSP_ProgressBar
            // ProgressBar["CurrentVesselChargeState"].
            ASCIIgraph();

            double amount = 0, maxAmount = 0;
            List<PartResource> resources = new List<PartResource>();
            part.GetConnectedResourceTotals(ElectricChargeID, ResourceFlowMode.ALL_VESSEL, out amount, out maxAmount);
                //Log.Info(String.Format("{0} Amount {1} maxAmount {2}", "Electric Charge", amount, maxAmount));

           // foreach (PartResource resource in this.part.Resources)
            foreach (PartResource resource in resources)
            {
                maxAmount += resource.maxAmount;
                amount += resource.amount;
            }
                //Log.Info(String.Format("{0} Amount {1} maxAmount {2}", "Electric Charge", amount, maxAmount));

            double cfTime = TimeWarp.fixedDeltaTime,
                    ECNeed = (double)(maxAmount * threshold - amount);
                    
            _fuelModeMaxECRateLimit = ODFC_config.modes[fuelMode].maxEC * rateLimit;
            Log.Info(OnDemandFuelCellsEC.ToString());

 //? add stall code
            if (HighLogic.CurrentGame.Parameters.CustomParams<Options>().needsECtoStart && amount == 0)
            {
                UpdateState(states.stalled, 0, ODFC_config.modes[fuelMode].maxEC);
                return;
            }

// Determine activity based on supply/demand
            cfTime = Math.Min(cfTime, ECNeed / _fuelModeMaxECRateLimit);
            if (cfTime <= 0)
            {
                UpdateState(states.noDemand, 0, ODFC_config.modes[fuelMode].maxEC);
               // UpdateState(states.noDemand, 0, _fuelModeMaxECRateLimit);
                return;
            }

 // Determine activity based on available fuel
            foreach (Fuel fuel in ODFC_config.modes[fuelMode].fuels)
            {
                amount = 0;
                part.GetConnectedResourceTotals(fuel.resourceID, out amount, out maxAmount);

               // foreach (PartResource r in this.part.Resources)
                foreach (PartResource r in resources)
                    amount += r.amount;

                cfTime = Math.Min(cfTime, amount / (fuel.rate * rateLimit));
            }

            if (cfTime == 0)
            {
                UpdateState(states.fuelDeprived, 0, ODFC_config.modes[fuelMode].maxEC);

//? this looks for another fuel mode that isn't deprived if autoSwitch == true
                if (HighLogic.CurrentGame.Parameters.CustomParams<Options>().autoSwitch)
                {
            // TODO: responseTime
                    // this slows down the autoSwitch so it doesn't break the PAW
                    if ((HighLogic.CurrentGame.Parameters.CustomParams<Options>().responseTime / 10) <= timeOut++)
                    {
                        return;
                    }    
                    else
                    {
                        nextFuelMode();
                        timeOut = 0;
                    }                        
                }
                return;
            }


//?s Calculate usage based on rate limiting and duty cycle
            double adjr = rateLimit * cfTime;
            double ECAmount = _fuelModeMaxECRateLimit * cfTime;

            part.RequestResource(ElectricChargeID, -ECAmount); // Don't forget the most important part (add ElectricCharge (EC))
            
            kommit(ODFC_config.modes[fuelMode].fuels, adjr); // Commit changes to fuel used
            kommit(ODFC_config.modes[fuelMode].byproducts, adjr); // Handle byproducts

            UpdateState(states.nominal, ECAmount / TimeWarp.fixedDeltaTime, ODFC_config.modes[fuelMode].maxEC);
            
            base.OnFixedUpdate();
        }

        public override void OnUpdate()
        /// <summary>Updates this instance.</summary>
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                double newMax = ODFC_config.modes[fuelMode].maxEC * rateLimit;
                if (lastMax != newMax)
                {
                    lastMax = newMax;
                    maxECs_status = lastMax.ToString(FuelTransferFormat);
                }

                states newState = fuelCellIsEnabled ? states.nominal : states.off;
                UpdateState(newState, newState == states.nominal ? 1 : 0, 1);
            }
            base.OnUpdate();
        }
#endregion on Events
#region GetInfo

        public override string GetInfo()
        /// <summary>Formats the information for the part information in the editors.</summary>
        /// <returns>Editor Part Detail Information</returns>
        {
            //? this is what is show in the editor
            //? As annoying as it is, pre-parsing the config MUST be done here, because this is called during part loading.
            //? The config is only fully parsed after everything is fully loaded (which is why it's in OnStart())
            if (info == string.Empty)
            {
                info += Localizer.Format("#ODFC-manu-titl"); // #ODFC-manu-titl = Okram Industries
                info += "\n v" + Version.Text; // ODFC Version Number text
                info += "\n<color=#b4d455FF>" + Localizer.Format("#ODFC-desc"); // #ODFC-desc = Automated fuel cell controller which only generates electricity when really needed
                info += "</color>\n\n";

                ConfigNode[] mds = configNode.GetNodes("MODE");
                info += "Modes: " + mds.Length.ToString();

                for (int n = 0; n < mds.Length; n++)
                    info += "\n\n<color=#99FF00FF>Mode: " + n.ToString() + "</color> - Max EC: " + mds[n].GetValue("MaxEC") +
                        "/s\n<color=#FFFF00FF>Fuels:</color>" + GetResourceRates(mds[n].GetNode("FUELS")) +
                        "\n<color=#FFFF00FF>Byproducts:</color>" + GetResourceRates(mds[n].GetNode("BYPRODUCTS"));
            }

            //info += "\n\n<color=orange>Requires:</color> \n - <color=white><b>" + Localizer.Format("#autoLOC_252004"); // #autoLOC_252004 = ElectricCharge
            //info += "</b>: \n <color=#99FF00FF>  - Per Crew: </b></color><color=white>" + RateString(ECFactor) + " </color>";
            //info += "</b>: \n <color=#99FF00FF>  - Max Crew: </b></color><color=white>" + RateString(maxCrew * ECFactor) + "</color>";
            Log.Info(info);
            return info;
        }

        private string GetResourceRates(ConfigNode node)
        {
            if (node == null || node.values.Count < 1)
                return "\n - None";

            string resourceRates = String.Empty;

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

                resourceRates += "\n - " + value.name + ": " + rate.ToString("0.##########") + sfx;
            }
               // Log.Info(resourceRates);
            return resourceRates;
        }
#endregion GetInfo
#region new zed'K code (future)
        
        private static readonly string  aStr = char.ConvertFromUtf32(0x2593),
                                        bStr = char.ConvertFromUtf32(0x2593),
                                        cStr = char.ConvertFromUtf32(0x2592),
                                        dStr = char.ConvertFromUtf32(0x2591);

        private void ASCIIgraph()
        {

           if (HighLogic.CurrentGame.Parameters.CustomParams<Options>().powerGraph)
           {

                string tmpStr = "";
                Fields["PAWGraph"].guiActive = true;

                   // Log.Info(String.Format("ASCIIgraph: {0}", CurrentVesselChargeState / 3));

                for (byte ticks = 0; ticks < (CurrentVesselChargeState / 3); ticks++)
                {
                    /*
                    TODO:
                        * add color (honor PAWColor)
                            HighLogic.CurrentGame.Parameters.CustomParams<Options>().coloredPAW
                    */

                    if (ticks <= 5) tmpStr += "<color=red>" + aStr + "</color>";
                    if (ticks > 5 && ticks < 10) tmpStr += "<color=yellow>" + cStr + "</color>";
                    if (ticks > 10 && ticks < 30) tmpStr += "<color=green>" + cStr + "</color>";
                    if (ticks >= 30) tmpStr += "<color=blue>" + dStr + "</color>";
                }
                PAWGraph = tmpStr + "</color>";
           }
           else
              Fields["PAWGraph"].guiActive = false;
        }
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

#endregion notes 