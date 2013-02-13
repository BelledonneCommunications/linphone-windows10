using Linphone.Resources;

namespace Linphone
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        /// <summary>
        /// Provides easy access to localized resources.
        /// </summary>
        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}