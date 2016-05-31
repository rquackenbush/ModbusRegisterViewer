using System.Linq;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.CaptureViewer.Interpreted.ViewModel
{
    public class RegistersViewModel
    {
        private readonly RegisterViewModel[] _registers;
        private readonly string _hex;

        public RegistersViewModel(ushort[] registers, ushort startingRegisterIndex = 0)
        {
            ushort registerIndex = startingRegisterIndex;

            _registers = registers
                .Select(r => new RegisterViewModel(registerIndex++, r))
                .ToArray();

            var bytes = registers
                .SelectMany(r => new byte[]
                {
                    r.GetMSB(),
                    r.GetLSB()
                })
                .ToArray();

            _hex = bytes.FormatHex();
        }

        public RegisterViewModel[] Registers
        {
            get { return _registers; }
        }

        public string Hex
        {
            get { return _hex; }
        }
    }
}