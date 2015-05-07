using System;
using System.Collections.Generic;
using ModbusTools.Common;
using ModbusTools.SlaveSimulator.Model.MessageFactories;

namespace ModbusTools.SlaveSimulator.Model
{
    public class Slave : ISlave
    {
        private readonly byte _slaveId;
        private readonly Dictionary<FunctionCode, IModbusFunctionHandler> _functionhandlers = new Dictionary<FunctionCode, IModbusFunctionHandler>();

        public Slave(byte slaveId, IEnumerable<IModbusFunctionHandler> functionHandlers)
        {
            if (functionHandlers == null) throw new ArgumentNullException("functionHandlers");
            if (slaveId == 0) throw new ArgumentOutOfRangeException("slaveId", "slaveId must be greater than zero.");

            foreach (var functionHandler in functionHandlers)
            {
                _functionhandlers.Add(functionHandler.FunctionCode, functionHandler);
            }

            _slaveId = slaveId;
        }

        public byte SlaveId
        {
            get { return _slaveId; }
        }

        private IModbusFunctionHandler GetFunctionHandler(FunctionCode functionCode)
        {
            IModbusFunctionHandler functionHandler;

            if (_functionhandlers.TryGetValue(functionCode, out functionHandler))
                return functionHandler;

            return null;
        }

        public byte[] ProcessRequest(byte[] request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (request.Length < 5) throw new ArgumentException("The request must be at least 5 bytes long", "request");

            var isBroadcast = MessageUtilities.GetSlaveId(request) == 0;


            //Get the function code
            var functionCode = MessageUtilities.GetFunction(request);

            //Get the handler
            var functionHandler = GetFunctionHandler(functionCode);

            //Check to see if we found a valid handler
            if (functionHandler == null)
            {
                //Don't return anything if this was a broadcast
                if (isBroadcast)
                    return null;

                //This wasn't a broadcast, so return an error.
                return ExceptionResponseFactory.Create(_slaveId, functionCode, ModbusExceptionCodes.FunctionCodeNotSupported);
            }

            //Verify that we can actually support a broadcast message
            if (isBroadcast && !functionHandler.SupportsBroadcast)
                return null;

            //Let the function handler deal with it.
            return functionHandler.ProcessRequest(SlaveId, request);
        }
    }
}
