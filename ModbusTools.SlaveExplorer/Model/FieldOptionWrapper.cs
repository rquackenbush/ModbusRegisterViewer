using System.Collections.Generic;
using System.Linq;

namespace ModbusTools.SlaveExplorer.Model
{
    public abstract class FieldOptionWrapper
    {
        private readonly Dictionary<string, string> _options = new Dictionary<string, string>();

        protected FieldOptionWrapper(IEnumerable<FieldOptionModel> options)
        {
            if (options != null)
            {
                foreach (var option in options)
                {
                    _options.Add(option.Key, option.Value);
                }
            }
        }

        protected int GetInt32(string key, int defaultValue)
        {
            string rawValue;

            if (!_options.TryGetValue(key, out rawValue))
                return defaultValue;

            int value;

            if (int.TryParse(rawValue, out value))
                return value;

            return defaultValue;
        }

        protected void SetInt32(string key, int value)
        {
            _options[key] = value.ToString();
        }

        protected string GetString(string key, string defaultValue)
        {
            string value;

            if (_options.TryGetValue(key, out value))
                return value;

            return defaultValue;
        }

        protected void SetString(string key, string value)
        {
            _options[key] = value;
        }

        public FieldOptionModel[] GetOptions()
        {
            return _options.Select(p => new FieldOptionModel()
            {
                Key = p.Key,
                Value = p.Value
            }).ToArray();
        }
    }
}
