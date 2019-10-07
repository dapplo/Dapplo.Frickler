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

using System;
using Autofac.Features.OwnedInstances;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Frickler.Configuration;
using Frickler.Ui.ViewModels;
using MahApps.Metro.IconPacks;

namespace Frickler.Ui
{
    /// <summary>
    /// Defines the system-tray config menu item
    /// </summary>
    [Menu("systemtray")]
    public sealed class ConfigMenuItem : ClickableMenuItem, IDisposable
    {
        private readonly IFricklerTranslations _fricklerTranslations;
        private IDisposable _displayNameBinding;

        /// <inheritdoc />
        public ConfigMenuItem(
            IFricklerTranslations fricklerTranslations,
            IWindowManager windowManager,
            Func<Owned<ConfigViewModel>> configViewModelFactory)
        {
            _fricklerTranslations = fricklerTranslations;
            Style = MenuItemStyles.Default;
            Id = "B_Config";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Settings
            };

            ClickAction = item =>
            {
                IsEnabled = false;
                using (var ownedConfigViewModel = configViewModelFactory())
                {
                    windowManager.ShowDialog(ownedConfigViewModel.Value);
                }

                IsEnabled = true;
            };
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            _displayNameBinding = _fricklerTranslations.CreateDisplayNameBinding(this, nameof(IFricklerTranslations.Configuration));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _displayNameBinding?.Dispose();
            _displayNameBinding = null;
        }
    }
}
