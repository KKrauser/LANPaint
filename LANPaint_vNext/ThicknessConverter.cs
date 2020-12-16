﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace LANPaint_vNext
{
    [ValueConversion(typeof(string), typeof(double))]
    class ThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? 0 : double.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
