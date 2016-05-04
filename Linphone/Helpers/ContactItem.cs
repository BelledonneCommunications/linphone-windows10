/*
ContactItem.xaml.cs
Copyright (C) 2016  Belledonne Communications, Grenoble, France
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
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Helpers
{
    public class ContactItem
    {
        private string contactId;
        private string contactName;
        private ImageSource contactImage;
        private string contactNumber;
        private string contactEmail;

        public ContactItem(string ContactId, string ContactName)
        {
            this.contactId = ContactId;
            this.contactName = ContactName;
        }

        public async void SetImageAsync(IRandomAccessStreamReference ThumbnailReference)
        {
            var thumbnailImage = new BitmapImage();
            if (ThumbnailReference != null)
            {
                using (IRandomAccessStreamWithContentType thumbnailStream = await ThumbnailReference.OpenReadAsync())
                {
                    thumbnailImage.SetSource(thumbnailStream);
                }
            }
            else
            {
                thumbnailImage = new BitmapImage(new Uri("ms-appx:///Assets/avatar.png", UriKind.Absolute));
            }
            this.contactImage = thumbnailImage;
        }

        public string ContactId
        {
            get
            {
                return contactId;
            }
        }

        public string ContactName
        {
            get
            {
                return contactName;
            }
        }

        public ImageSource ContactImage
        {
            get
            {
                return contactImage;
            }
        }
    }
}
