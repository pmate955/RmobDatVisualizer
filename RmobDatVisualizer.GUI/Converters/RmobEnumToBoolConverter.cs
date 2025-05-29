using System;
using System.Globalization;
using System.Windows.Data;

namespace RmobDatVisualizer.GUI
{
    public class RmobEnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string enumString && value != null)
            {
                var enumValue = Enum.Parse(value.GetType(), enumString);
                return enumValue.Equals(value);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string enumString && (bool)value)
            {
                return Enum.Parse(targetType, enumString);
            }
            return Binding.DoNothing;
        }
    }
}
