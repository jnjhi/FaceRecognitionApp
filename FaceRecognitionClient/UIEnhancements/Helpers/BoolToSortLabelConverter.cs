using System.Globalization;
using System.Windows.Data;

namespace FaceRecognitionClient.UIEnhancements.Helpers
{
    public class BoolToSortLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? "Sort ↓" : "Sort ↑";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
