using System;
using System.ComponentModel;
using System.Windows.Data;

namespace ScreenSizeSupport.Misc
{
  public class StringToScreenInfoConverter : TypeConverter, IValueConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
        return true;

      return false;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      return Convert(value, null, null, null);
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var str = value as string;
      if (str == null)
        throw new ArgumentOutOfRangeException("value", "value must be a string");

      if (string.IsNullOrWhiteSpace(str))
        return ScreenInfo.NullValue;

      var parts = str.Split(new[] { ',' });
      if (parts.Length != 3)
        throw new ArgumentOutOfRangeException("value", "value must be 'diagonal_inches,horizontal_res,aspect_ratio'");

      double diagonal = 0;
      if (!double.TryParse(parts[0], out diagonal))
        throw new ArgumentOutOfRangeException("value", "invalid screen diagonal");

      double width = 0;
      if (!double.TryParse(parts[1], out width))
        throw new ArgumentOutOfRangeException("value", "invalid screen width in pixels");

      double aspectRatio = 0;
      if (!double.TryParse(parts[2], out aspectRatio))
        throw new ArgumentOutOfRangeException("value", "invalid aspect ratio");

      return new ScreenInfo(diagonal, width, aspectRatio);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
