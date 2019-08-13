#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODFC {
	public class ODFC : PartModule {
		#region Enums Vars
		public enum states : byte { error, off, nominal, deploy, retract, fuelDepr, noDemand };

		private const string FTFMT = "0.##";
		private const float
			percentStop = 0.05f,
			percentMin = percentStop,
			percentMax = 1;

		private Animation a;
		private AnimationState ast;
		private Double
			lgen = -1,
			lmax = -1,
			ltf = -1;
		private int lfm = -1;
		private string info = string.Empty;

		public ConfigNode cn;
		public static List<rla> rlas = new List<rla>();
		public static int electricChargeID;
		public string scn;
		public bool ns = true;
		public cfg c;
		public states state = states.error;
		#endregion

		#region Fields Events Actions
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public int fuelMode = 0;

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Status")]
		public string status_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "EC/s (cur/max)")]
		public string ecs_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Max EC/s")]
		public string maxECperSec_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Fuel Used")]
		public string fuelUsed_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = false, guiName = "Byproducts")]
		public string byproducts_t = "ERROR!";

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Enabled:"), UI_Toggle(disabledText = "No", enabledText = "Yes")]
		public bool on = true;

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Previous Fuel Mode")]
		public void prevFuel() {
			if(--fuelMode < 0)
				fuelMode = c.ms.Length - 1;

			udft();
		}

		[KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Next Fuel Mode")]
		public void nextFuel() {
			if(++fuelMode >= c.ms.Length)
				fuelMode = 0;

			udft();
		}

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rate Limit:", guiFormat = "P0"), UI_FloatRange(minValue = percentMin, maxValue = percentMax, stepIncrement = percentStop)]
		public float rateLimit = 1f;

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Threshold:", guiFormat = "P0"), UI_FloatRange(minValue = percentMin, maxValue = percentMax, stepIncrement = percentStop)]
		public float threshold = percentMin;

		[KSPAction("Toggle")]
		public void tg(KSPActionParam kap) {
			on = !on;
		}

		[KSPAction("Enable")]
		public void en(KSPActionParam kap) {
			on = true;
		}

		[KSPAction("Disable")]
		public void ds(KSPActionParam kap) {
			on = false;
		}

		[KSPAction("Previous Fuel Mode")]
		public void pfm(KSPActionParam kap) {
			prevFuel();
		}

		[KSPAction("Next Fuel Mode")]
		public void nfm(KSPActionParam kap) {
			nextFuel();
		}

		[KSPAction("Decrease Rate Limit")]
		public void decRateLimit(KSPActionParam kap) {
			rateLimit = Math.Max(rateLimit - percentStop, percentMin);
		}

		[KSPAction("Increase Rate Limit")]
		public void incRateLimit(KSPActionParam kap) {
			rateLimit = Math.Min(rateLimit + percentStop, percentMax);
		}

		[KSPAction("Decrease Threshold")]
		public void decThreshold(KSPActionParam kap) {
			threshold = Math.Max(threshold - percentStop, percentMin);
		}

		[KSPAction("Increase Threshold")]
		public void incThreshold(KSPActionParam kap) {
			threshold = Math.Min(threshold + percentStop, percentMax);
		}
		#endregion

		#region Private Functions
		private void udfs(out string s, fuel[] fl) {
			if(fl.Length == 0) {
				s = "None";
				return;
			}

			s = "";
			bool plus = false;

			foreach(fuel f in fl) {
				if(plus)
					s += " + ";
				
				plus = true;
				rla abr = rlas.Find(x => x.rid == f.rid);

				if(abr == default(rla))	// If we're missing a resource abbreviation (bad!)
					s += PartResourceLibrary.Instance.GetDefinition(f.rid).name;
				else	// Found one (good!)
					s += abr.shrt;
			}
		}

		private void udft() {
			udfs(out fuelUsed_t, c.ms[fuelMode].fuels);
			udfs(out byproducts_t, c.ms[fuelMode].bypr);

			foreach(ConfigNode.Value cnv in c.ms[fuelMode].tanks) {
				foreach(MeshRenderer mr in part.FindModelComponents<MeshRenderer>(cnv.name))
					mr.material.mainTexture = GameDatabase.Instance.GetTexture(cnv.value, false);
			}
 
            foreach(emttr e in c.ms[fuelMode].emttrs) {

                foreach (KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>(e.name) ?? new List<KSPParticleEmitter>())
					kpe.colorAnimation = e.clrs;
			}

            foreach(light l in c.ms[fuelMode].lights) {
				foreach(Light lim in part.FindModelComponents<Light>(l.name) ?? new List<Light>())
					lim.color = l.clr;
			}
		}

		private void uds(states newstate, Double gen, Double max) {
			if(gen != lgen || max != lmax) {
				lgen = gen;
				lmax = max;

				ecs_t = gen.ToString(FTFMT) + " / " + max.ToString(FTFMT);
			}

			if(state != newstate) {
				state = newstate;
			
				switch(state) {
					case states.fuelDepr: {
						status_t = "Fuel Deprived";
						break;
					}
					case states.noDemand: {
						status_t = "No Demand";
						break;
					}
					case states.nominal: {
						status_t = "Nominal";
						break;
					}
					case states.deploy: {
						status_t = "Deploying";
						amfd(-1);
						break;
					}
					case states.retract: {
						status_t = "Retracting";
						amfd(1);
						break;
					}
					case states.off: {
						status_t = "Off";
						break;
					}
#if DEBUG
					default: {
						status_t = "ERROR!";
						break;
					}
#endif
				}
			}
			
			Double tf = gen / max * rateLimit;
			if(tf != ltf || fuelMode != lfm) {
				ltf = tf;
				lfm = fuelMode;

                 foreach(light l in c.ms[fuelMode].lights) {
					foreach(Light lim in part.FindModelComponents<Light>(l.name) ?? new List<Light>())
						lim.intensity = Convert.ToSingle(l.mmag * tf);
				}
				tf *= c.em;
				int min = (state == states.nominal ? 1 : 0);
                
				foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>()) {
					emttr e = Array.Find(c.ms[fuelMode].emttrs, x => x.name == kpe.name);
					kpe.minEmission = kpe.maxEmission = Math.Max((int)(tf * (e == default(emttr) ? 1 : e.scale)), min);
				}
			}
		}

		private void amfd(float sp) {
			if(a == null)
				return;

			ast.speed = sp;

			if(!a.isPlaying)
				a.Play();
		}

		private states stchk() {
			if(a == null)
				return on ? states.nominal : states.off;

			// Unity is stupid and doesn't have a WrapMode to stop when AnimationState.normalizedTime == 1 without resetting it to 0.  WTF?!
			if(ast.normalizedTime < 0 || ast.normalizedTime > 1) {
				float nnt = Mathf.Clamp(ast.normalizedTime, 0, 1);
				a.Stop();	// This screws with time/normalizedTime, so we have to set it back to what it should be below
				ast.normalizedTime = nnt;
			}

			return on ? (ast.normalizedTime == 0 ? states.nominal : states.deploy) : (ast.normalizedTime == 1 ? states.off : states.retract);
		}

		private string resrates(ConfigNode nd) {
			if(nd == null || nd.values.Count < 1)
				return "\n - None";

			string rrs = "";

			foreach(ConfigNode.Value v in nd.values) {
				float rate = float.Parse(v.value);
				string sfx = "/s";

				if(rate <= 0.004444444f) {
					rate *= 3600;
					sfx = "/h";
				} else if(rate < 0.2666667f) {
					rate *= 60;
					sfx = "/m";
				}

				rrs += "\n - " + v.name + ": " + rate.ToString() + sfx;
			}

			return rrs;
		}

		private void kommit(fuel[] fl, Double adjr) {
			foreach(fuel f in fl)
                part.RequestResource(f.rid, f.rate * adjr);
		}
		#endregion

		#region Public Functions
		public override void OnLoad(ConfigNode cn) {
			if(string.IsNullOrEmpty(scn)) {
				this.cn = cn;			// Needed for GetInfo()
				scn = cn.ToString();	// Needed for marshalling
			}
		}

		public override void OnStart(StartState ss) {
			a = part.FindModelComponent<Animation>();
			ast = (a == null) ? null : a[a.clip.name];

			if(electricChargeID == default(int))
				electricChargeID = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;

			cn = ConfigNode.Parse(scn).GetNode("MODULE");
			c = new cfg(cn, part);

			foreach(ConfigNode.Value cnv in cn.GetNode("FSHORT").values) {
				int rid = PartResourceLibrary.Instance.GetDefinition(cnv.name).id;

				if(!rlas.Exists(x => x.rid == rid))
					rlas.Add(new rla(rid, cnv.value));
			}

			// One kitten will explode for every question you ask about this code.  Please, think of the kittens. {
			if(ns) {
				ns = false;

				foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>() ?? new List<KSPParticleEmitter>()) {
					// Why isn't there just one variable (Vector3 for shape3D) and then just use only the floats you need in that?  Oh, that would make too much sense.
					kpe.shape3D			*= c.sh;
					kpe.shape2D			*= c.sh;
					kpe.shape1D			*= c.sh;
					kpe.minSize			*= c.sh;
					kpe.maxSize			*= c.sh;
					kpe.rndVelocity	*= c.sh;
					kpe.localVelocity	*= c.sh;
					kpe.force			*= c.sh;
					kpe.rndForce		*= c.sh;
				}

				foreach(Light l in part.FindModelComponents<Light>() ?? new List<Light>())
					l.range *= c.sh;
			}
			// }

			udft();

			if(c.ms.Length < 2) {	// Disable unneccessary UI elements if we only have a single mode
				Events["nextFuel"].guiActive			= false;
				Events["nextFuel"].guiActiveEditor	    = false;
				Fields["fmod_t"].guiActive				= false;
				Fields["fmod_t"].guiActiveEditor		= false;
				Actions["pfm"].active					= false;
				Actions["nfm"].active					= false;
			} else {						// If we have at least 2 modes
				if(c.ms.Length > 2) {		// If we have at least 3 modes
					Events["prevFuel"].guiActive			= true;
					Events["prevFuel"].guiActiveEditor	= true;
				} else {							// If we have exactly 2 modes
					Actions["pfm"].active					= false;
				}

				foreach(mode m in c.ms) {	// Show byproducts tweakable if at least one mode has at least one byproduct
					if(m.bypr.Length > 0) {
						Fields["bypr_t"].guiActive			= true;
						Fields["bypr_t"].guiActiveEditor	= true;
						break;
					}
				}
			}

			if(ss != StartState.Editor)
				part.force_activate();
		}

		public override string GetInfo() {
			// As annoying as it is, pre-parsing the config MUST be done here, because this is called during part loading.
			// The config is only fully parsed after everything is fully loaded (which is why it's in OnStart())
			if(info == string.Empty) {
				ConfigNode[] mds = cn.GetNodes("MODE");
				info += "Modes: " + mds.Length.ToString();

				for(byte n = 0; n < mds.Length; n++)
					info += "\n\n<color=#99FF00FF>Mode: " + n.ToString() + "</color> - Max EC: " + mds[n].GetValue("MaxEC") +
						"/s\n<color=#FFFF00FF>Fuels:</color>" + resrates(mds[n].GetNode("FUELS")) +
						"\n<color=#FFFF00FF>Byproducts:</color>" + resrates(mds[n].GetNode("BYPRODUCTS"));
			}

			return info;
		}

		public override void OnFixedUpdate() {
			states ns = stchk();

			if(ns != states.nominal) {
				uds(ns, 0, 0);
				return;
			}

			Double amt = 0, tot = 0;
			List<PartResource> pr = new List<PartResource>();
            /* pr.part.partInfo.title */
            //part.GetConnectedResource(ecid, ResourceFlowMode.ALL_VESSEL, pr);
            part.GetConnectedResourceTotals(electricChargeID, out amt, out tot);

			foreach(PartResource r in pr) {
				tot += r.maxAmount;
				amt += r.amount;
			}

			Double
				cft = TimeWarp.fixedDeltaTime,
				ecn = (Double)(tot * threshold - amt),
				mecrl	= c.ms[fuelMode].maxec * rateLimit;

			cft = Math.Min(cft, ecn / mecrl);	// Determine activity based on supply/demand

			if(cft <= 0) {
				uds(states.noDemand, 0, mecrl);
				return;
			}

			foreach(fuel f in c.ms[fuelMode].fuels) {	// Determine activity based on available fuel
				amt = 0;
				pr.Clear(); // Might not be necessary, but safer
                //part.GetConnectedResources(f.rid, f.rfm, pr);
                part.GetConnectedResourceTotals(electricChargeID, out amt, out tot);

                foreach (PartResource r in pr)
					amt += r.amount;

				cft = Math.Min(cft, ((Double)amt) / (f.rate * rateLimit));
			}

			if(cft == 0) {
				uds(states.fuelDepr, 0, mecrl);
				return;
			}

			Double adjr = rateLimit * cft;			// Calculate usage based on rate limiting and duty cycle
			kommit(c.ms[fuelMode].fuels, adjr);			// Commit changes to fuel used
			kommit(c.ms[fuelMode].bypr, adjr);			// Handle byproducts

			Double eca = mecrl * cft;
            part.RequestResource(electricChargeID, -eca);   // Don't forget the most important part
            uds(states.nominal, eca / TimeWarp.fixedDeltaTime, mecrl);
		}

        //private void uds(states nominal, Double v, Double mecrl)
        //{
        //    throw new NotImplementedException();
        //}

        private void uds(states fuelDepr, int v, Double mecrl)
        {
            throw new NotImplementedException();
        }

        public void Update() {
			if(HighLogic.LoadedSceneIsEditor) {
				Double nmax = c.ms[fuelMode].maxec * rateLimit;
				
				if(lmax != nmax) {
					lmax = nmax;
					maxECperSec_t = lmax.ToString(FTFMT);
				}

				states ns = stchk();
                uds(ns, ns == states.nominal ? 1 : 0, 1);
			}
		}
		#endregion
	}
}
