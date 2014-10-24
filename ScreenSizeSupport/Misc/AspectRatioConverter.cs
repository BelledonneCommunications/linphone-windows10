using System;
using System.Windows.Data;

namespace ScreenSizeSupport.Misc
{
  public class AspectRatioConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (SizeHelpers.GetFriendlyAspectRatio((double)value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var str = (string)value;

      double aspectRatio = 0;
      if (double.TryParse(str, out aspectRatio))
        return aspectRatio;

      // look for friendly name
      return SizeHelpers.GetAspectRatioFromFriendlyName(str);
    }
  }
}
