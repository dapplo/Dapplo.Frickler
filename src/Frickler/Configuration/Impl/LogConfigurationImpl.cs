// Dapplo - building blocks for desktop applications
// Copyright (C) 2017-2018  Dapplo
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

using System.Collections.Generic;
using Dapplo.Config.Ini;
using Dapplo.Log;
#pragma warning disable 1591

namespace Dapplo.Frickler.Configuration.Impl
{
    public class LogConfigurationImpl : IniSectionBase<ILogConfiguration>, ILogConfiguration
    {
        #region Implementation of ILoggerConfiguration

        public LogLevels LogLevel { get; set; }
        public bool UseShortSource { get; set; }
        public string DateTimeFormat { get; set; }
        public string LogLineFormat { get; set; }

        #endregion

        #region Implementation of IFileLoggerConfiguration

        public bool PreFormat { get; set; }
        public int MaxBufferSize { get; set; }
        public int WriteInterval { get; set; }
        public string ProcessName { get; set; }
        public string Extension { get; set; }
        public string FilenamePattern { get; set; }
        public string DirectoryPath { get; set; }
        public string ArchiveFilenamePattern { get; set; }
        public string ArchiveDirectoryPath { get; set; }
        public string ArchiveExtension { get; set; }
        public bool ArchiveCompress { get; set; }
        public int ArchiveCount { get; set; }
        public IList<string> ArchiveHistory { get; set; }

        #endregion
    }
}
