//#define DEBUG

using System;
using UnityEngine;

namespace ODFC {
	public class resourceLa { //resource la? 
		public int resourceID;
		public string ressourceAbbreviation;

		public resourceLa(int resourceID, string ressourceAbbreviation) {
			this.resourceID = resourceID;
			this.ressourceAbbreviation = ressourceAbbreviation;
		}
	}

	public struct Fuel {
		public int resourceID;
		public float rate;
		public ResourceFlowMode resourceFlowMode;

		public Fuel(ConfigNode.Value nodeValue, bool bp = false) {
			resourceID = PartResourceLibrary.Instance.GetDefinition(nodeValue.name).id;
			rate = (bp ? -float.Parse(nodeValue.value) : float.Parse(nodeValue.value));
			resourceFlowMode = PartResourceLibrary.GetDefaultFlowMode(resourceID);
		}
	}

/*	public class emmitter {
		public string name;
		public Color[] colors;
		public float scale;

		public emmitter(ConfigNode.Value nodeValue) {
			name = nodeValue.name;
			scale = 1;

			string[] scs = nodeValue.value.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			colors = new Color[5];

			for(byte x = 0; x < 5; x++)
				colors[x] = ConfigNode.ParseColor(x > scs.Length - 1 ? "1,1,1" : scs[x] + "," + Mathf.Clamp(6f / x - 1.25f, 0f, 1f).ToString());	// Yes it's lame to convert to a string just to convert it back to a float, but I don't care
		}
	}*/

/*	public class light {
		public string name;
		public Color color;
		public Double mmag;

		public light(ConfigNode.Value value, Part part) {
			name = value.name;
			color = ConfigNode.ParseColor(value.value);
			mmag = part.FindModelComponent<Light>(name).intensity;
		}
	}*/

	public struct mode {
		public Fuel[] fuels, byproducts;
/*		public ConfigNode.ValueList tanks;
		public emmitter[] emttrs;
		public light[] lights;
*/		public Double maxEC;

		public mode(ConfigNode node, Part part) {
			maxEC = 10;

			foreach(ConfigNode.Value nodeValue in node.values ?? new ConfigNode.ValueList()) {
				switch(nodeValue.name) {
					case "MaxEC": {
						maxEC = Double.Parse(nodeValue.value);
						break;
					}
				}
			}

			ConfigNode.ValueList nodeValues = node.GetNode("FUELS").values;	// No null coalescing intentional here
			fuels = new Fuel[nodeValues.Count];

			for(byte n = 0; n < nodeValues.Count; n++)
				fuels[n] = new Fuel(nodeValues[n]);

			nodeValues = (node.GetNode("BYPRODUCTS") ?? new ConfigNode()).values;
			byproducts = new Fuel[nodeValues.Count];

			for(byte n = 0; n < nodeValues.Count; n++)
				byproducts[n] = new Fuel(nodeValues[n], true);

/*			tanks = (node.GetNode("TANKS") ?? new ConfigNode()).values;

			nodeValues = (node.GetNode("EMITTERS") ?? new ConfigNode()).values;
			emttrs = new emmitter[nodeValues.Count];

			for(byte n = 0; n < nodeValues.Count; n++)
				emttrs[n] = new emmitter(nodeValues[n]);

			foreach(ConfigNode.Value cnv in (node.GetNode("EMITSCALE") ?? new ConfigNode()).values)
				Array.Find(emttrs, x => x.name == cnv.name).scale = Double.Parse(cnv.value);

			nodeValues = (node.GetNode("LIGHTS") ?? new ConfigNode()).values;
			lights = new light[nodeValues.Count];

			for(byte i = 0; i < nodeValues.Count; i++)
				lights[i] = new light(nodeValues[i], part);*/
		}
	}

	public struct cfg {
		public mode[] modes;
		public Double scaleHack;
		//public int emissionCoefficient;

		public cfg(ConfigNode node, Part part) {
			scaleHack = 1;
			//emissionCoefficient = 50;

			foreach(ConfigNode.Value v in node.values ?? new ConfigNode.ValueList()) {
				switch(v.name) {
					case "ScaleHack": {
						scaleHack = Double.Parse(v.value);
						break;
					}
					//case "EmitCoeff": {
					//	emissionCoefficient = int.Parse(v.value);
					//	break;
					//}
				}
			}

			ConfigNode[] nodes = node.GetNodes("MODE");	// Shouldn't need null coalescing as we should always have at least one MODE
			modes = new mode[nodes.Length];

			for(byte i = 0; i < nodes.Length; i++)
				modes[i] = new mode(nodes[i], part);
		}
	}
}