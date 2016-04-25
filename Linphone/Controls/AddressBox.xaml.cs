/*
AddressBox.xaml.cs
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
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace Linphone.Controls
{

    public interface AddressBoxFocused
    {
        void Focused();

        void UnFocused();
    }

    public partial class AddressBox : UserControl
    {
        public AddressBoxFocused FocusListener;

        public String Text
        {
            get { return address.Text; }
            set { address.Text = value;  Debug.WriteLine("Set"); }
        }

        public AddressBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void backspace_Hold_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            address.Text = "";
        }

        private void backspace_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (address.Text.Length > 0)
                address.Text = address.Text.Substring(0, address.Text.Length - 1);
        }

        private void address_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (FocusListener != null)
                FocusListener.Focused();
        }

        private void address_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (FocusListener != null)
                FocusListener.UnFocused();
        }

        private void address_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(address.Text.Length > 0)
            {
                Backspace.IsEnabled = true;
            } else
            {
                Backspace.IsEnabled = false;
            }
        }
    }
}
