#region license
/*  On Demand Fuel Cells (ODFC) is a plugin to simulate fuel cells in 
    Kerbal Space Program (KSP), and do a better job of it than stock's
    use of a resource converter.
    
    Copyright (C) 2014 by Orum
    Copyright (C) 2017, 2022 by Orum and zer0Kerbal (zer0Kerbal at hotmail dot com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
*/
#endregion

//#define DEBUG

using System;

namespace OnDemandFuelCells
{
    public class ResourceLabel
    { //resource la? originally rla
        public int resourceID;
        private string resourceAbbreviation;

        private ResourceLabel(int resourceID, string resourceAbbreviation)
        {
            this.resourceID = resourceID;
            this.resourceAbbreviation = resourceAbbreviation;
        }
    }

    /// <summary></summary>
    public struct Fuel
    {
        public int resourceID;
        public float rate;
        public ResourceFlowMode resourceFlowMode;

        public Fuel(ConfigNode.Value nodeValue, bool bp = false)
        {
            resourceID = PartResourceLibrary.Instance.GetDefinition(nodeValue.name).id;
            rate = bp ? -float.Parse(nodeValue.value) : float.Parse(s: nodeValue.value);
            resourceFlowMode = PartResourceLibrary.GetDefaultFlowMode(resourceID);
        }
    }

    /// <summary></summary>
    public struct mode
    {
        public Fuel[] fuels, byproducts;
        public double maxEC;

        public mode(ConfigNode node, Part part)
        {
            maxEC = 10;

            foreach (ConfigNode.Value nodeValue in node.values ?? new ConfigNode.ValueList())
            {
                switch (nodeValue.name)
                {
                    case "MaxEC":
                        {
                            maxEC = double.Parse(nodeValue.value);
                            break;
                        }
                }
            }

            ConfigNode.ValueList nodeValues = node.GetNode("FUELS").values; // No null coalescing intentional here
            fuels = new Fuel[nodeValues.Count];

            for (byte n = 0; n < nodeValues.Count; n++)
                fuels[n] = new Fuel(nodeValues[n]);

            nodeValues = (node.GetNode("BYPRODUCTS") ?? new ConfigNode()).values;
            byproducts = new Fuel[nodeValues.Count];

            for (byte n = 0; n < nodeValues.Count; n++)
                byproducts[n] = new Fuel(nodeValues[n], true);
        }
    }

    public struct Config
    {
        public mode[] modes;
        //  public bool autoSwitch { get; private set; }
        //public double scaleHack;

        public Config(ConfigNode node, Part part)
        {
            /*if (node.HasValues("MODE"))*/
            {
                //        ConfigNode[] nodes = node.GetNodes("ODFC"); // shouldn't need null coalescing as we should always have at least one ODFC module

                // Shouldn't need null coalescing as we should always have at least one MODE
                ConfigNode[] nodes = node.GetNodes("MODE");
                modes = new mode[nodes.Length];

                for (byte i = 0; i < nodes.Length; i++)
                    modes[i] = new mode(nodes[i], part);
            }
            /*            else
                        {
                            modes = null;
                            Log.dbg("Malformed config node: MODE");
                        }*/
        }
    }
}