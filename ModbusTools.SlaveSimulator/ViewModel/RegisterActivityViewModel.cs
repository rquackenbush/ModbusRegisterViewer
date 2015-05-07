using System;
using System.Linq;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    public class RegisterActivityViewModel : ActivityViewModel<ushort>
    {
        public RegisterActivityViewModel(DateTime timestamp, string operation, int startingIndex, ushort[] data, bool isZeroBased)
            : base(timestamp, operation, startingIndex, data, isZeroBased)
        {
        }

        public override string Summary
        {
            get
            {
                var hexNumbers = Data.Select(r => Convert.ToString(r, 16).PadLeft(4, '0')).ToArray();

                return string.Join(" ", hexNumbers);
            }
        }
    }
}
