using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using KSP.Localization;

// This will add a tab to the Stock Settings in the Difficulty settings called "On Demand Fuel Cells"
// To use, reference the setting using the following:
//
//  HighLogic.CurrentGame.Parameters.CustomParams<Options>().needsECtoStart
//
// As it is set up, the option is disabled, so in order to enable it, the player would have
// to deliberately go in and change it
//
namespace ODFC
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class Options : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Default Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "On Demand Fuel Cells"; } }
        public override string DisplaySection { get { return "On Demand Fuel Cells"; } }
        public override int SectionOrder { get { return 1; } }

        /// <summary>
        /// The needs EC to start in GameParameters
        /// </summary>
        [GameParameters.CustomParameterUI("Require EC to run",
            toolTip = "if set to yes, the fuel cells will 'stall' if the vessels total electric charge reaches zero and will not function until vessel electric charge is above zero.",
            newGameOnly = false,
            unlockedDuringMission = true
            )]
        public bool needsECtoStart = false;
        /// <summary>
        /// The needs EC to start in GameParameters
        /// </summary>
        [GameParameters.CustomParameterUI("Consumes EC",
            toolTip = "if set to yes, the fuel cells will consume electric charge to operate.",
            newGameOnly = false,
            unlockedDuringMission = true
            )]
        public bool consumesEC = false;

        /// <summary>
        /// The automatic switch in GameParameters
        /// </summary>
        [GameParameters.CustomParameterUI("Auto Fuel Mode Switch",
            toolTip = "if current fuel mode becomes fuel deprived, will 'hunt' or 'search' for a fuel mode that has fuel.",
            newGameOnly = false,
            unlockedDuringMission = true)]
        public bool autoSwitch = true;

        /// <summary>
        /// The colored paw
        /// </summary>
        [GameParameters.CustomParameterUI("PAW Color",
            toolTip = "allow color coding in ODFC PAW (part action window) / part RMB (right menu button).",
            newGameOnly = false,
            unlockedDuringMission = true)]
        public bool coloredPAW = true;

        /// <summary>
        /// This setting turns ON/off ODFC sending mail via in game mail system
        /// </summary>
        [GameParameters.CustomParameterUI("InGameMail? (not implemented yet) (YES/no)",
            toolTip = "allow On Demand Fuel Cells to send you in game mail (not implemented yet). Default is YES.",
            newGameOnly = false,
            unlockedDuringMission = true)]
        public bool _InGameMail = true;

        public bool InGameMail { get { return this._InGameMail; } }

        /// <summary>
        /// Sets the globalScalingFactor in GameParameters
        /// </summary>
        [GameParameters.CustomFloatParameterUI("Global Scaling Factor",
            toolTip = "Scales production and consumption Globally on all ODFC modules.",
            newGameOnly = false,
            unlockedDuringMission = true,
            minValue = 0.05f,
            maxValue = 5.0f,
            stepCount = 101,
            displayFormat = "F2",
            asPercentage = false)]
        public float globalScalingFactor = 1.0f;

        /// <summary>
        /// Sets the globalScalingFactor in GameParameters
        /// </summary>
        [GameParameters.CustomFloatParameterUI("responseTime ",
            toolTip = "Sets responseTime = 1 - 100 (lower equals faster).",
            newGameOnly = false,
            unlockedDuringMission = true,
            minValue = 0,
            maxValue = 100,
            stepCount = 1,
            //displayFormat = "F2",
            asPercentage = false)]
        public int responseTime = 25;

        // If you want to have some of the game settings default to enabled,  change 
        // the "if false" to "if true" and set the values as you like


#if true        
        /// <summary>
        /// Gets a value indicating whether this instance has presets.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has presets; otherwise, <c>false</c>.
        /// </value>
        public override bool HasPresets { get { return true; } }
        /// <summary>
        /// Sets the difficulty preset.
        /// </summary>
        /// <param name="preset">The preset.</param>
        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Debug.Log("Setting difficulty preset");
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    needsECtoStart = false;
                    autoSwitch = true;
                    globalScalingFactor = 1.5f;
                    break;

                case GameParameters.Preset.Normal:
                    needsECtoStart = false;
                    autoSwitch = true;
                    globalScalingFactor = 1.0f;
                    break;

                case GameParameters.Preset.Moderate:
                    needsECtoStart = true;
                    autoSwitch = true;
                    globalScalingFactor = 0.75f;
                    break;

                case GameParameters.Preset.Hard:
                    needsECtoStart = true;
                    autoSwitch = false;
                    globalScalingFactor = 0.5f;
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

   