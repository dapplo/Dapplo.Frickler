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

using Dapplo.Addons;
using Dapplo.Config.Ini.Converters;

#endregion

namespace Dapplo.Frickler.Modules
{
    /// <summary>
    ///     Initialize the Configuration framework
    /// </summary>
    [Service(nameof(ConfigStartup))]
    public class ConfigStartup : IStartup
    {
        /// <summary>
        ///     Initialize the Configuration framework
        /// </summary>
        public void Startup()
        {
            StringEncryptionTypeConverter.RgbIv = "pgf02URf@h1!f2rA";
            StringEncryptionTypeConverter.RgbKey = "dKjjh@fjh34g8tg$d0o56SDFgFH23eo0";
        }
    }
}