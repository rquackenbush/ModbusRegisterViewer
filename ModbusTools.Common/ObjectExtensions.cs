namespace ModbusTools.Common
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns an array with the item as the only element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] ToSingletonArray<T>(this T item)
        {
            return new T[] { item };
        }
    }
}
