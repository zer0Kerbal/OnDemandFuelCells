using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODFC
{
    class Thermal
    {
        /*
TODO: 
	void 	AddSkinThermalFlux (double kilowatts)
	void 	AddThermalFlux (double kilowatts)
    based upon 1kw EC = 1kw Thermal?

*/
    internal void AddThermalkw(double kw)
        {
            AddSkinkw(kw);
            AddInternalkw(kw);
        }
    internal void AddSkinkw(double kilowatts)
        {
            // AddSkinThermalFlux(kw);
            return;
        }
        void AddInternalkw(double kilowatts)
        {
            //AddThermalFlux(kw);
            return;
        }
    }
}
