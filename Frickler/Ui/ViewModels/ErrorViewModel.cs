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

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Caliburn.Micro;
using Dapplo.CaliburnMicro;

namespace Dapplo.Frickler.Ui.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    public class ErrorViewModel : Screen
    {
        public IVersionProvider VersionProvider { get; }

        [ImportingConstructor]
        public ErrorViewModel(
            [Import(AllowDefault = true)]
            IVersionProvider versionProvider)
        {
            VersionProvider = versionProvider;
        }

        /// <summary>
        /// Checks if the current version is the latest
        /// </summary>
        public bool IsMostRecent => VersionProvider?.CurrentVersion?.Equals(VersionProvider?.LatestVersion) ?? true;

        /// <summary>
        /// Set the exception to display
        /// </summary>
        public void SetExceptionToDisplay(Exception exception)
        {
            Stacktrace = exception.ToStringDemystified();
            Message = exception.Message;
        }

        /// <summary>
        /// The stacktrace to display
        /// </summary>
        public string Stacktrace { get; set; }

        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }
    }
}
