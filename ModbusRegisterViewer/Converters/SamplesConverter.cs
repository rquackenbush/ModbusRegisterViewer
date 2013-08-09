using System.Windows.Markup;
using ModbusRegisterViewer.Model;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using ModbusRegisterViewer.ViewModel;

namespace ModbusRegisterViewer.Converters
{
    public class SamplesConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var samples = value as IEnumerable<Sample>;

            if (samples == null)
                return null;

            Sample previousSample = null;

            var viewModels = new List<SampleViewModel>();

            foreach (var sample in samples)
            {
                viewModels.Add(new SampleViewModel(sample, previousSample));

                previousSample = sample;
            }

            return viewModels;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static SamplesConverter _converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null) _converter = new SamplesConverter();
            return _converter;
        }

    }
}
