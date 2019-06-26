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

using Dapplo.CaliburnMicro.Metro;
using Dapplo.Config.Ini;

#pragma warning disable 1591

namespace Dapplo.Frickler.Configuration.Impl
{
    public class FiddlerConfigurationImpl : IniSectionBase<IFiddlerConfiguration>, IFiddlerConfiguration
    {
        private readonly MetroThemeManager _metroThemeManager;

        public FiddlerConfigurationImpl(MetroThemeManager metroThemeManager)
        {
            _metroThemeManager = metroThemeManager;
        }

        public bool IsEnabled { get; set; }
        public bool ManageProxyEnvironmentVariables { get; set; }
        public int ProxyPort { get; set; }
        public bool AutomaticallyAuthenticate { get; set; }
        public bool IsSystemProxy { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }

        public string Theme { get; set; }

        public string ThemeColor { get; set; }


        public override void AfterLoad()
        {
            if (string.IsNullOrEmpty(Theme))
            {
                Theme = null;
            }
            if (string.IsNullOrEmpty(ThemeColor))
            {
                ThemeColor = null;
            }
            _metroThemeManager.ChangeTheme(Theme, ThemeColor);

            base.AfterLoad();
        }
    }
}
