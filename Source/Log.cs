#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ODFC
{
    class Log
    {
        /// <summary>
        ///   sends the specific message to ingame mail and screen if Debug is defined
        ///   For debugging use only.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        [ConditionalAttribute("DEBUG")]
        internal static void dbg(string msg)
        {
         #if DEBUG
            ScreenMessages.PostScreenMessage(msg, 1, ScreenMessageStyle.UPPER_CENTER, true);
            // for ingame mail 
            UnityEngine.Debug.Log(msg);
            DebugLog(msg);
                
        #endif
        }

        /// <summary>
        ///   sends the specific message to the game log if Debug is defined
        ///   For debugging use only.
        /// </summary>
        /// <param name="m">The m.</param>
        [Conditional(conditionString: "DEBUG")]
        private static void DebugLog(object m) => UnityEngine.Debug.LogError(message: $"[ODFC]: {m}");

    }
}
