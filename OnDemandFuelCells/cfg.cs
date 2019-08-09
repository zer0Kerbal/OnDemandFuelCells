//#define DEBUG

using System;
using UnityEngine;

namespace ODFC {
	public class rla {
		public int rid;
		public string shrt;

		public rla(int rid, string shrt) {
			this.rid = rid;
			this.shrt = shrt;
		}
	}

	public struct fuel {
		public int rid;
		public float rate;
		public ResourceFlowMode rfm;

		public fuel(ConfigNode.Value cnv, bool bp = false) {
			rid = PartResourceLibrary.Instance.GetDefinition(cnv.name).id;
			rate = (bp ? -float.Parse(cnv.value) : float.Parse(cnv.value));
			rfm = PartResourceLibrary.GetDefaultFlowMode(rid);
		}
	}

	public class emttr {
		public string name;
		public Color[] clrs;
		public float scale;

		public emttr(ConfigNode.Value cnv) {
			name = cnv.name;
			scale = 1;

			string[] scs = cnv.value.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			clrs = new Color[5];

			for(byte x = 0; x < 5; x++)
				clrs[x] = ConfigNode.ParseColor(x > scs.Length - 1 ? "1,1,1" : scs[x] + "," + Mathf.Clamp(6f / x - 1.25f, 0f, 1f).ToString());	// Yes it's lame to convert to a string just to convert it back to a float, but I don't care
		}
	}

	public class light {
		public string name;
		public Color clr;
		public Double mmag;

		public light(ConfigNode.Value cnv, Part p) {
			name = cnv.name;
			clr = ConfigNode.ParseColor(cnv.value);
			mmag = p.FindModelComponent<Light>(name).intensity;
		}
	}

	public struct mode {
		public fuel[] fuels, bypr;
		public ConfigNode.ValueList tanks;
		public emttr[] emttrs;
		public light[] lights;
		public Double maxec;

		public mode(ConfigNode cn, Part p) {
			maxec = 10;

			foreach(ConfigNode.Value cnv in cn.values ?? new ConfigNode.ValueList()) {
				switch(cnv.name) {
					case "MaxEC": {
						maxec = float.Parse(cnv.value);
						break;
					}
				}
			}

			ConfigNode.ValueList vl = cn.GetNode("FUELS").values;	// No null coalescing intentional here
			fuels = new fuel[vl.Count];

			for(byte n = 0; n < vl.Count; n++)
				fuels[n] = new fuel(vl[n]);

			vl = (cn.GetNode("BYPRODUCTS") ?? new ConfigNode()).values;
			bypr = new fuel[vl.Count];

			for(byte n = 0; n < vl.Count; n++)
				bypr[n] = new fuel(vl[n], true);

			tanks = (cn.GetNode("TANKS") ?? new ConfigNode()).values;

			vl = (cn.GetNode("EMITTERS") ?? new ConfigNode()).values;
			emttrs = new emttr[vl.Count];

			for(byte n = 0; n < vl.Count; n++)
				emttrs[n] = new emttr(vl[n]);

			foreach(ConfigNode.Value cnv in (cn.GetNode("EMITSCALE") ?? new ConfigNode()).values)
				Array.Find(emttrs, x => x.name == cnv.name).scale = float.Parse(cnv.value);

			vl = (cn.GetNode("LIGHTS") ?? new ConfigNode()).values;
			lights = new light[vl.Count];

			for(byte n = 0; n < vl.Count; n++)
				lights[n] = new light(vl[n], p);
		}
	}

	public struct cfg {
		public mode[] ms;
		public float sh;
		public int em;

		public cfg(ConfigNode cn, Part p) {
			sh = 1;
			em = 50;

			foreach(ConfigNode.Value v in cn.values ?? new ConfigNode.ValueList()) {
				switch(v.name) {
					case "ScaleHack": {
						sh = float.Parse(v.value);
						break;
					}
					case "EmitCoeff": {
						em = int.Parse(v.value);
						break;
					}
				}
			}

			ConfigNode[] nds = cn.GetNodes("MODE");	// Shouldn't need null coalescing as we should always have at least one MODE
			ms = new mode[nds.Length];

			for(byte n = 0; n < nds.Length; n++)
				ms[n] = new mode(nds[n], p);
		}
	}
}
