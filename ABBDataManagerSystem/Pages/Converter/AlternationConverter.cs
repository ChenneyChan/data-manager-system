using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ABBDataManagerSystem.Pages.Converter
{
    public class AlternationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            return index % 2 == 0 ? Brushes.LightBlue : Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
