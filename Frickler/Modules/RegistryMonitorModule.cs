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

using System;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Toasts;
using Dapplo.Frickler.Ui.ViewModels;
using Dapplo.Log;
using Dapplo.Windows.Advapi32;
using Microsoft.Win32;

namespace Dapplo.Frickler.Modules
{
    [UiStartupAction, UiShutdownAction]
    public class RegistryMonitorModule : IUiStartupAction, IUiShutdownAction
    {
        private static readonly LogSource Log = new LogSource();
        private const string InternetSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
        private readonly ToastConductor _toastConductor;
        private readonly IFiddlerModule _fiddlerModule;
        private readonly SynchronizationContext _uiSynchronizationContext;
        private readonly NetworkSettingsChangedToastViewModel _networkSettingsChangedToastViewModel;
        private IDisposable _disposables;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toastConductor">ToastConductor used to show toasts</param>
        /// <param name="fiddlerModule">IFiddlerModule used to stop and start the proxy</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext used to show information</param>
        /// <param name="networkSettingsChangedToastViewModel">NetworkSettingsChangedToastViewModel is the view model to show when changes occured.</param>
        [ImportingConstructor]
        public RegistryMonitorModule(
            ToastConductor toastConductor,
            IFiddlerModule fiddlerModule,
            [Import("ui")]
            SynchronizationContext uiSynchronizationContext,
            NetworkSettingsChangedToastViewModel networkSettingsChangedToastViewModel)
        {
            _toastConductor = toastConductor;
            _fiddlerModule = fiddlerModule;
            _uiSynchronizationContext = uiSynchronizationContext;
            _networkSettingsChangedToastViewModel = networkSettingsChangedToastViewModel;
        }

        /// <inheritdoc />
        public void Start()
        {
            MonitorNetworkChanges();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _toastConductor.TryClose();
            _disposables?.Dispose();
        }

        private void MonitorNetworkChanges()
        {
            _disposables = new CompositeDisposable
            {

                RegistryMonitor.ObserveChanges(RegistryHive.LocalMachine, InternetSettingsKey).ObserveOn(_uiSynchronizationContext).SubscribeOn(_uiSynchronizationContext).Subscribe(ProcessNetworkSettingsChange),
                RegistryMonitor.ObserveChanges(RegistryHive.CurrentUser, InternetSettingsKey).ObserveOn(_uiSynchronizationContext).SubscribeOn(_uiSynchronizationContext).Subscribe(ProcessNetworkSettingsChange)
            };
        }

        private void ProcessNetworkSettingsChange(Unit unit)
        {
            _disposables?.Dispose();

            Log.Info().WriteLine("Network settings have been changed, restarting the fiddlerModule.");
            _fiddlerModule.Shutdown();
            _fiddlerModule.Start();
            _toastConductor.ActivateItem(_networkSettingsChangedToastViewModel);
            MonitorNetworkChanges();
        }

    }
}
