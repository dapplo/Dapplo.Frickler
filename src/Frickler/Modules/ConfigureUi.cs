﻿// Dapplo - building blocks for desktop applications
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

using System;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;

namespace Frickler.Modules
{
    /// <inheritdoc />
    [Service(nameof(ConfigureUiDefaults), nameof(CaliburnServices.ConfigurationService), TaskSchedulerName = "ui")]
    public class ConfigureUiDefaults : IStartup
    {
        private readonly ResourceManager _resourceManager;

        /// <inheritdoc />
        public ConfigureUiDefaults(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <inheritdoc />
        public void Startup()
        {
            var fricklerResourceDirectory = new Uri("pack://application:,,,/Frickler;component/FricklerResourceDirectory.xaml", UriKind.RelativeOrAbsolute);
            _resourceManager.AddResourceDictionary(fricklerResourceDirectory);
        }
    }
}
