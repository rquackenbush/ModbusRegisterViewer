using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ModbusTools.Common
{
    public class Preferences : IPreferences
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        private readonly string _path;

        private bool _isDirty;

        [DebuggerNonUserCode]
        public Preferences(string path)
        {
            _path = path;

            OperationAttempter.Attempt(() =>
            {
                var serializer = new XmlSerializer(typeof (SavedSettingsContainer));

                using (var file = File.Open(_path, FileMode.Open))
                {
                    var toLoad = (SavedSettingsContainer)serializer.Deserialize(file);

                    if (toLoad != null && toLoad.Settings != null)
                    {
                        foreach (var setting in toLoad.Settings)
                        {
                            _values[setting.Key] = setting.Value;
                        }
                    }
                }
            });
        }

        private string Read(string key)
        {
            string value;

            if (_values.TryGetValue(key, out value))
                return value;

            return null;
        }

        private void Write(string key, string value)
        {
            if (value == null)
            {
                if (_values.ContainsKey(key))
                {
                    _values.Remove(key);

                    _isDirty = true;
                }

            }
            else
            {
                string existingValue;

                if (!_values.TryGetValue(key, out existingValue) || string.CompareOrdinal(existingValue, value) != 0)
                {
                    _isDirty = true;

                    _values[key] = value;    
                } 
            }
        }

        public void Save()
        {
            if (_isDirty)
            {
                OperationAttempter.Attempt(() =>
                {
                    FileUtilities.CreateDirectoryForFile(_path);

                    var toSave = new SavedSettingsContainer()
                    {
                        Settings = _values.Select(p => new SavedSetting()
                        {
                            Key = p.Key,
                            Value = p.Value
                        }).ToArray()
                    };

                    var serializer = new XmlSerializer(typeof(SavedSettingsContainer));

                    using (var file = File.Open(_path, FileMode.Create))
                    {
                        serializer.Serialize(file, toSave);
                    }    
                });
            }
        }

        public void Clear()
        {
            _values.Clear();

            _isDirty = true;
        }


        public string this[string key]
        {
            get { return Read(key); }
            set { Write(key, value); }
        }
    }

    public class SavedSettingsContainer
    {
        public SavedSetting[] Settings { get; set; }
    }

    public class SavedSetting
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
