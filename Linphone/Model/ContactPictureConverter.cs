using Microsoft.Phone;
using Microsoft.Phone.UserData;
using System;
using System.IO;

namespace Linphone.Model
{
    /// <summary>
    /// Converter to get contacts' images without having to call Contact object methods
    /// </summary>
    public class ContactPictureConverter : System.Windows.Data.IValueConverter
    {
        /// <summary>
        /// Converts the picture of a contact (an ImageStream) into an Image object.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Contact c = value as Contact;
            if (c == null)
                return null;

            Stream imgStream = c.GetPicture();
            if (imgStream != null)
                return PictureDecoder.DecodeJpeg(imgStream);

            return null;
        }

        /// <summary>
        /// Not implemented (not needed).
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
