﻿using Newtonsoft.Json;

namespace ModbusTools.SlaveExplorer.Model
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T source)
            where T : class
        {
            var data = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
