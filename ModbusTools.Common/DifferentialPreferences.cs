using System.Collections.Generic;
using System.Linq;

namespace ModbusTools.Common
{
    public class DifferentialPreferences : Preferences
    {
        private readonly Dictionary<string, string> _changes = new Dictionary<string, string>();

        public DifferentialPreferences(string path) : base(path)
        {

        }

        protected override void Write(string key, string value)
        {
            base.Write(key, value);

            // Keep track of all the changes that are made
            _changes[key] = value;
        }

        protected override bool IsDirty
        {
            get { return _changes.Any(); }
        }

        public override void Save()
        {
            if (IsDirty)
            {
                //Load up the existing settings
                var settings = LoadSavedSettings(_path);

                //Process the changes
                foreach (var change in _changes)
                {
                    //Apply each change to the settings
                    settings[change.Key] = change.Value;
                }

                //Save the merged settings
                SaveSettings(_path, settings);

                //We've applied our changes. No need to do them again.
                _changes.Clear();
            }
        }
    }
}
