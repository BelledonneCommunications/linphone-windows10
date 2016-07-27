/*
About.xaml.cs
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

using Linphone.Model;
using System.Reflection;
using Windows.UI.Xaml.Controls;

namespace Linphone.Views
{

    public partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();

            var packageId = Windows.ApplicationModel.Package.Current.Id;
            AppVersion.Text = string.Format("{0}.{1}.{2}", packageId.Version.Major, packageId.Version.Minor, packageId.Version.Build);
            CoreVersion.Text = LinphoneManager.Instance.getCoreVersion();

            // Parse the text to insert clickable links when a line start with http://
            // And replace #version# tag by actual version name
            /* foreach (var line in text.Split('\n'))
             {
                 String textLine = line;
                 if (line.Contains("#version#"))
                 {
                     textLine = line.Replace("#version#", versionName);
                 }

                 if (line.StartsWith("http://"))
                 {
                     Hyperlink link = new Hyperlink();
                     link.NavigateUri = new Uri(textLine);
                     link.Inlines.Add(textLine);
                     link.TargetName = "_blank";

                     paragraph.Inlines.Add(link);
                     i++;
                 }
                 else
                 {
                     paragraph.Inlines.Add(textLine);
                     paragraph.Inlines.Add(new LineBreak());
                 }
             }
             AboutText.Blocks.Add(paragraph);*/
        }
    }
}