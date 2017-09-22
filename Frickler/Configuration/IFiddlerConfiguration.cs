#region Usings

using System.ComponentModel;
using Dapplo.Ini;
using Dapplo.InterfaceImpl.Extensions;

#endregion

namespace Frickler.Configuration
{
    /// <summary>
    ///     This defines the configuration needed for Fiddler
    /// </summary>
    [IniSection("Fiddler")]
    public interface IFiddlerConfiguration : IIniSection, INotifyPropertyChanged, IDefaultValue, ITransactionalProperties
    {
        /// <summary>
        ///     Describes if fiddler is active
        /// </summary>
        [DefaultValue(true)]
        bool IsEnabled { get; set; }

        /// <summary>
        ///     Describes if we need to manage the proxy environment variables?
        /// </summary>
        [DefaultValue(true)]
        bool ManageProxyEnvironmentVariables { get; set; }

        /// <summary>
        ///     Port to proxy on
        /// </summary>
        [DefaultValue(8888)]
        int ProxyPort { get; set; }

        /// <summary>
        ///     Specifies if the connections are automatically authenticated
        /// </summary>
        [DefaultValue(true)]
        bool AutomaticallyAuthenticate { get; set; }

        /// <summary>
        ///     Defines if Fiddler installs itself as the default system proxy
        /// </summary>
        [DefaultValue(true)]
        bool IsSystemProxy { get; set; }
    }
}