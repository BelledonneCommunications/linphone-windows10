/*
DialerHeader.xaml.cs
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Linphone.Controls
{
    public sealed partial class DialerHeader : UserControl
    {
        public DialerHeader()
        {
            this.InitializeComponent();
        }

        public void AppendText(String text)
        {
            this.DialerTextBox.Text += text;
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            var len = this.DialerTextBox.Text.Length;
            if (len > 0)
            {
                this.DialerTextBox.Text = this.DialerTextBox.Text.Remove(len - 1);
            }
        }

        private void BackspaceButton_Holding(object sender, HoldingRoutedEventArgs e)
        {
            this.DialerTextBox.Text = "";
        }
    }
}
