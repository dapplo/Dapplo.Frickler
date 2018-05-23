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

using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Frickler.Configuration;
using Dapplo.Frickler.Ui.ViewModels;
using MahApps.Metro.IconPacks;

namespace Dapplo.Frickler.Ui
{
    /// <summary>
    /// Defines the systemtray config menu item
    /// </summary>
    [Menu("systemtray")]
    public sealed class ConfigMenuItem : ClickableMenuItem
    {
        /// <inheritdoc />
        public ConfigMenuItem(
            IFricklerTranslations fricklerTranslations,
            IWindowManager windowManager,
            ConfigViewModel configViewModel)
        {
            Style = MenuItemStyles.Default;
            Id = "B_Config";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Settings
            };
            ClickAction = item =>
            {
                if (configViewModel.IsActive)
                {
                    return;
                }

                windowManager.ShowDialog(configViewModel);
            };
            fricklerTranslations.CreateDisplayNameBinding(this, nameof(IFricklerTranslations.Configuration));
        }
    }
}
