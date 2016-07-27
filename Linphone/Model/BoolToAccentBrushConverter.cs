/*
BoolToAccentBrushConverter.cs
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Linphone.Model
{
    /// <summary>
    /// Converter returning the AccentColorBrush if the boolean is true, else returning a title color.
    /// </summary>
    public class BoolToAccentBrushConverter : IValueConverter
    {
        /// <returns>A SolidColorBrush (PhoneAccentBrush or PhoneSubtleBrush).</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
            }
            else
            {
                return new SolidColorBrush { Color = Colors.Gray };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
