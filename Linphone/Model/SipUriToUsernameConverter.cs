using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Linphone.Model
{
    /// <summary>
    /// Converter returning the AccentColorBrush if the boolean is true, else returning a title color.
    /// </summary>
    public class SipUriToUsernameConverter : IValueConverter
    {
        /// <returns>A SolidColorBrush (PhoneAccentBrush or PhoneSubtleBrush).</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sipAddress = (string)value;
            if (sipAddress.StartsWith("sip:"))
            {
                sipAddress = sipAddress.Replace("sip:", "");
            }
            if (sipAddress.Contains("@"))
            {
                sipAddress = sipAddress.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            return sipAddress;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
