using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom TextBox to look like a chat bubble.
    /// </summary>
    public partial class TextBoxChatBubble : UserControl
    {
        /// <summary>
        /// Public accessor to this control text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public TextBoxChatBubble()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Empty the content of the textbox.
        /// </summary>
        public void Reset()
        {
            Message.Text = "";
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Message.Text;
        }
    }
}
