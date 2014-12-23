using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusTools.Common;

namespace ModbusTools.SlaveViewer.Model
{
    internal class RegisterViewerPreferences
    {
        private readonly IPreferences _preferences;

        internal static class Keys
        {
            public const string SlaveAddress = "SlaveViewer.SlaveAddress";
            public const string StartingRegister = "SlaveViewer.StartingRegister";
            public const string NumberOfRegisters = "SlaveViewer.NumberOfRegisters";
            public const string RegisterType = "SlaveViewer.RegisterType";
            public const string ModbusAdapter = "SlaveViewer.ModbusAdapter";
        }

        public RegisterViewerPreferences(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public byte SlaveAddress
        {
            get { return _preferences.GetByte(Keys.SlaveAddress); }
            set { _preferences.WriteSetting(Keys.SlaveAddress, value); }
        }

        public ushort StartingRegister
        {
            get { return _preferences.GetUInt16(Keys.StartingRegister); }
            set { _preferences.WriteSetting(Keys.StartingRegister, value); }
        }

        public ushort NumberOfRegisters
        {
            get { return _preferences.GetUInt16(Keys.NumberOfRegisters); }
            set { _preferences.WriteSetting(Keys.NumberOfRegisters, value); }
        }

        public RegisterType RegisterType
        {
            get { return (RegisterType)_preferences.GetInt32(Keys.RegisterType); }
            set { _preferences.WriteSetting(Keys.RegisterType, (Int32)value); }
        }
    }
}
