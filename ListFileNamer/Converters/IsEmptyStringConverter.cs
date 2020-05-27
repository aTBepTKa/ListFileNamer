using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ListFileNamer.Converters
{
    class IsEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            string.IsNullOrEmpty((string)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
             DependencyProperty.UnsetValue;
    }
}
