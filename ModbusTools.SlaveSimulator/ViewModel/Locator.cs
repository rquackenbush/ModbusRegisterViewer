using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ModbusTools.SlaveSimulator.View;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public static class Locator
    {
        public static SlaveSimulatorViewModel SlaveSimulator
        {
            get {  return new SlaveSimulatorViewModel();}
        }

        public static SlaveViewModel Slave
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                    return new SlaveViewModel();

                return null;
            }
        }
    }
}
