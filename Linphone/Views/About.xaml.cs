using Linphone.Resources;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Documents;

namespace Linphone.Views
{
    /// <summary>
    /// Page used to display message about the app.
    /// </summary>
    public partial class About : BasePage
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public About()
        {
            InitializeComponent();
            ResourceManager resourceManager = new ResourceManager("Linphone.Resources.AppResources", typeof(AppResources).Assembly);
            string versionName = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString();

            int i = 0;
            string text = resourceManager.GetString("AboutText", CultureInfo.CurrentCulture);
            Paragraph paragraph = new Paragraph();

            // Parse the text to insert clickable links when a line start with http://
            // And replace #version# tag by actual version name
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