using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    /// <summary>
    /// Represents a chat conversation between the user and someone else.
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Display name of the remote contact.
        /// </summary>
        public string DisplayedName { get; set; }

        /// <summary>
        /// SIP address of the remote contact.
        /// </summary>
        public string SipAddress { get; set; }

        /// <summary>
        /// Latest message (can be troncated) received or sent.
        /// </summary>
        public string LatestMessage { get; set; }

        /// <summary>
        /// Date of the latest message received or sent.
        /// </summary>
        public string LatestMessageDate { get; set; }

        /// <summary>
        /// List of messages sent and received.
        /// </summary>
        public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Conversation(string sipAddress, string displayName, List<ChatMessage> messages)
        {
            SipAddress = sipAddress;
            DisplayedName = displayName;
            Messages = messages;
        }
    }
}
