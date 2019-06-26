// Dapplo - building blocks for desktop applications
// Copyright (C) 2017-2019  Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Frickler
// 
// Frickler is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Frickler is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Frickler. If not, see <http://www.gnu.org/licenses/lgpl.txt>.
// 

#region Usings

using System.ComponentModel;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.Config.Ini;
using Dapplo.Config.Ini.Converters;

#endregion

namespace Dapplo.Frickler.Configuration
{
    /// <summary>
    ///     This defines the configuration needed for Fiddler
    /// </summary>
    [IniSection("Fiddler")]
    public interface IFiddlerConfiguration : IIniSection, IMetroUiConfiguration
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

        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(StringEncryptionTypeConverter))]
        string ProxyUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(StringEncryptionTypeConverter))]
        string ProxyPassword { get; set; }
    }
}