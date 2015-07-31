using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusTools.Common;
using ModbusTools.SlaveExplorer.Model;
using ModbusTools.SlaveExplorer.View;

namespace ModbusTools.SlaveExplorer.ViewModel
{
    internal static class ViewModelLocator
    {
        public static SlaveExplorerViewModel SlaveExplorer
        {
            get {  return new SlaveExplorerViewModel(); }
        }

        public static SlaveViewModel Slave
        {
            get
            {
                var slaveModel = new SlaveModel()
                {
                    Name = "RTULink",
                    SlaveId = 60,
                    Ranges = new RangeModel[]
                    {
                        new RangeModel()
                        {
                            Name = "Sys Info",
                            Fields = new FieldModel[]
                            {
                                new FieldModel()
                                {
                                    Name = "Field 1"
                                }
                                ,
                                new FieldModel()
                                {
                                    Name = "Field 2"
                                }
                            }
                        }
                    }
                };

                var viewModel = new SlaveViewModel(null, slaveModel, null);

                


                return viewModel;
            }
        }

        public static RegisterRangeViewModel RegisterRange
        {
            get
            {
                var rangeModel = new RangeModel()
                {
                    Name = "Sys Info",
                    Fields = new FieldModel[]
                    {
                        new FieldModel()
                        {
                            Name = "Field 1",
                            FieldType = FieldType.UINT16
                        }
                        ,
                        new FieldModel()
                        {
                            Name = "Field 2",
                            FieldType = FieldType.UINT16
                        }
                    }
                };
                

                return new RegisterRangeViewModel(null, rangeModel, null, null);
            }
        }

        public static RegisterRangeEditorViewModel RegisterRangeEditor
        {
            get
            {
                var rangeModel = new RangeModel()
                {
                    Name = "Test Range",
                    RegisterType = RegisterType.Holding,
                    Fields = new FieldModel[]
                    {
                        new FieldModel()
                        {
                            Name = "Field 1",
                            FieldType = FieldType.UINT16,
                        },
                        new FieldModel()
                        {
                            Name = "Field 2",
                            FieldType = FieldType.FLOAT32
                        }
                    }
                };

                var viewModel = new RegisterRangeEditorViewModel(rangeModel)
                {
                    Name = "Design Time Editor"
                };

                return viewModel;
            }
        }
    }
}
