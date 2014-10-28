using Linphone.Core;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Linphone.Views
{
    /// <summary>
    /// Model for the InCall page that handles the display of the various elements of the page.
    /// </summary>
    public class ChatsModel : BaseModel
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public ChatsModel()
            : base()
        {
        }

        public ObservableCollection<Conversation> Conversations
        {
            get
            {
                return conversations;
            }
            set
            {
                conversations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Conversation> conversations = new ObservableCollection<Conversation>();
    }
}