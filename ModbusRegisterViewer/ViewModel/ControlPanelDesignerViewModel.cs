using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ModbusRegisterViewer.Model;

namespace ModbusRegisterViewer.ViewModel
{
    public class ControlPanelDesignerViewModel : ViewModelBase
    {

        public ControlPanelDesignerViewModel()
        {
            this.ControlPanel = new ControlPanelViewModel();

            var range1 = new RangeViewModel()
                {
                    Name =  "Inputs",
                    Type = RegisterType.Holding,
                    StartingAddress = 1000,
                    Number = 4
                };

            range1.Fields = new ObservableCollection<FieldViewModel>(new []
                {
                    new FieldViewModel(range1) { ByteAddress = 0, Name = "IN1: 0 to 10v"}, 
                    new FieldViewModel(range1) { ByteAddress = 2, Name = "Space Temperature"}, 
                    new FieldViewModel(range1) { ByteAddress = 2, Name = "Stat - Call For Heat"}, 
                });

            range1.Fields[0].Controls.Add(new ControlViewModel(){ Name = "Slider"});
            range1.Fields[1].Controls.Add(new ControlViewModel() { Name = "Slider" });
            range1.Fields[2].Controls.Add(new ControlViewModel() { Name = "Switch" });

            var range2 = new RangeViewModel()
                {
                    Name = "Outputs",
                    Type = RegisterType.Input,
                    StartingAddress = 2000,
                    Number = 5
                };

            range2.Fields = new ObservableCollection<FieldViewModel>(new []
                {
                    new FieldViewModel(range1) { ByteAddress = 0, Name = "OUT1: 0 to 10v" }, 
                    new FieldViewModel(range1) { ByteAddress = 2, Name = "Space Temperature"}, 
                });

            range2.Fields[0].Controls.Add(new ControlViewModel() { Name = "Slider" });
            range2.Fields[1].Controls.Add(new ControlViewModel() { Name = "Light" });

            this.ControlPanel = new ControlPanelViewModel()
                {
                    Ranges = new ObservableCollection<RangeViewModel>
                        {
                            range1,
                            range2
                        }
                };

            
        }

        private ControlPanelViewModel _controlPanel;
        public ControlPanelViewModel ControlPanel
        {
            get { return _controlPanel; }
            set
            {
                _controlPanel = value;
                RaisePropertyChanged(() => ControlPanel);
            }

        }


    }
}
