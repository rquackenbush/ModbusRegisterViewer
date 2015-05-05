using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public static class Locator
    {
        public static SlaveSimulatorViewModel SlaveSimulator
        {
            get {  return new SlaveSimulatorViewModel();}
        }
    }
}
