using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System.Linq;

namespace ListFileNamer.Converters
{
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value as IEnumerable<string>).Select(x => Path.GetFileName(x));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
             DependencyProperty.UnsetValue;

    }
}
