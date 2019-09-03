using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

// This will add a tab to the Stock Settings in the Difficulty settings called "On Demand Fuel Cells"
// To use, reference the setting using the following:
//
//  HighLogic.CurrentGame.Parameters.CustomParams<ODFC_Options>().needsECtoStart
//
// As it is set up, the option is disabled, so in order to enable it, the player would have
// to deliberately go in and change it
//
namespace ODFC
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class ODFC_Options : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Default Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "ODFCr"; } }
        public override string DisplaySection { get { return "On Demand Fuel Cells"; } }
        public override int SectionOrder { get { return 1; } }


        [GameParameters.CustomParameterUI("Require EC to start")]
        public bool needsECtoStart = false;

        // If you want to have some of the game settings default to enabled,  change 
        // the "if false" to "if true" and set the values as you like


#if true
        public override bool HasPresets { get { return true; } }
        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Debug.Log("Setting difficulty preset");
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    needsECtoStart = false;
                    break;

                case GameParameters.Preset.Normal:
                    needsECtoStart = false;
                    break;

                case GameParameters.Preset.Moderate:
                    needsECtoStart = true;
                    break;

                case GameParameters.Preset.Hard:
                    needsECtoStart = true;
                    break;
            }
        }

#else
        public override bool HasPresets { get { return false; } }
        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
#endif

        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }
}

   