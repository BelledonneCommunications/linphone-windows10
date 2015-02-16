/*
AddressBox.xaml.cs
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
using System.Windows.Controls;

namespace Linphone.Controls
{
    /// <summary>
    /// Listener to throw events depending on the focus state of this control.
    /// </summary>
    public interface AddressBoxFocused
    {
        /// <summary>
        /// Called when the addressbox get focused.
        /// </summary>
        void Focused();

        /// <summary>
        /// Called when the addressbox lost its focus.
        /// </summary>
        void UnFocused();
    }

    /// <summary>
    /// Custom control representing a textbox that contains a SIP address.
    /// </summary>
    public partial class AddressBox : UserControl
    {
        /// <summary>
        /// Listener to throw events depending on the focus state of this control.
        /// </summary>
        public AddressBoxFocused FocusListener;

        /// <summary>
        /// String content of the textbox.
        /// </summary>
        public String Text
        {
            get { return address.Text; }
            set { address.Text = value; }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AddressBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void backspace_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            address.Text = "";
        }

        private void backspace_Click_1(object sender, RoutedEventArgs e)
        {
            if (address.Text.Length > 0)
                address.Text = address.Text.Substring(0, address.Text.Length - 1);
        }

        private void address_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FocusListener != null)
                FocusListener.Focused();
        }

        private void address_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FocusListener != null)
                FocusListener.UnFocused();
        }
    }
}
