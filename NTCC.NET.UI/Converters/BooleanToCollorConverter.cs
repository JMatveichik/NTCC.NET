using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NTCC.NET.UI.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {

        public SolidColorBrush TrueColor
        {
            get;
            set;
        } = Brushes.DarkGreen;

        public SolidColorBrush FalseColor
        {
            get;
            set;
        } = Brushes.DarkRed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueColor : FalseColor;
            }

            return Brushes.Transparent; // или другой цвет по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
