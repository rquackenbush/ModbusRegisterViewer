using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Modbus;
using ModbusTools.Common;
using ModbusTools.Common.ViewModel;

namespace ModbusTools.Scanner.ViewModel
{
    public class SlaveScannerViewModel : ViewModelBase
    {
        private CancellationTokenSource _cts;
        private byte _startAddress = ModbusConstants.MinSlaveAddress;
        private byte _endAddress = ModbusConstants.MaxSlaveAddress;
        private readonly ObservableCollection<ResultViewModel> _results = new ObservableCollection<ResultViewModel>();
        private readonly ModbusAdaptersViewModel _modbusAdapters = new ModbusAdaptersViewModel();
        private byte _progressMax;
        private byte _progressValue;
        private bool _isRunning;
        private byte _currentSlaveAddress;

        public SlaveScannerViewModel()
        {
            ScanCommand = new RelayCommand(Scan, CanScan);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public ICommand ScanCommand { get; }
        public ICommand CancelCommand { get; }

        public byte StartAddress
        {
            get { return _startAddress; }
            set
            {
                _startAddress = value; 
                RaisePropertyChanged();
            }
        }

        public byte EndAddress
        {
            get { return _endAddress; }
            set
            {
                _endAddress = value; 
                RaisePropertyChanged();
            }
        }

        private void Scan()
        {
            _cts = new CancellationTokenSource();

            ScanAsync(_cts.Token);
        }

        private bool CanScan()
        {
            return _cts == null && ModbusAdapters.IsItemSelected && EndAddress >= StartAddress;
        }

        private async void ScanAsync(CancellationToken token)
        {
            try
            {
                IsRunning = true;

                _results.Clear();

                using (var master = ModbusAdapters.GetFactory().Create())
                {
                    try
                    {
                        var stream = master.Master.Transport.GetStreamResource();
                        
                        stream.DiscardInBuffer();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    ProgressValue = 0;
                    ProgressMax = (byte)(EndAddress - StartAddress);

                    for (byte slaveAddress = StartAddress; slaveAddress < EndAddress; slaveAddress++)
                    {
                        CurrentSlaveAddress = slaveAddress;

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        bool wasFound = false;
                        string reason;

                        try
                        {
                            await master.Master.ReadHoldingRegistersAsync(slaveAddress, 1000, 1);
                            reason = "Reading Holding Register succeeded.";
                            wasFound = true;
                        }
                        catch (TimeoutException ex)
                        {
                            reason = ex.Message;
                            //Not found
                        }
                        catch (SlaveException ex)
                        {
                            //We got sopme sort of slave exception.
                            reason = ex.Message;
                            wasFound = true;
                        }
                        catch (Exception ex)
                        {
                            reason = ex.Message;
                        }

                        //Create the result.
                        var result = new ResultViewModel()
                        {
                            SlaveAddress = slaveAddress,
                            WasFound = wasFound,
                            Reason = reason,
                        };

                        _results.Add(result);
                        
                        ProgressValue++;
                    }
                }
               
                if (_results.Count != 0)
                {
                    string message;

                    var successfulCount = _results.Count(r => r.WasFound);

                    if (successfulCount == 0)
                    {
                        message = "No Modbus slaves were found.";
                    }
                    else if (successfulCount == 1)
                    {
                        message = "1 Modbus slave was found.";
                    }
                    else
                    {
                        message = $"{successfulCount} Modbus slaves were found.";
                    }

                    MessageBox.Show(message, "Scan Result");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            IsRunning = false;

            _cts = null;

            CommandManager.InvalidateRequerySuggested();
        }

        private void Cancel()
        {
            _cts?.Cancel();
        }

        private bool CanCancel()
        {
            return _cts != null;
        }

        public IEnumerable<ResultViewModel> Results
        {
            get {  return _results;}
        }

        public ModbusAdaptersViewModel ModbusAdapters
        {
            get { return _modbusAdapters; }
        }

        public byte ProgressMax
        {
            get { return _progressMax; }
            private set
            {
                _progressMax = value; 
                RaisePropertyChanged();
            }
        }

        public byte ProgressValue
        {
            get { return _progressValue; }
            private set
            {
                _progressValue = value; 
                RaisePropertyChanged();
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value; 
                RaisePropertyChanged();
            }
        }

        public byte CurrentSlaveAddress
        {
            get { return _currentSlaveAddress; }
            private set
            {
                _currentSlaveAddress = value; 
                RaisePropertyChanged();
            }
        }
    }
}