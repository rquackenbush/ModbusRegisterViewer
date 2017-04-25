//namespace ModbusTools.SlaveSimulator.Model
//{
//    public static class IRegisterStorageExtensions
//    {
//        public static void WriteSingle<TPoint>(this IPointStorage<TPoint> registerStorage, ushort address, TPoint value)
//        {
//            registerStorage.Write(address,
//                new []
//                {
//                    value
//                });
//        }

//        public static TPoint ReadSingle<TPoint>(this IPointStorage<TPoint> registerStorage, ushort address)
//        {
//            return registerStorage.Read(address, 1)[0];
//        }
//    }
//}
