using System;
using System.Globalization;
using System.Windows.Data;

namespace Goldpoint_Inventory_System 
{
    class ExpanderToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }
}
