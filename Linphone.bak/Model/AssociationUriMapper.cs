/*
AssociationUriMapper.cs
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
using System.Net;
using System.Windows.Navigation;

namespace Linphone.Model
{
    /// <summary>
    /// Class used to allow the app to be automatically launched when user click on a sip: or sips: link.
    /// </summary>
    class AssociationUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            string tempUri = HttpUtility.UrlDecode(uri.ToString());

            if (tempUri.StartsWith("/Protocol?encodedLaunchUri="))
            {
                tempUri = tempUri.Replace("/Protocol?encodedLaunchUri=", "");
                if (tempUri.StartsWith("sip:") || tempUri.StartsWith("sips:"))
                {
                    return new Uri("/Views/Dialer.xaml?sip=" + tempUri, UriKind.RelativeOrAbsolute);
                }
            }

            return uri;
        }
    }
}
