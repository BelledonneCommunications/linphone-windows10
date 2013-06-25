using Linphone.Core;
using Linphone.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Linphone
{
    /// <summary>
    /// Specific listener for any view which want to be notified when the mute state changes.
    /// </summary>
    public interface MuteChangedListener
    {
        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        void MuteStateChanged(bool isMicMuted);
    }

    /// <summary>
    /// Specific listener for any view which want to be notified when the pause state changes.
    /// </summary>
    public interface PauseChangedListener
    {
        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        void PauseStateChanged(LinphoneCall call, bool isCallPaused);
    }

    /// <summary>
    /// Listener called when a call is updated by the correspondent.
    /// </summary>
    public interface CallUpdatedByRemoteListener
    {
        /// <summary>
        /// Called when the call is updated by the remote party.
        /// </summary>
        /// <param name="call">The call that has been updated</param>
        /// <param name="isVideoAdded">A boolean telling whether the remote party added video</param>
        void CallUpdatedByRemote(LinphoneCall call, bool isVideoAdded);
    }

    /// <summary>
    /// Model view for each page implementing the call controller listener to adjust displayed page depending on call events.
    /// </summary>
    public class BaseModel : INotifyPropertyChanged, CallControllerListener
    {
        /// <summary>
        /// Specific listener for any view which want to be notified when the mute state changes.
        /// </summary>
        public MuteChangedListener MuteListener { get; set; }

        /// <summary>
        /// Specific listener for any view which want to be notified when the pause state changes.
        /// </summary>
        public PauseChangedListener PauseListener { get; set; }

        /// <summary>
        /// Specific listener for any view which want to be notifiedd when the call is updated by the remote party.
        /// </summary>
        public CallUpdatedByRemoteListener CallUpdatedByRemoteListener { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public BaseModel()
        {

        }

        /// <summary>
        /// Page currently displayed.
        /// </summary>
        public BasePage Page { get; set; }

        /// <summary>
        /// Page currently displayed.
        /// </summary>
        public static BasePage CurrentPage { get; set; }

        /// <summary>
        /// Dispatcher used to run tasks on the UI thread.
        /// </summary>
        public static Dispatcher UIDispatcher;

        /// <summary>
        /// Called when a call is starting.
        /// Displays the InCall.xaml page.
        /// </summary>
        /// <param name="callerNumber"></param>
        public void NewCallStarted(string callerNumber)
        {
            this.Page.NavigationService.Navigate(new Uri("/Views/InCall.xaml?sip=" + callerNumber, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Called when a call is finished.
        /// Goes back to the last page if possible, else displays Dialer.xaml.
        /// </summary>
        public void CallEnded(LinphoneCall call)
        {
            Logger.Msg("[CallListener] Call ended, can go back ? " + this.Page.NavigationService.CanGoBack);

            if (this.Page.NavigationService.CanGoBack)
                this.Page.NavigationService.GoBack();
            else
            {
                //If incall view directly accessed from home page, backstack is empty
                //If so, instead of keeping the incall view, launch the Dialer and remove the incall view from the backstack
                this.Page.NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                this.Page.NavigationService.RemoveBackEntry();
            }
        }

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        public void MuteStateChanged(Boolean isMicMuted)
        {
            if (this.MuteListener != null)
                this.MuteListener.MuteStateChanged(isMicMuted);
        }

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        public void PauseStateChanged(LinphoneCall call, bool isCallPaused)
        {
            if (this.PauseListener != null)
                this.PauseListener.PauseStateChanged(call, isCallPaused);
        }

        /// <summary>
        /// Called when the call is updated by the remote party.
        /// </summary>
        /// <param name="call">The call that has been updated</param>
        /// <param name="isVideoAdded">A boolean telling whether the remote party added video</param>
        public void CallUpdatedByRemote(LinphoneCall call, bool isVideoAdded)
        {
            if (this.CallUpdatedByRemoteListener != null)
                this.CallUpdatedByRemoteListener.CallUpdatedByRemote(call, isVideoAdded);
        }

        /// <summary>
        /// Actualises the listener when the pages changes.
        /// </summary>
        public virtual void OnNavigatedTo(NavigationEventArgs nea)
        {
            LinphoneManager.Instance.CallListener = this;
            CurrentPage = this.Page;
            UIDispatcher = this.Page.Dispatcher;
        }

        /// <summary>
        /// Actualises the listener when the pages changes.
        /// </summary>
        public virtual void OnNavigatedFrom(NavigationEventArgs nea)
        {
            LinphoneManager.Instance.CallListener = null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method called by the Set accessor of the properties to notify the change with an event.
        /// </summary>
        /// <param name="name">The property name to be notified. The CallerMemberName attribute allows to automatically pass the property name.</param>
        protected void OnPropertyChanged([CallerMemberName] String name = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
