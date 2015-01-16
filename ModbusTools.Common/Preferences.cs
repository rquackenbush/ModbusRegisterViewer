using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ModbusTools.Common
{
    public class Preferences : IPreferences
    {
        protected Dictionary<string, string> _values = new Dictionary<string, string>();

        protected readonly string _path;

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

        protected static IDictionary<string, string> LoadSavedSettings(string path)
        {
            var serializer = new XmlSerializer(typeof (SavedSettingsContainer));

            using (var file = File.Open(path, FileMode.Open))
            {
                var container = (SavedSettingsContainer) serializer.Deserialize(file);

                var settings = new Dictionary<string, string>();

                if (container != null && container.Settings != null)
                {
                    foreach (var setting in container.Settings)
                    {
                        settings[setting.Key] = setting.Value;
                    }
                }

                return settings;
            }
        }

        protected static void SaveSettings(string path, IDictionary<string, string> settings)
        {
            FileUtilities.CreateDirectoryForFile(path);

            var container = new SavedSettingsContainer()
            {
                Settings = settings.Select(p => new SavedSetting()
                {
                    Key = p.Key,
                    Value = p.Value
                }).ToArray()
            };

            var serializer = new XmlSerializer(typeof(SavedSettingsContainer));

            using (var file = File.Open(path, FileMode.Create))
            {
                serializer.Serialize(file, container);
            }    
        }

        protected virtual bool IsDirty
        {
            get { return _isDirty; }
        }

        protected virtual string Read(string key)
        {
            string value;

            if (_values.TryGetValue(key, out value))
                return value;

            return null;
        }

        protected virtual void Write(string key, string value)
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

        public virtual void Save()
        {
            if (_isDirty)
            {
                OperationAttempter.Attempt(() => SaveSettings(_path, _values));
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
