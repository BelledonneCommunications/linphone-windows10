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
    public partial class ContactAction : UserControl
    {
        private String _action;
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

        private String _phone;
        public String Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                phone.Text = value;
            }
        }

        public ContactAction()
        {
            InitializeComponent();
        }
    }
}
