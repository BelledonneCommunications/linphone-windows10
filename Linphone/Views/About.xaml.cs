using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;
using Linphone.Resources;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Linphone.Views
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            ResourceManager resourceManager = new ResourceManager("Linphone.Resources.AppResources", typeof(AppResources).Assembly);
            string versionName = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString();

            int i = 0;
            string text = resourceManager.GetString("AboutText", CultureInfo.CurrentCulture);
            Paragraph paragraph = new Paragraph();
            foreach (var line in text.Split('\n'))
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
            AboutText.Blocks.Add(paragraph);
        }
    }
}