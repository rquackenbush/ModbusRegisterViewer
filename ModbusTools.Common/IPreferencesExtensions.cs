using System;
using System.Xml;

namespace ModbusTools.Common
{
    public static class IPreferencesExtensions
    {
        public static int GetInt32(this IPreferences settings, string key, int defaultValue = 0)
        {
            int parsed;

            if (int.TryParse(settings[key], out parsed))
                return parsed;

            return defaultValue;
        }

        public static byte GetByte(this IPreferences settings, string key, byte defaultValue = 0)
        {
            byte parsed;

            if (byte.TryParse(settings[key], out parsed))
                return parsed;

            return defaultValue;
        }

        public static UInt16 GetUInt16(this IPreferences settings, string key, UInt16 defaultValue = 0)
        {
            UInt16 parsed;

            if (UInt16.TryParse(settings[key], out parsed))
                return parsed;

            return defaultValue;
        }

        public static string GetString(this IPreferences settings, string key, string defaultValue = null)
        {
            string value = settings[key];

            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return defaultValue;
        }

        public static void WriteSetting(this IPreferences settings, string key, int value)
        {
            settings[key] = value.ToString();
        }

        public static void WriteSetting(this IPreferences settings, string key, bool value)
        {
            settings[key] = value.ToString();
        }

        public static void WriteSetting(this IPreferences settings, string key, UInt16 value)
        {
            settings[key] = value.ToString();
        }

        public static void WriteSetting(this IPreferences settings, string key, byte value)
        {
            settings[key] = value.ToString();
        }

        public static bool GetBoolean(this IPreferences settings, string key, bool defaultValue = false)
        {
            bool parsed;

            if (bool.TryParse(settings[key], out parsed))
                return parsed;

            return defaultValue;
        }

        public static void SetBoolean(this IPreferences settings, string key, bool value)
        {
            settings[key] = value.ToString();
        }
    }
}
