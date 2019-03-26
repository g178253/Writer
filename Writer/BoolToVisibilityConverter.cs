using System;
using Windows.UI.Xaml.Data;

namespace Writer
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(bool)))
            {
                throw new ArgumentException("Only Boolean is supported");
            }
            if (targetType.Equals(typeof(Windows.UI.Xaml.Visibility)))
            {
                return ((bool)value) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                throw new ArgumentException("Unsuported type {0}", targetType.FullName);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
