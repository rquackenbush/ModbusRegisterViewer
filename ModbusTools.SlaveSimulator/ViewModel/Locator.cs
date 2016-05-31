using GalaSoft.MvvmLight;

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
