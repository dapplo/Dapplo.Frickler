#region Usings

using System.ComponentModel;
using Dapplo.CaliburnMicro.Translations;
using Dapplo.Language;

#endregion

namespace Frickler.Configuration
{
    [Language("Frickler")]
    public interface IFricklerTranslations : ILanguage, IConfigTranslations, ICoreTranslations, INotifyPropertyChanged
    {
        [DefaultValue("Enable Fiddler")]
        string IsFiddlerEnabled { get; }

        [DefaultValue("Manage HTTP_PROXY & HTTPS_PROXY")]
        string ManageHttpProxy { get; }

        [DefaultValue("Automatically authenticate")]
        string AutomaticallyAutomaticallyAuthenticate { get; }

        [DefaultValue("Is Fiddler to act as the default system proxy")]
        string IsSystemProxy { get; }

        [DefaultValue("Proxy Port")]
        string ProxyPort { get; }

        /// <summary>
        ///     This describes the name of the configuration window and system tray icon
        /// </summary>
        [DefaultValue("Configuration")]
        string Configuration { get; }

        [DefaultValue("Frickler")]
        string Title { get; }
    }
}