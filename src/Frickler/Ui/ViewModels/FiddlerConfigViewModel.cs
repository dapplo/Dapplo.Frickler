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
// 

using System;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Frickler.Configuration;
using Frickler.Modules;

namespace Frickler.Ui.ViewModels
{
    /// <summary>
    ///     The fiddler config ViewModel
    /// </summary>
    public sealed class FiddlerConfigViewModel : SimpleConfigScreen
    {
        private readonly IFiddlerModule _fiddlerModule;
        private IDisposable _displayNameBinding;
        /// <summary>
        ///     construct the ViewModel
        /// </summary>
        /// <param name="fiddlerConfiguration">IFiddlerConfiguration</param>
        /// <param name="fiddlerModule">IFiddlerModule used to stop/start</param>
        /// <param name="fricklerTranslations">IFricklerTranslations</param>
        public FiddlerConfigViewModel(
            IFiddlerConfiguration fiddlerConfiguration,
            IFiddlerModule fiddlerModule,
            IFricklerTranslations fricklerTranslations)
        {
            _fiddlerModule = fiddlerModule;
            Id = "C_Fiddler";
            FiddlerConfiguration = fiddlerConfiguration;
            FricklerTranslations = fricklerTranslations;
            _displayNameBinding = FricklerTranslations.CreateDisplayNameBinding(this, nameof(IFricklerTranslations.Title));
        }

        /// <summary>
        ///     Used from the View
        /// </summary>
        public IFiddlerConfiguration FiddlerConfiguration { get; }

        /// <summary>
        ///     Used from the View
        /// </summary>
        public IFricklerTranslations FricklerTranslations { get; }

        /// <inheritdoc />
        public override void Commit()
        {
            _fiddlerModule.Shutdown();
            base.Commit();
            _fiddlerModule.Startup();
        }
    }
}