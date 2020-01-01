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
        [ConditionalAttribute("DEBUG")]
        internal static void dbg(string msg)
        {
            #if DEBUG
                ScreenMessages.PostScreenMessage(msg, 1, ScreenMessageStyle.UPPER_CENTER, true);
                // for ingame mail 
                UnityEngine.Debug.Log(msg);
            #endif
        }
    }
}
