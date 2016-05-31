using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Modbus.IO;
using ModbusTools.Common;
using System.Timers;

namespace ModbusTools.SlaveSimulator.Model
{
    /// <summary>
    /// This hosts one or more slave simulators.
    /// </summary>
    public class SlaveSimulatorHost : IDisposable
    {
        private readonly IMasterContext _masterContext;
        private readonly double _characterTimeout;
        private readonly double _messageTimeout;
        private readonly Dictionary<byte, ISlave> _slaves = new Dictionary<byte, ISlave>();

        private PromiscuousListener _listener;
        private IStreamResource _port;

        private readonly List<byte> _sampleBuffer = new List<byte>();

        private SlaveSimulatorState _state = SlaveSimulatorState.Idle;

        private readonly Timer _timer;

        public SlaveSimulatorHost(IMasterContext masterContext, 
            IEnumerable<ISlave> slaves, 
            double characterTimeout,
            double messageTimeout)
        {
            if (masterContext == null) throw new ArgumentNullException(nameof(masterContext));
            if (slaves == null) throw new ArgumentNullException(nameof(slaves));

            _masterContext = masterContext;
            _characterTimeout = characterTimeout;
            _messageTimeout = messageTimeout;

            foreach (var slave in slaves)
            {
                _slaves.Add(slave.SlaveId, slave);    
            }

            var task = new Task(() =>
            {
                try
                {
                    using (var master = _masterContext)
                    {
                        _port = master.Master.Transport.GetStreamResource();

                        _listener = new PromiscuousListener(_port);

                        _listener.Sample += OnSample;

                        _listener.Listen();
                    }
                }
                catch (Exception ex)
                {
                    //This will exception out when the port is killed
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    _listener.Sample -= OnSample;

                    _listener = null;
                }
            });

            //Create the timer
            _timer = new System.Timers.Timer
            {
                AutoReset = false
            
            };

            _timer.Elapsed += TimerElapsed;

            task.Start();
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                switch (_state)
                {
                    case SlaveSimulatorState.Idle:

                        StopTimer();
                        break;

                    case SlaveSimulatorState.ListeningToRequest:

                        StopTimer();

                        var request = _sampleBuffer.ToArray();

                        //Make sure there actually is a sample.
                        if (request.Length > 0)
                        {
                            //Get the slave id
                            var slaveId = request[0];

                            //Try to find the slave
                            var slave = GetSlave(slaveId);

                            //Make sure we found it (would be weird if we didn't)
                            if (slave != null)
                            {
                                if (CrcExtensions.DoesCrcMatch(request))
                                {
                                    //Create the response
                                    var response = slave.ProcessRequest(request);

                                    //Only write a response if this isn't a broadcast message
                                    if (slaveId == 0)
                                    {
                                        Console.WriteLine("This is a brodcast so we don't write anything back.");
                                    }
                                    else if (response == null)
                                    {
                                        Console.WriteLine("The response from the slave was null.");
                                    }
                                    else
                                    {
                                        //Write the response
                                        _port.Write(response, 0, response.Length);

                                        Console.WriteLine("Response: {0}", HexFormatter.FormatHex(response));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("CRC doesn't match.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Couldn't find slave {0}", slaveId);
                            }
                        }

                        TransitionToIdle();

                        break;

                    case SlaveSimulatorState.ListeningToOtherSlave:

                        TransitionToIdle();

                        break;


                    default:
                        throw new InvalidOperationException(string.Format("Unexpected state '{0}'", _state));
                }
            }
            catch (Exception ex)
            {
                TransitionToIdle();
                Console.WriteLine(ex.ToString());                
                
            }
        }

        private void TransitionToIdle()
        {
            StopTimer();
            _state = SlaveSimulatorState.Idle;

            Console.WriteLine(HexFormatter.FormatHex(_sampleBuffer.ToArray()));

            _sampleBuffer.Clear();
            Console.WriteLine("Back to idle");
        }

        private void StartTimer(double milliseconds)
        {
            _timer.Interval = milliseconds;
            _timer.Start();
            _timer.Enabled = true;
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Enabled = false;
        }

        void OnSample(object sender, SampleEventArgs e)
        {
            switch (_state)
            {
                case SlaveSimulatorState.Idle:

                    _sampleBuffer.Add(e.Sample);

                    if (DoesSlaveExist(e.Sample))
                    {
                        _state = SlaveSimulatorState.ListeningToRequest;

                        StartTimer(_characterTimeout);

                        Console.WriteLine("Starting to listen to our own slave request...");

                    }
                    else
                    {
                        _state = SlaveSimulatorState.ListeningToOtherSlave;
                        StartTimer(_messageTimeout);

                        Console.WriteLine("Starting to listen to someone else's request...");
                    }

                    break;

                case SlaveSimulatorState.ListeningToRequest:

                    _sampleBuffer.Add(e.Sample);

                    StartTimer(_characterTimeout);

                    break;

                case SlaveSimulatorState.ListeningToOtherSlave:

                    _sampleBuffer.Add(e.Sample);
                    StartTimer(_messageTimeout);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected state '{0}'", _state));

            }
        }

        private bool DoesSlaveExist(byte deviceAddress)
        {
            if (deviceAddress == 0)
                return true;

            return _slaves.ContainsKey(deviceAddress);
        }

        /// <summary>
        /// Gets a slave from its device address.
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <returns></returns>
        private ISlave GetSlave(byte deviceAddress)
        {
            ISlave slave;

            if (_slaves.TryGetValue(deviceAddress, out slave))
                return slave;

            return null;
        }

        public void Dispose()
        {
            if (_listener != null)
            {
                //When a message is received
                _listener.Sample -= OnSample;
                _listener.Dispose();
            }

            if (_port != null)
            {
                _port.Dispose();
            }
        }
    }
}
