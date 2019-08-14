#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODFC {
	public class ODFC : PartModule {
		#region Enums Vars
		public enum states : byte { error, off, nominal, deploy, retract, fuelDeprived, noDemand };

		private const string FuelTransferFormat = "0.##"; //FuelTransferFormat?
		private const float
			thresHoldSteps = 0.05f,
			thresholdMin = thresHoldSteps,
			thresHoldMax = 1;

		//private Animation animation;
		//private AnimationState animationState;
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
				
				plus = true;
				resourceLa abr = lastResource.Find(x => x.resourceID == fuel.resourceID);

				if(abr == default(resourceLa))	// If we're missing a resource abbreviation (bad!)
					s += PartResourceLibrary.Instance.GetDefinition(fuel.resourceID).name;
				else	// Found one (good!)
					s += abr.resourceAbbreviation;
			}
		}

		private void udft() { //udft?
			udfs(out fuel_consumption, ODFC_config.modes[fuelMode].fuels);
			udfs(out byproducts, ODFC_config.modes[fuelMode].byproducts);

	/*		foreach(ConfigNode.Value cnv in ODFC_config.modes[fuelMode].tanks) {
				foreach(MeshRenderer mr in part.FindModelComponents<MeshRenderer>(cnv.name))
					mr.material.mainTexture = GameDatabase.Instance.GetTexture(cnv.value, false);
			}
 
            foreach(emmitter e in ODFC_config.modes[fuelMode].emttrs) {

                foreach (KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>(e.name) ?? new List<KSPParticleEmitter>())
					kpe.colorAnimation = e.colors;
			}

            foreach(light l in ODFC_config.modes[fuelMode].lights) {
				foreach(Light lim in part.FindModelComponents<Light>(l.name) ?? new List<Light>())
					lim.color = l.color;
			}*/
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
					//case states.deploy: {
					//	status = "Deploying";
					//	amfd(-1);
					//	break;
					//}
					//case states.retract: {
					//	status = "Retracting";
					//	amfd(1);
					//	break;
					//}
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

 /*                foreach(light l in ODFC_config.modes[fuelMode].lights) {
					foreach(Light lim in part.FindModelComponents<Light>(l.name) ?? new List<Light>())
						lim.intensity = Convert.ToSingle(l.mmag * tf);
				}
				tf *= ODFC_config.emissionCoefficient;
				int min = (state == states.nominal ? 1 : 0);
                
				foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>()) {
					emmitter e = Array.Find(ODFC_config.modes[fuelMode].emttrs, x => x.name == kpe.name);
					kpe.minEmission = kpe.maxEmission = Math.Max((int)(tf * (e == default(emmitter) ? 1 : e.scale)), min);
				}*/
			}
		}

		//private void amfd(float speed) {
		//	if(animation == null)
		//		return;

		//	animationState.speed = speed;

		//	if(!animation.isPlaying)
		//		animation.Play();
		//}

		//private states stchk() {
		//	if(animation == null)
		//		return fuelCellIsEnabled ? states.nominal : states.off;

		//	// Unity is stupid and doesn't have a WrapMode to stop when AnimationState.normalizedTime == 1 without resetting it to 0.  WTF?!
		//	if(animationState.normalizedTime < 0 || animationState.normalizedTime > 1) {
		//		float nnt = Mathf.Clamp(animationState.normalizedTime, 0, 1);
		//		animation.Stop();	// This screws with time/normalizedTime, so we have to set it back to what it should be below
		//		animationState.normalizedTime = nnt;
		//	}

		//	return fuelCellIsEnabled ? (animationState.normalizedTime == 0 ? states.nominal : states.deploy) : (animationState.normalizedTime == 1 ? states.off : states.retract);
		//}

		private string GetresourceRates(ConfigNode node) {
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
			//animation = part.FindModelComponent<Animation>();
			//animationState = (animation == null) ? null : animation[animation.clip.name];

			if(ElectricChargeID == default(int))
				ElectricChargeID = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;

			configNode = ConfigNode.Parse(scn).GetNode("MODULE");
			ODFC_config = new cfg(configNode, part);

			foreach(ConfigNode.Value cnv in configNode.GetNode("FSHORT").values) {
				int rid = PartResourceLibrary.Instance.GetDefinition(cnv.name).id;

				if(!lastResource.Exists(x => x.resourceID == rid))
					lastResource.Add(new resourceLa(rid, cnv.value));
			}

			// One puppy will explode for every question you ask about this code.  Please, think of the puppies. {
			//if(ns) {
			//	ns = false;

			//	foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>() ?? new List<KSPParticleEmitter>()) {
			//		// Why isn't there just one variable (Vector3 for shape3D) and then just use only the floats you need in that?  Oh, that would make too much sense.
			//		kpe.shape3D			*= ODFC_config.scaleHack;
			//		kpe.shape2D			*= ODFC_config.scaleHack;
			//		kpe.shape1D			*= ODFC_config.scaleHack;
			//		kpe.minSize			*= ODFC_config.scaleHack;
			//		kpe.maxSize			*= ODFC_config.scaleHack;
			//		kpe.rndVelocity	*= ODFC_config.scaleHack;
			//		kpe.localVelocity	*= ODFC_config.scaleHack;
			//		kpe.force			*= ODFC_config.scaleHack;
			//		kpe.rndForce		*= ODFC_config.scaleHack;
			//	}

			//	foreach(Light l in part.FindModelComponents<Light>() ?? new List<Light>())
			//		l.range *= ODFC_config.scaleHack;
			//}
			// }

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
						"/s\n<color=#FFFF00FF>Fuels:</color>" + GetresourceRates(mds[n].GetNode("FUELS")) +
						"\n<color=#FFFF00FF>Byproducts:</color>" + GetresourceRates(mds[n].GetNode("BYPRODUCTS"));
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
            /* pr.part.partInfo.title */
            //part.GetConnectedResource(ecid, ResourceFlowMode.ALL_VESSEL, pr);
            part.GetConnectedResourceTotals(ElectricChargeID, out amount, out maxAmount);

			foreach(PartResource resource in resources) { 
				maxAmount += resource.maxAmount;
				amount += resource.amount;
			}

			Double
				cft = TimeWarp.fixedDeltaTime,
				ecn = (Double)(maxAmount * threshold - amount),
				mecrl	= ODFC_config.modes[fuelMode].maxEC * rateLimit;

			cft = Math.Min(cft, ecn / mecrl);	// Determine activity based on supply/demand

			if(cft <= 0) {
				UpdateState(states.noDemand, 0, mecrl);
				return;
			}                      

            foreach (Fuel fuel in ODFC_config.modes[fuelMode].fuels) {	// Determine activity based on available fuel
				amount = 0;
				resources.Clear(); // Might not be necessary, but safer
                //part.GetConnectedResources(f.rid, f.rfm, pr);
                part.GetConnectedResourceTotals(ElectricChargeID, out amount, out maxAmount);

                foreach (PartResource r in resources)
					amount += r.amount;

				cft = Math.Min(cft, ((Double)amount) / (fuel.rate * rateLimit));
			}

            if (cft == 0)
            {
                UpdateState(states.fuelDeprived, 0, mecrl);
                return;
            }

            Double adjr = rateLimit * cft;			// Calculate usage based on rate limiting and duty cycle
			kommit(ODFC_config.modes[fuelMode].fuels, adjr);			// Commit changes to fuel used
			kommit(ODFC_config.modes[fuelMode].byproducts, adjr);			// Handle byproducts

			Double eca = mecrl * cft;
            part.RequestResource(ElectricChargeID, -eca);   // Don't forget the most important part
            UpdateState(states.nominal, eca / TimeWarp.fixedDeltaTime, mecrl);
		}

        //private void uds(states nominal, Double v, Double mecrl)
        //{
        //    throw new NotImplementedException();
        //}

        //private void uds(states fuelDeprived, int v, Double mecrl)
        //{
        //    throw new NotImplementedException();
        //}

        public void Update() {
			if(HighLogic.LoadedSceneIsEditor) {
				Double nmax = ODFC_config.modes[fuelMode].maxEC * rateLimit;
				
				if(lastMax != nmax) {
					lastMax = nmax;
					maxECs_status = lastMax.ToString(FuelTransferFormat);
				}

                states ns = fuelCellIsEnabled ? states.nominal : states.off;
                UpdateState(ns, ns == states.nominal ? 1 : 0, 1);
			}
		}
		#endregion
	}
}
