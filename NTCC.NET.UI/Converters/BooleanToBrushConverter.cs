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
    public class BooleanToBrushConverter : IValueConverter
    {

        public Brush TrueBrush
        {
            get;
            set;
        }

        public Brush FalseBrush
        {
          get;
          set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueBrush : FalseBrush;
            }

            return Brushes.Transparent; // или другой цвет по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
