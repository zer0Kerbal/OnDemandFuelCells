using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODFC
{
    class KerbalTraits
    {
        /* TODO: create generic kerbal trait class (is Eng onboard?) (what is the fuelEffeciency bonus)
        */
        /*private bool HasSkill()
        {
            if (FlightGlobals.ActiveVessel.GetCrewCount() > 0)
            {
                //if repair is done by EVA success is 40%
                if (FlightGlobals.ActiveVessel.FindPartModuleImplementing<KerbalEVA>() != null) repairChance = 0.4f;
                for (int i = 0; i < FlightGlobals.ActiveVessel.GetVesselCrew().Count(); i++)
                {
                    //Engineers give a 10% bonus per level to repair rates
                    ProtoCrewMember p = FlightGlobals.ActiveVessel.GetVesselCrew().ElementAt(i);
                    for (int ii = 0; ii < p.experienceTrait.Effects.Count(); ii++)
                        if (p.experienceTrait.Effects[ii].Name == "FailureRepairSkill")
                        {
                            if (p.experienceTrait.Effects[ii].LevelModifiers.Length > 0)
                            {
                                //repairChance += p.experienceTrait.Effects[ii].LevelModifiers[p.experienceLevel];
                                Log.dbg(String.Format("[OhScrap]: name {0) experiencetrait {1) level {2) modifier {3)", p.name, p.experienceTrait.ToString(), p.experienceLevel.ToString(), p.experienceTrait.Effects[ii].LevelModifiers[p.experienceLevel].ToString()));
                            }
                            //else repairChance += p.experienceLevel * 0.1f;
                            //break;
                        }
                }
            }
            Log.dbg("[OhScrap]: Repair Chance: " + repairChance + " Rolled: " + roll + " Success: " + (roll < repairChance).ToString());
            return true; // roll < repairChance;
        }*/
    }
}
