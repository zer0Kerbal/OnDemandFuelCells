#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODFC {
	public class ODFC : PartModule {
		#region Enums Vars
		public enum states : byte { error, off, nominal, fuelDeprived, noDemand }; // deploy, retract, 

        private const string FuelTransferFormat = "0.##"; //FuelTransferFormat?
		private const float
			thresHoldSteps = 0.05f,
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

		#region Fields Events Actions
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public int fuelMode = 0;

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Status")]
		public string status = "ERROR!";

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "EC/s (cur/max)")]
		public string ECs_status = "ERROR!";

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Max EC/s")]
		public string maxECs_status = "ERROR!";

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Fuel Used")]
		public string fuel_consumption = "ERROR!";

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = false, guiName = "Byproducts")]
		public string byproducts = "ERROR!";

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Enabled:"), UI_Toggle(disabledText = "No", enabledText = "Yes")]
		public bool fuelCellIsEnabled = true;

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Previous Fuel Mode")]
		public void previousFuelMode() {
			if(--fuelMode < 0)
				fuelMode = ODFC_config.modes.Length - 1;

			udft();
		}

		[KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Next Fuel Mode")]
		public void nextFuelMode() {
			if(++fuelMode >= ODFC_config.modes.Length)
				fuelMode = 0;

			udft();
		}

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rate Limit:", guiFormat = "P0"), UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
		public float rateLimit = 1f;

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Threshold:", guiFormat = "P0"), UI_FloatRange(minValue = thresholdMin, maxValue = thresHoldMax, stepIncrement = thresHoldSteps)]
		public float threshold = thresholdMin;

		[KSPAction("Toggle")]
		public void toggleAction(KSPActionParam kap) {
			fuelCellIsEnabled = !fuelCellIsEnabled;
		}

		[KSPAction("Enable")]
		public void enableAction(KSPActionParam kap) {
			fuelCellIsEnabled = true;
		}

		[KSPAction("Disable")]
		public void disableAction(KSPActionParam kap) {
			fuelCellIsEnabled = false;
		}

		[KSPAction("Previous Fuel Mode")]
		public void previousFuelModeAction(KSPActionParam kap) {
			previousFuelMode();
		}

		[KSPAction("Next Fuel Mode")]
		public void nextFuelmodeAction(KSPActionParam kap) {
			nextFuelMode();
		}

		[KSPAction("Decrease Rate Limit")]
		public void decreaseRateLimitAction(KSPActionParam kap) {
			rateLimit = Math.Max(rateLimit - thresHoldSteps, thresholdMin);
		}

		[KSPAction("Increase Rate Limit")]
		public void increaseRateLimitAction(KSPActionParam kap) {
			rateLimit = Math.Min(rateLimit + thresHoldSteps, thresHoldMax);
		}

		[KSPAction("Decrease Threshold")]
		public void decreaseThresholdAction(KSPActionParam kap) {
			threshold = Math.Max(threshold - thresHoldSteps, thresholdMin);
		}

		[KSPAction("Increase Threshold")]
		public void increaseThresholdAction(KSPActionParam kap) {
			threshold = Math.Min(threshold + thresHoldSteps, thresHoldMax);
		}
		#endregion

		#region Private Functions
		private void udfs(out string s, Fuel[] fuels) {
			if(fuels.Length == 0) {
				s = "None";
				return;
			}

			s = "";
			bool plus = false;

			foreach(Fuel fuel in fuels) {
				if(plus)
					s += " + ";
	// add code to verify found exists to prevent nullref			
				plus = true;
				resourceLa abr = lastResource.Find(x => x.resourceID == fuel.resourceID);

                s += PartResourceLibrary.Instance.GetDefinition(fuel.resourceID).name;
            }
        }

		private void udft() { //udft?
			udfs(out fuel_consumption, ODFC_config.modes[fuelMode].fuels);
			udfs(out byproducts, ODFC_config.modes[fuelMode].byproducts);
		}

		private void UpdateState(states newstate, Double gen, Double max) {
			if(gen != lastGen || max != lastMax) {
				lastGen = gen;
				lastMax = max;

				ECs_status = gen.ToString(FuelTransferFormat) + " / " + max.ToString(FuelTransferFormat);
			}

			if(state != newstate) {
				state = newstate;
			
				switch(state) {
					case states.fuelDeprived: {
						status = "Fuel Deprived";
						break;
					}
					case states.noDemand: {
						status = "No Demand";
						break;
					}
					case states.nominal: {
						status = "Nominal";
						break;
					}
					case states.off: {
						status = "Off";
						break;
					}
#if DEBUG
					default: {
						status = "ERROR!";
						break;
					}
#endif
				}
			}
			
			Double tf = gen / max * rateLimit;
			if(tf != lastTF || fuelMode != lastFuelMode) {
				lastTF = tf;
				lastFuelMode = fuelMode;
			}
		}

		private string GetResourceRates(ConfigNode node) {
			if(node == null || node.values.Count < 1)
				return "\n - None";

			string resourceRates = "";

			foreach(ConfigNode.Value value in node.values) {
				float rate = float.Parse(value.value);
				string sfx = "/s";

				if(rate <= 0.004444444f) {
					rate *= 3600;
					sfx = "/h";
				} else if(rate < 0.2666667f) {
					rate *= 60;
					sfx = "/m";
				}

				resourceRates += "\n - " + value.name + ": " + rate.ToString() + sfx;
			}

			return resourceRates;
		}

		private void kommit(Fuel[] fuels, Double adjr) {
			foreach(Fuel fuel in fuels)
                part.RequestResource(fuel.resourceID, fuel.rate * adjr);
		}
		#endregion

		#region Public Functions
		public override void OnLoad(ConfigNode configNode) {
			if(string.IsNullOrEmpty(scn)) {
				this.configNode = configNode;			// Needed for GetInfo()
				scn = configNode.ToString();	// Needed for marshalling
			}
		}

		public override void OnStart(StartState state) {

			if(ElectricChargeID == default(int))
				ElectricChargeID = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;

			configNode = ConfigNode.Parse(scn).GetNode("MODULE");
			ODFC_config = new cfg(configNode, part);

			// One puppy will explode for every question you ask about this code.  Please, think of the puppies.

			udft();

			if(ODFC_config.modes.Length < 2) {	// Disable unneccessary UI elements if we only have a single mode
				Events["nextFuel"].guiActive			= false;
				Events["nextFuel"].guiActiveEditor	    = false;
				Fields["fuel_consumption"].guiActive				= false;
				Fields["fuel_consumption"].guiActiveEditor		= false;
				Actions["previousFuelModeAction"].active					= false;
				Actions["nextFuelModeAction"].active					= false;
			} else {						// If we have at least 2 modes
				if(ODFC_config.modes.Length > 2) {		// If we have at least 3 modes
					Events["previousFuelMode"].guiActive			= true;
					Events["previousFuelMode"].guiActiveEditor	= true;
				} else {							// If we have exactly 2 modes
					Actions["previousFuelModeAction"].active					= false;
				}

				foreach(mode m in ODFC_config.modes) {	// Show byproducts tweakable if at least one mode has at least one byproduct
					if(m.byproducts.Length > 0) {
						Fields["byproducts"].guiActive			= true;
						Fields["byproducts"].guiActiveEditor	= true;
						break;
					}
				}
			}

			if(state != StartState.Editor)
				part.force_activate();
		}

		public override string GetInfo() {
			// As annoying as it is, pre-parsing the config MUST be done here, because this is called during part loading.
			// The config is only fully parsed after everything is fully loaded (which is why it's in OnStart())
			if(info == string.Empty) {
				ConfigNode[] mds = configNode.GetNodes("MODE");
				info += "Modes: " + mds.Length.ToString();

				for(byte n = 0; n < mds.Length; n++)
					info += "\n\n<color=#99FF00FF>Mode: " + n.ToString() + "</color> - Max EC: " + mds[n].GetValue("MaxEC") +
						"/s\n<color=#FFFF00FF>Fuels:</color>" + GetResourceRates(mds[n].GetNode("FUELS")) +
						"\n<color=#FFFF00FF>Byproducts:</color>" + GetResourceRates(mds[n].GetNode("BYPRODUCTS"));
			}

			return info;
		}

		public override void OnFixedUpdate() {
            states ns = fuelCellIsEnabled ? states.nominal : states.off;

            if (ns != states.nominal) {
				UpdateState(ns, 0, 0);
				return;
			}

			Double amount = 0, maxAmount = 0;
			List<PartResource> resources = new List<PartResource>();
            part.GetConnectedResourceTotals(ElectricChargeID, out amount, out maxAmount);

			foreach(PartResource resource in resources) { 
				maxAmount += resource.maxAmount;
				amount += resource.amount;
			}

			Double
				cfTime = TimeWarp.fixedDeltaTime,
				ECNeed = (Double)(maxAmount * threshold - amount),
				fuelModeMaxECRateLimit	= ODFC_config.modes[fuelMode].maxEC * rateLimit;

			cfTime = Math.Min(cfTime, ECNeed / fuelModeMaxECRateLimit);	// Determine activity based on supply/demand

			if(cfTime <= 0) {
				UpdateState(states.noDemand, 0, fuelModeMaxECRateLimit);
				return;
			}                      

            foreach (Fuel fuel in ODFC_config.modes[fuelMode].fuels) {	// Determine activity based on available fuel
				amount = 0;
                part.GetConnectedResourceTotals(fuel.resourceID , out amount, out maxAmount);

                foreach (PartResource r in resources)
					amount += r.amount;

				cfTime = Math.Min(cfTime, amount / (fuel.rate * rateLimit)); // (Double)amount)
            }

            if (cfTime == 0)  // (cfTime == 0) (Math.Round(cfTime, MidpointRounding.ToEven) == 0)
            {
                UpdateState(states.fuelDeprived, 0, fuelModeMaxECRateLimit);
                return;
            }

            Double adjr = rateLimit * cfTime;           // Calculate usage based on rate limiting and duty cycle
            Double ECAmount = fuelModeMaxECRateLimit * cfTime;
            part.RequestResource(ElectricChargeID, -ECAmount);   // Don't forget the most important part

            kommit(ODFC_config.modes[fuelMode].fuels, adjr);            // Commit changes to fuel used
            kommit(ODFC_config.modes[fuelMode].byproducts, adjr);           // Handle byproducts

            UpdateState(states.nominal, ECAmount / TimeWarp.fixedDeltaTime, fuelModeMaxECRateLimit);
		}

        public void Update() {
			if(HighLogic.LoadedSceneIsEditor) {
				Double newMax = ODFC_config.modes[fuelMode].maxEC * rateLimit;
				
				if(lastMax != newMax) {
					lastMax = newMax;
					maxECs_status = lastMax.ToString(FuelTransferFormat);
				}

                states newState = fuelCellIsEnabled ? states.nominal : states.off;
                UpdateState(newState, newState == states.nominal ? 1 : 0, 1);
			}
		}
		#endregion
	}
}
