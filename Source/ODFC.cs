#define DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODFC {
	public class ODFC : PartModule {
		#region Enums Vars
		public enum states : byte { error, off, nominal, deploy, retract, fuelDepr, noDeman };

		private const string FTFMT = "0.##";
		private const float
			PCTSTP = 0.05f,
			PCTMIN = PCTSTP,
			PCTMAX = 1;

		private Animation a;
		private AnimationState ast;
		private float
			lgen = -1,
			lmax = -1,
			ltf = -1;
		private int lfm = -1;
		private string info = string.Empty;

		public ConfigNode cn;
		public static List<rla> rlas = new List<rla>();
		public static int ecid;
		public string scn;
		public bool ns = true;
		public cfg c;
		public states state = states.error;
		#endregion

		#region Fields Events Actions
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public int fm = 0;

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Status")]
		public string state_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "EC/s (cur/max)")]
		public string ecs_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Max EC/s")]
		public string mecs_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Fuel Used")]
		public string fmod_t = "ERROR!";

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = false, guiName = "Byproducts")]
		public string bypr_t = "ERROR!";

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Enabled:"), UI_Toggle(disabledText = "No", enabledText = "Yes")]
		public bool on = true;

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Previous Fuel Mode")]
		public void prevFuel() {
			if(--fm < 0)
				fm = c.ms.Length - 1;

			udft();
		}

		[KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Next Fuel Mode")]
		public void nextFuel() {
			if(++fm >= c.ms.Length)
				fm = 0;

			udft();
		}

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rate Limit:", guiFormat = "P0"), UI_FloatRange(minValue = PCTMIN, maxValue = PCTMAX, stepIncrement = PCTSTP)]
		public float ratelmt = 1f;

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Threshold:", guiFormat = "P0"), UI_FloatRange(minValue = PCTMIN, maxValue = PCTMAX, stepIncrement = PCTSTP)]
		public float thresh = PCTMIN;

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
		public void inRL(KSPActionParam kap) {
			ratelmt = Math.Max(ratelmt - PCTSTP, PCTMIN);
		}

		[KSPAction("Increase Rate Limit")]
		public void deRL(KSPActionParam kap) {
			ratelmt = Math.Min(ratelmt + PCTSTP, PCTMAX);
		}

		[KSPAction("Decrease Threshold")]
		public void inTh(KSPActionParam kap) {
			thresh = Math.Max(thresh - PCTSTP, PCTMIN);
		}

		[KSPAction("Increase Threshold")]
		public void deTh(KSPActionParam kap) {
			thresh = Math.Min(thresh + PCTSTP, PCTMAX);
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
			udfs(out fmod_t, c.ms[fm].fuels);
			udfs(out bypr_t, c.ms[fm].bypr);

			foreach(ConfigNode.Value cnv in c.ms[fm].tanks) {
				foreach(MeshRenderer mr in part.FindModelComponents<MeshRenderer>(cnv.name))
					mr.material.mainTexture = GameDatabase.Instance.GetTexture(cnv.value, false);
			}

			foreach(emttr e in c.ms[fm].emttrs) {
				foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>(e.name) ?? new KSPParticleEmitter[0])
					kpe.colorAnimation = e.clrs;
			}

			foreach(light l in c.ms[fm].lights) {
				foreach(Light lim in part.FindModelComponents<Light>(l.name) ?? new Light[0])
					lim.color = l.clr;
			}
		}

		private void uds(states newstate, float gen, float max) {
			if(gen != lgen || max != lmax) {
				lgen = gen;
				lmax = max;

				ecs_t = gen.ToString(FTFMT) + " / " + max.ToString(FTFMT);
			}

			if(state != newstate) {
				state = newstate;
			
				switch(state) {
					case states.fuelDepr: {
						state_t = "Fuel Deprived";
						break;
					}
					case states.noDeman: {
						state_t = "No Demand";
						break;
					}
					case states.nominal: {
						state_t = "Nominal";
						break;
					}
					case states.deploy: {
						state_t = "Deploying";
						amfd(-1);
						break;
					}
					case states.retract: {
						state_t = "Retracting";
						amfd(1);
						break;
					}
					case states.off: {
						state_t = "Off";
						break;
					}
#if DEBUG
					default: {
						state_t = "ERROR!";
						break;
					}
#endif
				}
			}
			
			float tf = gen / max * ratelmt;
			if(tf != ltf || fm != lfm) {
				ltf = tf;
				lfm = fm;

				foreach(light l in c.ms[fm].lights) {
					foreach(Light lim in part.FindModelComponents<Light>(l.name) ?? new Light[0])
						lim.intensity = l.mmag * tf;
				}

				tf *= c.em;
				int min = (state == states.nominal ? 1 : 0);

				foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>()) {
					emttr e = Array.Find(c.ms[fm].emttrs, x => x.name == kpe.name);
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

		private void kommit(fuel[] fl, float adjr) {
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

			if(ecid == default(int))
				ecid = PartResourceLibrary.Instance.GetDefinition("ElectricCharge").id;

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

				foreach(KSPParticleEmitter kpe in part.FindModelComponents<KSPParticleEmitter>() ?? new KSPParticleEmitter[0]) {
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

				foreach(Light l in part.FindModelComponents<Light>() ?? new Light[0])
					l.range *= c.sh;
			}
			// }

			udft();

			if(c.ms.Length < 2) {	// Disable unneccessary UI elements if we only have a single mode
				Events["nextFuel"].guiActive			= false;
				Events["nextFuel"].guiActiveEditor	= false;
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

			double amt = 0, tot = 0;
			List<PartResource> pr = new List<PartResource>();
			part.GetConnectedResources(ecid, ResourceFlowMode.ALL_VESSEL, pr);

			foreach(PartResource r in pr) {
				tot += r.maxAmount;
				amt += r.amount;
			}

			float
				cft = TimeWarp.fixedDeltaTime,
				ecn = (float)(tot * thresh - amt),
				mecrl	= c.ms[fm].maxec * ratelmt;

			cft = Math.Min(cft, ecn / mecrl);	// Determine activity based on supply/demand

			if(cft <= 0) {
				uds(states.noDeman, 0, mecrl);
				return;
			}

			foreach(fuel f in c.ms[fm].fuels) {	// Determine activity based on available fuel
				amt = 0;
				pr.Clear();	// Might not be necessary, but safer
				part.GetConnectedResources(f.rid, f.rfm, pr);

				foreach(PartResource r in pr)
					amt += r.amount;

				cft = Math.Min(cft, ((float)amt) / (f.rate * ratelmt));
			}

			if(cft == 0) {
				uds(states.fuelDepr, 0, mecrl);
				return;
			}

			float adjr = ratelmt * cft;			// Calculate usage based on rate limiting and duty cycle
			kommit(c.ms[fm].fuels, adjr);			// Commit changes to fuel used
			kommit(c.ms[fm].bypr, adjr);			// Handle byproducts

			float eca = mecrl * cft;
			part.RequestResource(ecid, -eca);	// Don't forget the most important part
			uds(states.nominal, eca / TimeWarp.fixedDeltaTime, mecrl);
		}

		public void Update() {
			if(HighLogic.LoadedSceneIsEditor) {
				float nmax = c.ms[fm].maxec * ratelmt;
				
				if(lmax != nmax) {
					lmax = nmax;
					mecs_t = lmax.ToString(FTFMT);
				}

				states ns = stchk();
				uds(ns, ns == states.nominal ? 1 : 0, 1);
			}
		}
		#endregion
	}
}
