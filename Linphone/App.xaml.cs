using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Resources;
using System.Windows.Media;
using Linphone.Model;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Networking.Voip;

namespace Linphone
{
    /// <summary>
    /// Root frame for the application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary> 
        /// The push channel URI for this application 
        /// </summary> 
        public Uri PushChannelUri { get; private set; }

        /// <summary> 
        /// An event that is raised when the push channel URI changes 
        /// </summary> 
        public event EventHandler<Uri> PushChannelUriChanged; 

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Initialize the push channel 
            InitPushChannel();

            // Initialize the task to listen to incoming call notifications on the push channel 
            InitHttpNotificationTask();

            // Initalize the task to perform periodic maintenance 
            InitKeepAliveTask(); 

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Debug.WriteLine("[Linphone] Launching");
            LinphoneManager.Instance.ConnectBackgroundProcessToInterface();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            Debug.WriteLine("[Linphone] Activated");
            LinphoneManager.Instance.ConnectBackgroundProcessToInterface();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("[Linphone] Deactivated");
            LinphoneManager.Instance.DisconnectBackgroundProcessFromInterface();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("[Linphone] Closing");
            LinphoneManager.Instance.DisconnectBackgroundProcessFromInterface();
        }

        private const string pushChannelName = "Linphone.PushChannel";

        private void InitPushChannel()
        {
            // Try to find the push channel. 
            HttpNotificationChannel httpChannel = HttpNotificationChannel.Find(pushChannelName);

            // If the channel was not found, then create a new connection to the push service. 
            if (httpChannel == null)
            {
                // We need to create a new channel. 
                httpChannel = new HttpNotificationChannel(App.pushChannelName);
                httpChannel.Open();
            }
            else
            {
                // This is an existing channel. 
                PushChannelUri = httpChannel.ChannelUri;

                Debug.WriteLine("[Linphone] Existing Push channel URI is {0}", PushChannelUri);

                //  Let listeners know that we have a push channel URI 
                if (PushChannelUriChanged != null)
                {
                    PushChannelUriChanged(this, PushChannelUri);
                }
            }

            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
        }

        private void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Debug.WriteLine("[Linphone] New Push channel URI is {0}", e.ChannelUri);

            // Store the push channel URI 
            this.PushChannelUri = e.ChannelUri;

            //  Let listeners know that we have a push channel URI 
            if (this.PushChannelUriChanged != null)
            {
                this.PushChannelUriChanged(this, this.PushChannelUri);
            }
        }

        private void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {

        }

        private const string incomingCallTaskName = "Linphone.IncomingCallTask";

        /// <summary>
        /// Starts the push notification task.
        /// </summary>
        public void InitHttpNotificationTask()
        {
            // Obtain a reference to the existing task, if any. 
            VoipHttpIncomingCallTask incomingCallTask = ScheduledActionService.Find(incomingCallTaskName) as VoipHttpIncomingCallTask;
            if (incomingCallTask != null)
            {
                if (incomingCallTask.IsScheduled == false)
                {
                    // The incoming call task has been unscheduled due to OOM or throwing an unhandled exception twice in a row 
                    ScheduledActionService.Remove(incomingCallTaskName);
                }
                else
                {
                    // The incoming call task has been scheduled and is still scheduled so there is nothing more to do 
                    return;
                }
            }

            incomingCallTask = new VoipHttpIncomingCallTask(incomingCallTaskName, pushChannelName);
            incomingCallTask.Description = "Incoming call task";
            ScheduledActionService.Add(incomingCallTask);
        }

        private const string keepAliveTaskName = "Linphone.KeepAliveTask";

        /// <summary>
        /// Starts the keep alive task agent
        /// </summary>
        public void InitKeepAliveTask()
        {
            // Obtain a reference to the existing task, if any. 
            VoipKeepAliveTask keepAliveTask = ScheduledActionService.Find(keepAliveTaskName) as VoipKeepAliveTask;
            if (keepAliveTask != null)
            {
                if (keepAliveTask.IsScheduled == false)
                {
                    // The keep-alive task has been unscheduled due to OOM or throwing an unhandled exception twice in a row 
                    ScheduledActionService.Remove(keepAliveTaskName);
                }
                else
                {
                    // The keep-alive task has been scheduled and is still scheduled so there is nothing more to do
                    return;
                }
            }

            keepAliveTask = new VoipKeepAliveTask(keepAliveTaskName);
            keepAliveTask.Interval = new TimeSpan(10000000 * 5);
            keepAliveTask.Description = "keep-alive task";
            ScheduledActionService.Add(keepAliveTask);
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Assign custom URI mapper
            RootFrame.UriMapper = new AssociationUriMapper();

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            RootFrame.Navigating += RootFrame_Navigating;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            // Allow the app to run under the lockscreen (and prevent it from crashing when in foreground after screen lock/unlock)
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!e.IsNavigationInitiator && LinphoneManager.Instance.isLinphoneRunning)
            {
                // Disconnect the listeners to prevent crash of the background process
                LinphoneManager.Instance.LinphoneCore.CoreListener = null;
                LinphoneManager.Instance.isLinphoneRunning = false;
                Debug.WriteLine("[App] Removed listener, killing UI to force clean state at next start");
                Application.Current.Terminate();
            }
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}