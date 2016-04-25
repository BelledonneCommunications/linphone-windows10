/*
ContactAction.xaml.cs
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
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom user control representing a possible action on a phone number or email (in the Contact.xaml view).
    /// </summary>
    public partial class ContactAction : UserControl
    {
        private String _action;
        /// <summary>
        /// URI of an Image that represents the action.
        /// </summary>
        public String Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
                action2.Source = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); ;
            }
        }

        private String _action2;
        /// <summary>
        /// URI of an Image that represents the action.
        /// </summary>
        public String Action2
        {
            get
            {
                return _action2;
            }
            set
            {
                _action2 = value;
                action.Source = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); ;
            }
        }

        private String _label;
        /// <summary>
        /// Label of the phone number or the email displayed.
        /// </summary>
        public String Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                label.Text = value;
            }
        }

        private String _numberOrAddress;
        /// <summary>
        /// Phone number or email address to display.
        /// </summary>
        public String NumberOrAddress
        {
            get
            {
                return _numberOrAddress;
            }
            set
            {
                _numberOrAddress = value;
                phone.Text = value;
            }
        }

        /// <summary>
        /// Event triggered when action image is clicked.
        /// </summary>
        public RoutedEventHandler Click
        {
            set 
            { 
                button.Click += value;
                button.Tag = NumberOrAddress;
            }
            get { return null; }
        }

        /// <summary>
        /// Event triggered when action image is clicked.
        /// </summary>
        public RoutedEventHandler Click2
        {
            set
            {
                button2.Click += value;
                button2.Tag = NumberOrAddress;
            }
            get { return null; }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactAction()
        {
            InitializeComponent();
        }
    }
}
