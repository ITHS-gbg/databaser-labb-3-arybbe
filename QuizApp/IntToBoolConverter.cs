using System.Globalization;
using System.Windows.Data;

namespace QuizApp;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue && parameter is string strParam && int.TryParse(strParam, out int paramValue))
        {
            return intValue == paramValue && intValue >= 0;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string strParam && int.TryParse(strParam, out int paramValue))
        {
            return boolValue ? paramValue : -1;
        }
        return -1;
    }
}