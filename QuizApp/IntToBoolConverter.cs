using System.Globalization;
using System.Windows.Data;

namespace QuizApp;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue && parameter is string paramString)
        {
            int paramValue;
            if (int.TryParse(paramString, out paramValue))
            {
                return intValue == paramValue;
            }
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue)
        {
            if (int.TryParse(parameter as string, out int index))
            {
                return index;
            }
        }
        return Binding.DoNothing;
    }

}