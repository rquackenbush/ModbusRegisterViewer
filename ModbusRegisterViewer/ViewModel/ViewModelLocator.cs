/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ModbusRegisterViewer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ModbusRegisterViewer.Services;

namespace ModbusRegisterViewer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}


            //if (!SimpleIoc.Default.IsRegistered<RegisterViewerViewModel>())
            //{
            //    SimpleIoc.Default.Register<RegisterViewerViewModel>(
            //        () => new RegisterViewerViewModel(new MessageBoxService()));
            //}

            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<SnifferViewModel>();
            SimpleIoc.Default.Register<ControlPanelDesignerViewModel>();
            SimpleIoc.Default.Register<SlaveSimulatorViewModel>();

            if (!SimpleIoc.Default.IsRegistered<IMessageBoxService>())
            {
                SimpleIoc.Default.Register<IMessageBoxService>(() => new MessageBoxService());
            }
        }

        public RegisterViewerViewModel RegisterViewer
        {
            get
            {
                return new RegisterViewerViewModel(new MessageBoxService());
            }
        }

        public AboutViewModel About
        {
            get { return ServiceLocator.Current.GetInstance<AboutViewModel>(); }
        }

        public SnifferViewModel Sniffer
        {
            get { return ServiceLocator.Current.GetInstance<SnifferViewModel>(); }
        }

        public ControlPanelDesignerViewModel ControlPanelDesigner
        {
            get { return ServiceLocator.Current.GetInstance<ControlPanelDesignerViewModel>(); }
        }

        public SlaveSimulatorViewModel SlaveSimulator
        {
            get { return ServiceLocator.Current.GetInstance<SlaveSimulatorViewModel>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}