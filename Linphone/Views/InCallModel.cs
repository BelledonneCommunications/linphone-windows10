using Linphone.Core;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Linphone.Views
{
    public class InCallModel : BaseModel
    {
        public InCallModel()
            : base()
        {
        }

        public void ShowRemoteVideo()
        {
            Int32 id = LinphoneManager.Instance.LinphoneCore.GetNativeVideoWindowId();
            RemoteVideoUri = Mediastreamer2.WP8Video.VideoRenderer.StreamUri(id);
            RemoteVideoVisibility = Visibility.Visible;
        }

        public Uri RemoteVideoUri
        {
            get
            {
                return this.remoteVideoUri;
            }

            set
            {
                if (this.remoteVideoUri != value)
                {
                    this.remoteVideoUri = value;
                    this.OnPropertyChanged("RemoteVideoUri");
                }
            }
        }

        public Visibility RemoteVideoVisibility
        {
            get
            {
                return this.remoteVideoVisibility;
            }

            set
            {
                if (this.remoteVideoVisibility != value)
                {
                    this.remoteVideoVisibility = value;
                    this.OnPropertyChanged("RemoteVideoVisibility");
                }
            }
        }

        private Uri remoteVideoUri = null;
        private Visibility remoteVideoVisibility = Visibility.Collapsed;
    }
}
