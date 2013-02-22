using Linphone.Controls;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Linphone
{
    /// <summary>
    /// Base Phone Application Page for every Page in the application that need to be call event aware.
    /// </summary>
    public class BasePage: PhoneApplicationPage
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        protected BasePage()
            : this(new BaseModel())
        {

        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        protected BasePage(BaseModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.Page = this;
            this.Loaded += BasePage_Loaded;
        }

        /// <summary>
        /// The status bar displayed on the page if present.
        /// </summary>
        public static StatusBar StatusBar { get; set; }

        /// <summary>
        /// View model linked to the page, implements the call state listener.
        /// </summary>
        protected readonly BaseModel ViewModel;

        /// <summary>
        /// Bind the view model to the page when it's been loaded.
        /// </summary>
        protected virtual void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Forwards the OnNavigatedTo event to the model
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);

            this.ViewModel.OnNavigatedTo(nee);
        }

        /// <summary>
        /// Forwards the OnNavigatedFrom event to the model
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
            StatusBar = null;

            this.ViewModel.OnNavigatedFrom(nee);
        } 
    }
}
