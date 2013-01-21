using Microsoft.Phone;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    /// <summary>
    /// Converter to get contacts' images without having to call Contact object methods
    /// </summary>
    public class ContactPictureConverter : System.Windows.Data.IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
