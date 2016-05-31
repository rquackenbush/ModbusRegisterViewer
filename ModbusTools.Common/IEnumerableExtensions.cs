using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModbusTools.Common
{
    public static class IEnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var collection = new ObservableCollection<T>();

            foreach (var item in items)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}
