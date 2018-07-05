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

#region Usings

using System;
using System.Collections.Generic;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Translations;
using Dapplo.Frickler.Configuration;

#endregion

namespace Dapplo.Frickler.Ui.ViewModels
{
    /// <summary>
    ///     The settings view model is, well... for the settings :)
    ///     It is a conductor where one item is active.
    /// </summary>
    public sealed class ConfigViewModel : Config<IConfigScreen>, IMaintainPosition
    {
        /// <summary>
        ///     Constructor which takes care of exporting the ConfigMenuItem
        /// </summary>
        public ConfigViewModel(
            IEnumerable<Lazy<IConfigScreen>> configScreens,
            IFricklerTranslations fricklerTranslations)
        {
            ConfigScreens = configScreens;
            CoreTranslations = fricklerTranslations;
            ConfigTranslations = fricklerTranslations;
            FricklerTranslations = fricklerTranslations;
        }

        /// <summary>
        ///     The core translations for the view (ok / cancel)
        /// </summary>
        public ICoreTranslations CoreTranslations { get; private set; }

        /// <summary>
        ///     The translations for the config view
        /// </summary>
        public IConfigTranslations ConfigTranslations { get; set; }

        /// <summary>
        ///     The CallINGTranslations (configuration)
        /// </summary>
        public IFricklerTranslations FricklerTranslations { get; set; }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            base.OnActivate();
            // automatically update the DisplayName
            FricklerTranslations.CreateDisplayNameBinding(this, nameof(IFricklerTranslations.Configuration));
        }
    }
}