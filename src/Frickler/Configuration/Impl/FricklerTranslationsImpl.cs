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

using Dapplo.Config.Language;
#pragma warning disable 1591

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Dapplo.Frickler.Configuration.Impl
{
    /// <summary>
    /// Implementation of IFricklerTranslations
    /// </summary>
    public class FricklerTranslationsImpl : LanguageBase<IFricklerTranslations>, IFricklerTranslations
    {
        #region Implementation of IConfigTranslations

        public string Filter { get; }

        #endregion

        #region Implementation of ICoreTranslations

        public string Cancel { get; }
        public string Ok { get; }

        #endregion

        #region Implementation of IFricklerTranslations

        public string IsFiddlerEnabled { get; }
        public string ManageHttpProxy { get; }
        public string AutomaticallyAutomaticallyAuthenticate { get; }
        public string IsSystemProxy { get; }
        public string ProxyPort { get; }
        public string Configuration { get; }
        public string Title { get; }
        public string NetworkSettingsChanged { get; }

        #endregion
    }
}
