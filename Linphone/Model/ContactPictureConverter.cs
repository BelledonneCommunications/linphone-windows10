/*
ContactPictureConverter.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.IO;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Model
{
    /// <summary>
    /// Converter to get contacts' images without having to call Contact object methods
    /// </summary>
    public class ContactPictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contact c = value as Contact;
            if (c == null)
                return null;

            var imgStream = c.Thumbnail; 
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Converter to get contacts' images without having to call Contact object methods
    /// </summary>
    public class ContactNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contact c = value as Contact;
            if (c == null)
                return null;

            return c.DisplayName.Replace(" ", "\n"); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
