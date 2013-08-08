using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using Modbus.IO;
using log4net;

namespace Modbus.Extended
{
    public abstract class ModbusPromiscuousSlave : ModbusDevice
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ModbusSlave));

        internal ModbusPromiscuousSlave(ModbusTransport transport)
			: base(transport)
		{
		    	
		}

        /// <summary>
        /// Occurs when a modbus slave receives a request.
        /// </summary>
        public event EventHandler<ModbusSlaveRequestEventArgs> ModbusSlaveRequestReceived;

        /// <summary>
        /// Start slave listening for requests.
        /// </summary>
        public abstract void Listen();
    }
}
