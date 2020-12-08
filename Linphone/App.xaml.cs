/*
App.xaml.cs
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

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Linphone;
using Linphone.Model;
using System.Diagnostics;
using Windows.UI.Core;
using System.Collections.Generic;

namespace Linphone
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application, CallControllerListener
    {
        Frame rootFrame;
        bool acceptCall;
        String sipAddress;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
            this.Suspending += OnSuspending;
            SettingsManager.InstallConfigFile();
            acceptCall = false;
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private void Back_requested(object sender, BackRequestedEventArgs e)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }    
        }

        public void CallEnded(Call call)
        {
            Debug.WriteLine("[CallListener] Call ended, can go back ? " + rootFrame.CanGoBack);
            

            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
            else
            {
                // Launch the Dialer and remove the incall view from the backstack
                rootFrame.Navigate(typeof(Views.Dialer), null);
                if (rootFrame.BackStack.Count > 0)
                {
                    rootFrame.BackStack.Clear();
                }
            }
        }

        public void CallUpdatedByRemote(Call call, bool isVideoAdded)
        {
            //throw new NotImplementedException();
        }

        public void MuteStateChanged(bool isMicMuted)
        {
            //throw new NotImplementedException();
        }

        public void NewCallStarted(string callerNumber)
        {
            Debug.WriteLine("[CallListener] NewCallStarted " + callerNumber);
            List<String> parameters = new List<String>();
            parameters.Add(callerNumber);
            rootFrame.Navigate(typeof(Views.InCall), parameters);
        }

        public void PauseStateChanged(Call call, bool isCallPaused, bool isCallPausedByRemote)
        {
            Debug.WriteLine("Pausestatechanged");
           // if (this.PauseListener != null)
           //     this.PauseListener.PauseStateChanged(call, isCallPaused, isCallPausedByRemote);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Initialize(e, null);
        }

        private void Initialize(IActivatedEventArgs e, String args)
        {       

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            //Start linphone
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            LinphoneManager.Instance.InitLinphoneCore();
            LinphoneManager.Instance.CallListener = this;
            LinphoneManager.Instance.CoreDispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().CoreWindow.Dispatcher;
            

            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += Back_requested;

            if (rootFrame.Content == null)
            {
                if (args != null)
                {
                    if (args.StartsWith("chat"))
                    {
                        var sipAddr = args.Split('=')[1];
                        rootFrame.Navigate(typeof(Views.Chat), sipAddr);
                    }
                    else
                    {                    
                        if (args.StartsWith("answer"))
                        {
                            acceptCall = true;
                            sipAddress = args.Split('=')[1];
                        }
                        rootFrame.Navigate(typeof(Views.Dialer), null);
                    }   
                }
                else
                {
                    rootFrame.Navigate(typeof(Views.Dialer), args);
                }
            }
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ToastNotification)
            {
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                var arguments = toastArgs.Argument;
                Initialize(args, arguments);
            }

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        public void CallIncoming(Call call)
        {
            if(acceptCall)
            {
                if (sipAddress != "")
                {
                    Address addr = LinphoneManager.Instance.Core.InterpretUrl(sipAddress);
                    if (addr != null && addr.AsStringUriOnly().Equals(call.RemoteAddress.AsStringUriOnly()))
                    {
                        call.Accept();
                        List<String> parameters = new List<String>();
                        parameters.Add(call.RemoteAddress.AsString());
                        rootFrame.Navigate(typeof(Views.InCall), parameters);
                        acceptCall = false;
                    }
                }
            } else
            {
                rootFrame.Navigate(typeof(Views.IncomingCall), call.RemoteAddress.AsString());
            }
        }
    }
}
