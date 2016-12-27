/*
BasePage.cs
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

using Linphone.Controls;
using Linphone.Model;
using System;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone {
    /// <summary>
    /// Base Phone Application Page for every Page in the application that need to be call event aware.
    /// </summary>
    public class BasePage : Page {
        /// <summary>
        /// Public constructor.
        /// </summary>
        protected BasePage()
            : this(new BaseModel()) {

        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        protected BasePage(BaseModel viewModel) {
            this.ViewModel = viewModel;
            this.ViewModel.Page = this;
            this.Loaded += BasePage_Loaded;
        }

        /// <summary>
        /// The status bar displayed on the page if present.
        /// </summary>
        public static StatusBar StatusBar {
            get; set;
        }

        /// <summary>
        /// View model linked to the page, implements the call state listener.
        /// </summary>
        protected readonly BaseModel ViewModel;

        /// <summary>
        /// Bind the view model to the page when it's been loaded.
        /// </summary>
        protected virtual void BasePage_Loaded(object sender, RoutedEventArgs e) {
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Forwards the OnNavigatedTo event to the model
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs nee) {
            base.OnNavigatedTo(nee);

            this.ViewModel.OnNavigatedTo(nee);
        }

        /// <summary>
        /// Forwards the OnNavigatedFrom event to the model
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee) {
            base.OnNavigatedFrom(nee);
            StatusBar = null;

            this.ViewModel.OnNavigatedFrom(nee);
        }
    }
}
