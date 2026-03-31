using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calculator.ViewModels
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Если параметр "Collapsed", инвертируем логику
                if (parameter?.ToString() == "Collapsed")
                    boolValue = !boolValue;
                    
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}