﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModbusRegisterViewer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public byte SlaveAddress {
            get {
                return ((byte)(this["SlaveAddress"]));
            }
            set {
                this["SlaveAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public ushort StartingRegister {
            get {
                return ((ushort)(this["StartingRegister"]));
            }
            set {
                this["StartingRegister"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public ushort NumberOfRegisters {
            get {
                return ((ushort)(this["NumberOfRegisters"]));
            }
            set {
                this["NumberOfRegisters"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int RegisterType {
            get {
                return ((int)(this["RegisterType"]));
            }
            set {
                this["RegisterType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AdapterSerialNumber {
            get {
                return ((string)(this["AdapterSerialNumber"]));
            }
            set {
                this["AdapterSerialNumber"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SlaveSimulatorAdapterSerialNumber {
            get {
                return ((string)(this["SlaveSimulatorAdapterSerialNumber"]));
            }
            set {
                this["SlaveSimulatorAdapterSerialNumber"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int SlaveSimulatorSlaveAddress {
            get {
                return ((int)(this["SlaveSimulatorSlaveAddress"]));
            }
            set {
                this["SlaveSimulatorSlaveAddress"] = value;
            }
        }
    }
}
