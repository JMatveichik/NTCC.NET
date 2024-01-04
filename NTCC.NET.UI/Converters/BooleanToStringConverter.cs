using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NTCC.NET.UI.Converters
{
    public class BooleanToStringConverter : IValueConverter
    {
        public string TrueString
        {
            get;
            set;
        } = "ON";

        public string FalseString
        {
            get;
            set;
        } = "OFF";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueString : FalseString;
            }

            return ""; // или другой цвет по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
