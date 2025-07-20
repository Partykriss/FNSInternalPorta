using System.Globalization;
using System.Windows.Data;

namespace FNS.Admin.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                string timeFormat = dateTime.ToString("HH:mm");
                string dateFormat = dateTime.ToString("dd/MM/yy");
                return $"{timeFormat} {dateFormat}";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}