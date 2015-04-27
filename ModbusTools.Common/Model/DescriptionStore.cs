using System;
using System.IO;

namespace ModbusTools.Common.Model
{
    public class DescriptionStore
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

        private string GetKey(ushort registerIndex)
        {
            return string.Format("{0}_{1}_{2}", this.DeviceAddress, this.RegisterType, registerIndex);
        }

        public byte DeviceAddress { get; set; }

        public RegisterType RegisterType { get; set; }

        public string this[ushort registerIndex]
        {
            get { return _preferences[GetKey(registerIndex)]; }
            set { _preferences[GetKey(registerIndex)] = value; }
        }
    }
}
