using System.ComponentModel;
using Dapplo.Ini;
using Dapplo.InterfaceImpl.Extensions;

namespace Frickler.Configuration
{
    /// <summary>
    /// This defines the configuration needed for Fiddler
    /// </summary>
    [IniSection("Fiddler")]
    public interface IFiddlerConfiguration : IIniSection, INotifyPropertyChanged, IDefaultValue
    {
        /// <summary>
        /// Describes if fiddler is active
        /// </summary>
        [DefaultValue(true)]
        bool IsEnabled { get; set; }

        /// <summary>
        /// Port to proxy on
        /// </summary>
        [DefaultValue(8888)]
        int ProxyPort { get; set; }

        /// <summary>
        /// Specifies if the connections are automatically authenticated
        /// </summary>
        [DefaultValue(true)]
        bool AutomaticallyAuthenticate { get; set; }

        /// <summary>
        /// Defines if Fiddler installs itself as the default system proxy
        /// </summary>
        [DefaultValue(true)]
        bool IsSystemProxy { get; set; }
	}
}
