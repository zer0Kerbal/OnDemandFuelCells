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

namespace OnDemandFuelCells
{
    class IRRescale
    {
        class MyUpdater : TweakScale.IRescalable<ODFC>
        {
            private readonly ODFC pm;

            public MyUpdater(ODFC pm)
            {
                this.pm = pm;
            }

            public void OnRescale(TweakScale.ScalingFactor factor)
            {
                pm.OnRescale(factor.relative);//. .absolute);
            }
        }
    }
}