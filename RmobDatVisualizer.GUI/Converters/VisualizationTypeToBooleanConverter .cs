using System;
using System.Globalization;
using System.Windows.Data;
using static RmobDatVisualizer.GUI.MainViewModel;

namespace RmobDatVisualizer.GUI
{
    public class VisualizationTypeToBooleanConverter : IValueConverter
    {
        public VisualizationType TargetType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is VisualizationType type && type == TargetType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
                return TargetType;

            return Binding.DoNothing;
        }
    }
}
