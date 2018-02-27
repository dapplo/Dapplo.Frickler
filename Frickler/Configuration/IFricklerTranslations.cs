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
// 

#region Usings

using System.ComponentModel;
using Dapplo.CaliburnMicro.Translations;
using Dapplo.Language;

#endregion

namespace Dapplo.Frickler.Configuration
{
    /// <summary>
    ///     The translations for Frickler
    /// </summary>
    [Language("Frickler")]
    public interface IFricklerTranslations : ILanguage, IConfigTranslations, ICoreTranslations, INotifyPropertyChanged
    {
        /// <summary>
        ///     The translation for the enable fiddle checkbox in the configuration
        /// </summary>
        [DefaultValue("Enable Fiddler")]
        string IsFiddlerEnabled { get; }

        /// <summary>
        ///     The translation for the manage HTTP_PROXY and HTTPS_PROXY checkbox in the configuration
        /// </summary>
        [DefaultValue("Manage HTTP_PROXY & HTTPS_PROXY")]
        string ManageHttpProxy { get; }

        /// <summary>
        ///     The translation for the automatically authenticate checkbox in the configuration
        /// </summary>
        [DefaultValue("Automatically authenticate")]
        string AutomaticallyAutomaticallyAuthenticate { get; }

        /// <summary>
        ///     The translation for the is system proxy checkbox in the configuration
        /// </summary>
        [DefaultValue("Is Fiddler to act as the default system proxy")]
        string IsSystemProxy { get; }

        /// <summary>
        ///     The translation for the proxy port textbox label in the configuration
        /// </summary>
        [DefaultValue("Proxy Port")]
        string ProxyPort { get; }

        /// <summary>
        ///     This describes the name of the configuration window and system tray icon
        /// </summary>
        [DefaultValue("Configuration")]
        string Configuration { get; }

        /// <summary>
        ///     The title of the application
        /// </summary>
        [DefaultValue("Frickler")]
        string Title { get; }
    }
}