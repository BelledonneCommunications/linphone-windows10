using System;
using System.Windows.Data;

namespace ScreenSizeSupport.Misc
{
  public class DecimalRoundingConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double number;
      var requestedPlaces = parameter as string;

      if (!double.TryParse(value.ToString(), out number))
        return value;

      int decimalPlaces = 2;
      if (!string.IsNullOrEmpty(requestedPlaces))
        int.TryParse(requestedPlaces, out decimalPlaces);

      var decimalFormat = "";
      if (decimalPlaces >= 1)
      {
        decimalFormat = "0";
        for (int place = 1; place < decimalPlaces; place++)
          decimalFormat += "#";
      }

      return string.Format("{0:." + decimalFormat + "}", number);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
