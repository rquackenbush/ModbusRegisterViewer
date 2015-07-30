using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                var viewModel = new SlaveViewModel();

                viewModel.AddRange(new RegisterRangeViewModel()
                    {
                        Name = "Register Range 1"
                    });


                return viewModel;
            }
        }

        public static RegisterRangeEditorViewModel RegisterRangeEditor
        {
            get 
            {  

                var viewModel = new RegisterRangeEditorViewModel()
                {
                    Name = "Design Time Editor"
                };

                viewModel.Fields.Add(
                    new EditFieldViewModel()
                    {
                        Name = "Test 1"
                    });

                viewModel.Fields.Add(
                    new EditFieldViewModel()
                    {
                        Name = "Test 2"
                    });

                return viewModel;
            }
        }
    }
}
