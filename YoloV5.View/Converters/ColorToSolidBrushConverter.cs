using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace YoloV5.View.Converters
{
    public class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color color)
            {
                Color windowsColor = new Color
                {
                    A = color.A,
                    R = color.R,
                    G = color.G,
                    B = color.B
                };
                return new SolidColorBrush(windowsColor);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
