using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusTools.Common;
using Xceed.Wpf.DataGrid.Converters;

namespace ModbusTools.SlaveViewer.Model
{
    internal class DescriptionStore
    {
        private readonly IPreferences _preferences;

        public DescriptionStore()
        {
            var preferencesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ModbusTools", "registerViewerDescriptions.xml");

            _preferences = new DifferentialPreferences(preferencesPath);
        }

        public void Save()
        {
            _preferences.Save();
        }

        private string GetKey(ushort registerNumber)
        {
            return string.Format("{0}_{1}_{2}", this.DeviceAddress, this.RegisterType, registerNumber);
        }

        public byte DeviceAddress { get; set; }

        public RegisterType RegisterType { get; set; }

        public string this[ushort registerNumber]
        {
            get { return _preferences[GetKey(registerNumber)]; }
            set { _preferences[GetKey(registerNumber)] = value; }
        }
    }
}
