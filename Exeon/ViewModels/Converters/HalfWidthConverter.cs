using Microsoft.UI.Xaml.Data;
using System;

namespace Exeon.ViewModels.Converters
{
    public class HalfWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double width)
            {
                return width / 2;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
