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

        // This is needed for the textbox hack, see comment in .xaml file
        private void address_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            int defaultFontSize = 35;
            bool hasLongerText = address.Text.Length > behindAddress.Text.Length;
            behindAddress.Text = address.Text;

            if (behindAddress.Text.Length > 0)
            {
                int margin = 50; // Keep a margin
                var trailingEdge = behindAddress.GetRectFromCharacterIndex(behindAddress.Text.Length - 1, true);
                if (hasLongerText)
                {
                    while (behindAddress.ActualWidth < trailingEdge.Right + margin && behindAddress.FontSize > 1)
                    {
                        behindAddress.FontSize--;
                        trailingEdge = behindAddress.GetRectFromCharacterIndex(behindAddress.Text.Length - 1, true);
                    }
                }
                else
                {
                    while (trailingEdge.Right + margin <= behindAddress.ActualWidth && behindAddress.FontSize < defaultFontSize)
                    {
                        behindAddress.FontSize++;
                        trailingEdge = behindAddress.GetRectFromCharacterIndex(behindAddress.Text.Length - 1, true);
                    }
                }
            }
            else
            {
                behindAddress.FontSize = defaultFontSize;
            }
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
