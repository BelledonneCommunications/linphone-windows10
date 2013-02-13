using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

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
        /// Public constructor.
        /// </summary>
        public ContactAction()
        {
            InitializeComponent();
        }
    }
}
