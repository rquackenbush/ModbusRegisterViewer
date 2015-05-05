using System.Linq;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.SlaveSimulator.ViewModel
{
    /// <summary>
    /// Represents some sort of activity.
    /// </summary>
    public class ActivityViewModel
    {
        public ActivityViewModel(FunctionCode functionCode, string summary, ushort? startingAddress = null, ushort[] registers = null)
        {
            FunctionCode = functionCode;
            Summary = summary;
            
            if (startingAddress != null && registers != null)
            {
                ushort currentAddress = startingAddress.Value;

                Registers = registers.Select(r => new RegisterViewModel(currentAddress++, r)).ToArray();
            }
        }

        public FunctionCode FunctionCode { get; private set; }

        public string Summary { get; private set; }

        public RegisterViewModel[] Registers { get; private set; }
    }
}
