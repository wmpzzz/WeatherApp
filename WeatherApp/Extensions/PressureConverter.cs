using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Extensions
{
    public class PressureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double hpa || value is float hpaFloat && (hpa = hpaFloat) != 0 || int.TryParse(value?.ToString(), out int hpaInt) && (hpa = hpaInt) != 0)
            {
                return Math.Round(hpa * 0.750062, 1);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}

