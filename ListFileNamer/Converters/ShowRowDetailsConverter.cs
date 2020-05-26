using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ListFileNamer.Converters
{
    /// <summary>
    /// Отображение детализации строки в DataGrid.
    /// </summary>
    public class ShowRowDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isChecked = (bool)value;
            if (isChecked)
                return "VisibleWhenSelected";
            else
                return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
             DependencyProperty.UnsetValue;
    }
}
