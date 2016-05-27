using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ModbusTools.Common;

namespace ModbusTools.Launcher.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly Lazy<string> _license;

        public AboutViewModel()
        {
            _license = new Lazy<string>(() =>
                {
                    var assembly = this.GetType().Assembly;

                    return assembly.ReadManifestResourceStream("ModbusTools.Launcher.Licenses.Licenses.txt");
                });
        }
        
        public string Version
        {
            get { return this.GetType().Assembly.GetName().Version.ToString(); }
        }

        public string License
        {
            get { return _license.Value; }
        }
    }
}
