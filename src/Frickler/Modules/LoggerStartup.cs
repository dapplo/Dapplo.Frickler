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

using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.Frickler.Configuration;
using Dapplo.Log;
#if !DEBUG
using Dapplo.Log.LogFile;
#endif

#endregion

namespace Dapplo.Frickler.Modules
{
    /// <summary>
    ///     Initialize the logging
    /// </summary>
    [Service(nameof(LoggerStartup), nameof(CaliburnServices.ConfigurationService))]
    public class LoggerStartup : IStartup
    {
        private readonly ILogConfiguration _logConfiguration;

        /// <inheritdoc />
        public LoggerStartup(ILogConfiguration logConfiguration)
        {
            _logConfiguration = logConfiguration;
        }

        /// <summary>
        ///     Initialize the logging
        /// </summary>
        public void Startup()
        {
            _logConfiguration.PreFormat = true;
            _logConfiguration.WriteInterval = 100;

            // TODO: Decide on the log level, make available in the UI?
            _logConfiguration.LogLevel = LogLevels.Debug;
#if !DEBUG
            LogSettings.RegisterDefaultLogger<FileLogger>(_logConfiguration);
#endif
        }
    }
}