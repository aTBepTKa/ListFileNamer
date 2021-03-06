﻿using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace ListFileNamer.Converters
{
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Path.GetFileName((string)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
             DependencyProperty.UnsetValue;

    }
}
