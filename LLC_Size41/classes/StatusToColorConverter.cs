using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LLC_Size41.classes
{
    public class StatusToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Завершен" ? 
                new SolidColorBrush(Colors.LightGreen)
                : new SolidColorBrush(Colors.LightSteelBlue);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}