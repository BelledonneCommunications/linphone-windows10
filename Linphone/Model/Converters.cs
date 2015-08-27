/*
Converters.cs
Copyright(C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Model
{
    class StringLengthToBooleanConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(String)))
            {
                throw new ArgumentException("Only String is supported");
            }
            if (targetType.Equals(typeof(Boolean)))
            {
                int len = (value as String).Length;
                if (len == 0) return false;
                return true;
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported type {0}", targetType.FullName));
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class BooleanToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(Boolean)))
            {
                throw new ArgumentException("Only Boolean is supported");
            }
            if (targetType.Equals(typeof(Visibility)))
            {
                Boolean b = (Boolean)value;
                if (b) return Visibility.Visible;
                return Visibility.Collapsed;
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported type {0}", targetType.FullName));
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class HistoryEntryStateToImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(HistoryEntryState)))
            {
                throw new ArgumentException("Only HistoryEntryState is supported");
            }
            if (targetType.Equals(typeof(ImageSource)))
            {
                switch ((HistoryEntryState)value)
                {
                    case HistoryEntryState.Incoming:
                        return new BitmapImage(new Uri("ms-appx:///Assets/Images/call_status_incoming.png"));
                    case HistoryEntryState.Missed:
                        return new BitmapImage(new Uri("ms-appx:///Assets/Images/call_status_missed.png"));
                    case HistoryEntryState.Outgoing:
                    default:
                        return new BitmapImage(new Uri("ms-appx:///Assets/Images/call_status_outgoing.png"));
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported type {0}", targetType.FullName));
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class CapsLockTextConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(String)))
            {
                throw new ArgumentException("Only String is supported");
            }
            if (targetType.Equals(typeof(String)))
            {
                return (value as String).ToUpper();
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported type {0}", targetType.FullName));
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
