using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using Modbus.IO;
using Modbus.Utility;
using log4net;

namespace Modbus.Extended
{
    public class ModbusPromiscuousSerialSlave : ModbusPromiscuousSlave
    {
        public event EventHandler<RawSlaveData> MessageSniffed;
        public event EventHandler<InvalidData> InvalidMessage;

        private static readonly ILog _logger = LogManager.GetLogger(typeof(ModbusSerialSlave));

        private ModbusPromiscuousSerialSlave(ModbusTransport transport)
            : base(transport)
        {
        }

        private ModbusSerialTransport SerialTransport
        {
            get
            {
                var transport = Transport as ModbusSerialTransport;
                if (transport == null)
                    throw new ObjectDisposedException("SerialTransport");

                return transport;
            }
        }

        /// <summary>
        /// Modbus RTU slave factory method.
        /// </summary>
        public static ModbusPromiscuousSerialSlave CreateRtu(IStreamResource streamResource)
        {
            if (streamResource == null)
                throw new ArgumentNullException("streamResource");

            return new ModbusPromiscuousSerialSlave(new ModbusRtuTransport(streamResource));
        }

        /// <summary>
        /// Start slave listening for requests.
        /// </summary>
        public override void Listen()
        {

            var buffer = new byte[256];
            int bufferPosition = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            const int minimumFrameSize = 5;

            while (true)
            {
                try
                {
                    try
                    {
                        //Get a single byte
                        byte singleByte = SerialTransport.StreamResource.ReadSingleByte();

                        var elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

                        if (bufferPosition > buffer.Length - 1)
                        {
                            const string message = "Message size exceeded";

                            this.InvalidMessage.Raise(this, new InvalidData(buffer.Take(bufferPosition).ToArray(), message));

                            bufferPosition = 0;
                        }
                        else if (elapsedMilliseconds > 45)
                        {
                            string message = string.Format("Timed out: {0}ms", elapsedMilliseconds);

                            this.InvalidMessage.Raise(this, new InvalidData(buffer.Take(bufferPosition).ToArray(), message));

                            bufferPosition = 0;
                        }

                        //Reset the stopwatch
                        stopwatch.Reset();

                        //Copy the single buffer
                        buffer[bufferPosition] = singleByte;

                        //Check to see if we have the minimum size
                        if (bufferPosition + 1 >= minimumFrameSize)
                        {
                            //Get the message part
                            var messageFrame = buffer.Take(bufferPosition - 1).ToArray();

                            //Calculate the CRC with the given set of bytes
                            var calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(messageFrame), 0);

                            //Get the crc that is stored in the message
                            var messageCrc = BitConverter.ToUInt16(buffer, bufferPosition - 1);

                            //if they match, we'll call this a message.
                            if (calculatedCrc == messageCrc)
                            {
                                //Create the message
                                var message = new byte[bufferPosition + 1];

                                //Copy the data to it
                                Buffer.BlockCopy(buffer, 0, message, 0, bufferPosition + 1);

                                //This is the real deal
                                this.MessageSniffed.Raise(this, new RawSlaveData(message));

                                Console.WriteLine();

                                bufferPosition = 0;
                            }
                            else
                            {
                                bufferPosition++;
                            }
                        }
                        else
                        {
                            bufferPosition++;
                        }
                    }

                    catch (IOException ioe)
                    {
                        _logger.ErrorFormat("IO Exception encountered while listening for requests - {0}", ioe.Message);
                        SerialTransport.DiscardInBuffer();
                    }
                    catch (TimeoutException te)
                    {
                        _logger.ErrorFormat("Timeout Exception encountered while listening for requests - {0}",
                                            te.Message);
                        SerialTransport.DiscardInBuffer();
                    }
                }
                catch (InvalidOperationException)
                {
                    // when the underlying transport is disposed
                    break;
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("Something horrible happened. {0}", ex.Message);
                }
            }
        }

    }
}
