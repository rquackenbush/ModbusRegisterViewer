using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ModbusTools.Common;

namespace ModbusTools.CaptureViewer.Simple.ViewModel
{
    public class PacketViewModel : ViewModelBase
    {
        private readonly byte[] _message;
        private readonly string _problem;

        public PacketViewModel(byte[] message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Length == 0) throw new ArgumentException("message must have at least a single byte.", "message");

            _message = message;

            if (_message.Length < 4)
            {
                _problem = "Message is too short.";
            }
            else if (!message.DoesCrcMatch())
            {
                _problem = "CRC Mismatch";
            }
            else if ((_message[1] & 0x80) > 0)
            {
                _problem = "Error Response";
            }           
        }

        public byte SlaveId
        {
            get { return MessageUtilities.GetSlaveId(_message); }
        }

        public string Function
        {
            get
            {
                if (_message.Length < 2)
                    return null;

                var rawValue = (byte)(_message[1] & 0x7F);

                if (Enum.IsDefined(typeof (FunctionCode), rawValue))
                {
                    var enumValue = (FunctionCode) rawValue;

                    return enumValue.ToString();
                }

                return rawValue.ToString();
            }
        }

        public bool HasProblem
        {
            get { return !string.IsNullOrWhiteSpace(Problem); }
        }

        public string Problem
        {
            get { return _problem; }
        }

        public string Summary
        {
            get 
            {  
                var  hexStrings = _message.Select(b => b.ToString("X2"));

                return string.Join(" ", hexStrings);
            }
        }
    }
}
