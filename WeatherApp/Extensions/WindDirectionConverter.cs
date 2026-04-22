using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Extensions
{
    public class WindDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double deg || value is float degFloat && (deg = degFloat) >= 0)
            {
                string[] directions = { "Северный", "Северо-восточный", "Восточный", "Юго-восточный",
                                        "Южный", "Юго-западный", "Западный", "Северо-западный" };
                // Сдвиг на 22.5 градуса, чтобы "Север" охватывал диапазон вокруг 0
                int index = (int)((double)deg + 22.5) / 45 % 8;
                return directions[index];
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    }
}
