#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.Translations;
using Dapplo.Log;
using Frickler.Configuration;
using MahApps.Metro.IconPacks;

#endregion

namespace Frickler.Ui.ViewModels
{
    /// <summary>
    ///     The settings view model is, well... for the settings :)
    ///     It is a conductor where one item is active.
    /// </summary>
    [Export]
    public sealed class ConfigViewModel : Config<IConfigScreen>, IMaintainPosition
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Constructor which takes care of exporting the ConfigMenuItem
        /// </summary>
        [ImportingConstructor]
        public ConfigViewModel([ImportMany] IEnumerable<Lazy<IConfigScreen>> configScreens,
            IWindowManager windowManager,
            IFricklerTranslations fricklerTranslations)
        {
            ConfigScreens = configScreens;
            CoreTranslations = fricklerTranslations;
            ConfigTranslations = fricklerTranslations;
            FricklerTranslations = fricklerTranslations;
            var configMenuItem = new ClickableMenuItem
            {
                Style = MenuItemStyles.Default,
                Id = "B_Config",
                Icon = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.Settings
                },
                ClickAction = item =>
                {
                    if (IsActive)
                    {
                        return;
                    }
                    if (windowManager.ShowDialog(this) == false)
                    {
                        Log.Warn().WriteLine("The configuration was cancelled.");
                    }
                }
            };
            fricklerTranslations.CreateDisplayNameBinding(configMenuItem, nameof(IFricklerTranslations.Configuration));
            ConfigMenuItem = configMenuItem;
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

        [Export("systemtray", typeof(IMenuItem))]
        private IMenuItem ConfigMenuItem { get; }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            base.OnActivate();
            // automatically update the DisplayName
            FricklerTranslations.CreateDisplayNameBinding(this, nameof(IFricklerTranslations.Configuration));
        }
    }
}