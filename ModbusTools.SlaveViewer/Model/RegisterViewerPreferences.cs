using System;
using ModbusTools.Common;

namespace ModbusTools.SlaveViewer.Model
{
    internal class RegisterViewerPreferences
    {
        private readonly IPreferences _preferences;

        internal static class Keys
        {
            private const string Prefix = "SlaveViewer.";

            public const string SlaveAddress = Prefix + "SlaveAddress";
            public const string StartingRegister = Prefix + "StartingRegister";
            public const string NumberOfRegisters = Prefix + "NumberOfRegisters";
            public const string RegisterType = Prefix + "RegisterType";
            public const string ModbusAdapter = Prefix + "ModbusAdapter";
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
