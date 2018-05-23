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
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Toasts;
using Dapplo.Frickler.Ui.ViewModels;
using Dapplo.Log;
using Dapplo.Utils;
using Dapplo.Windows.Advapi32;
using Microsoft.Win32;

namespace Dapplo.Frickler.Modules
{
    /// <summary>
    /// This takes care of monitoring the registry for changes in the network/internet settings which the proxy needs to know of
    /// </summary>
    public class RegistryMonitorModule : IUiStartup, IUiShutdown
    {
        private static readonly LogSource Log = new LogSource();
        private const string InternetSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
        private readonly ToastConductor _toastConductor;
        private readonly IFiddlerModule _fiddlerModule;
        private readonly SynchronizationContext _uiSynchronizationContext;
        private readonly Func<InternetSettingsChangedToastViewModel> _internetSettingsChangedToastViewModelFactory;
        private IDisposable _disposables;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="toastConductor">ToastConductor used to show toasts</param>
        /// <param name="fiddlerModule">IFiddlerModule used to stop and start the proxy</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext used to show information</param>
        /// <param name="internetSettingsChangedToastViewModelFactory">NetworkSettingsChangedToastViewModel is the view model to show when changes occured.</param>
        public RegistryMonitorModule(
            ToastConductor toastConductor,
            IFiddlerModule fiddlerModule,
            [KeyFilter("ui")]SynchronizationContext uiSynchronizationContext,
            Func<InternetSettingsChangedToastViewModel> internetSettingsChangedToastViewModelFactory)
        {
            _toastConductor = toastConductor;
            _fiddlerModule = fiddlerModule;
            _uiSynchronizationContext = uiSynchronizationContext;
            _internetSettingsChangedToastViewModelFactory = internetSettingsChangedToastViewModelFactory;
        }

        /// <inheritdoc />
        public void Start()
        {
            MonitorInternetSettingsChanges();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _toastConductor.TryClose();
            _disposables?.Dispose();
        }

        /// <summary>
        /// Create a string for the internet settings, which can be used to compare for changes
        /// </summary>
        /// <param name="hive">RegistryHive</param>
        /// <returns>string</returns>
        private string SerializeInternetSettings(RegistryHive hive)
        {
            var serializedValue = new StringBuilder();
            serializedValue.AppendFormat(@"{0}\{1}", hive,InternetSettingsKey).AppendLine();
            using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
            using (var internetSettingsRegistryKey = baseKey.OpenSubKey(InternetSettingsKey))
            {
                if (internetSettingsRegistryKey == null)
                {
                    return string.Empty;
                }
                foreach(var valueName in internetSettingsRegistryKey.GetValueNames().ToList().OrderBy(s => s))
                {
                    // Ignore some values
                    if (valueName == "AutoDetect")
                    {
                        continue;;
                    }
                    object value = internetSettingsRegistryKey.GetValue(valueName);
                    if (value == null)
                    {
                        continue;
                    }

                    switch (internetSettingsRegistryKey.GetValueKind(valueName))
                    {
                        case RegistryValueKind.DWord:
                        case RegistryValueKind.QWord:
                            value = $"0x{value:X8}";
                            break;
                        case RegistryValueKind.Binary:
                            if (value is byte[] binaryValue)
                            {
                                value = string.Join(" ", binaryValue.Select(b => b.ToString("X2")));
                            }
                            break;
                    }

                    serializedValue.AppendFormat("{0} = {1}", valueName, value).AppendLine();
                }
            }

            return serializedValue.ToString();
        }

        /// <summary>
        /// This makes sure the internet settings are monitored for changes
        /// </summary>
        private void MonitorInternetSettingsChanges()
        {
            var localMachineSettings = SerializeInternetSettings(RegistryHive.LocalMachine);
            Log.Info().WriteLine("Current LocalMachine settings:");
            Log.Info().WriteLine(localMachineSettings);
            var currentUserSettings = SerializeInternetSettings(RegistryHive.CurrentUser);
            Log.Info().WriteLine("Current CurrentUser settings:");
            Log.Info().WriteLine(currentUserSettings);
            var localMachineMonitor = RegistryMonitor.ObserveChanges(RegistryHive.LocalMachine, InternetSettingsKey).ObserveOn(_uiSynchronizationContext).Select(unit => SerializeInternetSettings(RegistryHive.LocalMachine)).Where(settings => !localMachineSettings.Equals(settings));
            var currentUserMonitor = RegistryMonitor.ObserveChanges(RegistryHive.CurrentUser, InternetSettingsKey).ObserveOn(_uiSynchronizationContext).Select(unit => SerializeInternetSettings(RegistryHive.CurrentUser)).Where(settings => !currentUserSettings.Equals(settings));

            _disposables = new CompositeDisposable
            {
                localMachineMonitor.Merge(currentUserMonitor).Throttle(TimeSpan.FromSeconds(10)).Subscribe(ProcessInternetSettingsChange)
            };
            Log.Debug().WriteLine("Now monitoring changes to the Internet Settings!");
        }

        private void ProcessInternetSettingsChange(string settings)
        {
            // Make sure while restarting, other changes don't disturb the restart
            _disposables?.Dispose();
            UiContext.RunOn(async () =>
            {
                Log.Info().WriteLine("Network settings for have been changed, restarting the fiddlerModule. New settings:\r\n", settings);
                _toastConductor.ActivateItem(_internetSettingsChangedToastViewModelFactory());

                try
                {
                    _fiddlerModule.Shutdown();
                    await Task.Delay(1000).ConfigureAwait(true);
                    _fiddlerModule.Start();
                    await Task.Delay(1000).ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    Log.Error().WriteLine(ex, "Problem restarting the proxy:");
                }
                MonitorInternetSettingsChanges();
            });
        }
    }
}
