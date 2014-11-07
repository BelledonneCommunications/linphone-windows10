using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Linphone.Core;

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
            LinphoneAddress addr = LinphoneManager.Instance.LinphoneCore.InterpretURL(sipAddress);
            return addr.GetUserName();
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
