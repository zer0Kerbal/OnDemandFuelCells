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
namespace OnDemandFuelCells
{
    /// <summary>search for "Mod integration into Stock Settings
    /// http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813 /// </summary>
    public class Options : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "#ODFC-set-title"; } }		// Default Settings
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string DisplaySection { get { return "#ODFC-modname"; } } // On Demand Fuel Cells
        public override string Section { get { return "#ODFC-manu-name"; } }		   // On Demand Fuel Cells
        public override int SectionOrder { get { return 1; } }

        /// <summary>The needs EC to start in GameParameters</summary>
        [GameParameters.CustomParameterUI("#ODFC-set-ec-run",	// Require EC to run
            toolTip = "#ODFC-set-ec-run-tt",		            // if set to yes, the fuel cells will 'stall' if the vessels total electric charge reaches zero and will not function until vessel electric charge is above zero.
            newGameOnly = false,
            unlockedDuringMission = true
            )]
        public bool needsECtoStart = false;

        /// <summary>The needs EC to start in GameParameters</summary>
        [GameParameters.CustomParameterUI("#ODFC-set-ec-cons",	// Consumes EC
            toolTip = "#ODFC-set-ec-cons-tt",                   // if set to yes, the fuel cells will consume electric charge to operate.
            newGameOnly = false,
            unlockedDuringMission = true
            )]
        public bool consumesEC = false;

        /// <summary>The automatic switch in GameParameters</summary>
        [GameParameters.CustomParameterUI("#ODFC-set-switch-auto",	// Auto Fuel Mode Switch
            toolTip = "#ODFC-set-switch-auto-tt",		            // if current fuel mode becomes fuel deprived, will 'hunt' or 'search' for a fuel mode that has fuel.
            newGameOnly = false,
            unlockedDuringMission = true)]
        public bool autoSwitch = true;

        /// <summary>Does ODFC require full control in GameParameters</summary>
        [GameParameters.CustomParameterUI("#ODFC-fc",   // Require Full Control
            toolTip = "#ODFC-fc-tt",		            // requires full control for mode switchingoperation
            newGameOnly = false,
            unlockedDuringMission = true)]
        public static bool requireFullControl = false;

        /// <summary>The option to de-colored PAW/RMB</summary>
        [GameParameters.CustomParameterUI("#ODFC-set-paw-color",    // PAW Color
            toolTip = "#ODFC-set-paw-color-tt",                     // allow color coding in ODFC PAW (part action window) / part RMB (right menu button).
            newGameOnly = false,
            unlockedDuringMission = true)]
        public bool coloredPAW = true;

        /// <summary>This setting turns ON/off ODFC sending mail via in game mail system</summary>
        [GameParameters.CustomParameterUI("#ODFC-set-mail",	// InGameMail? (not implemented yet) (YES/no)
            toolTip = "#ODFC-set-mail-tt",                  // allow On Demand Fuel Cells to send you in game mail (not implemented yet). Default is YES.
            newGameOnly = false,
            unlockedDuringMission = true)]
        public bool _InGameMail = true;

        public bool InGameMail { get { return this._InGameMail; } }

        /// <summary>Sets the globalScalingFactor in GameParameters</summary>
        [GameParameters.CustomFloatParameterUI("#ODFC-set-scaling", // Global Scaling Factor
            toolTip = "#ODFC-set-scaling-tt",		                // Scales production and consumption Globally on all ODFC modules.
            newGameOnly = false,
            unlockedDuringMission = true,
            minValue = 0.05f,
            maxValue = 5.0f,
            stepCount = 101,
            displayFormat = "F2",
            asPercentage = false)]
        public float globalScalingFactor = 1.0f;

        /// <summary>/// Sets the globalScalingFactor in GameParameters/// </summary>
        [GameParameters.CustomFloatParameterUI("#ODFC-set-response", toolTip = "#ODFC-set-response-tt",		// responseTime // Sets responseTime = 1 - 1000 (lower equals faster).
            newGameOnly = false, unlockedDuringMission = true,
            minValue = 1, maxValue = 10000, stepCount = 10,
            //displayFormat = "F2",
            asPercentage = false)]
        public int responseTime = 100;

        /// <summary> Sets the globalScalingFactor in GameParameters </summary>
        [GameParameters.CustomFloatParameterUI("#ODFC-set-paw-graph", toolTip = "#ODFC-set-paw-graph-tt",		// PAW Power Graph // if yes, shows a graphics (and if allowed) color coded power graph in the PAW.
            newGameOnly = false, unlockedDuringMission = true)]
        public bool powerGraph = false;

#if true        
        /// <summary>Gets a value indicating whether this instance has presets.</summary>
        /// <value><c>true</c> if this instance has presets; otherwise, <c>false</c>.</value>
        public override bool HasPresets { get { return true; } }
        /// <summary>Sets the difficulty preset.</summary>
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