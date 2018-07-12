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
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro.Toasts;
using Dapplo.Frickler.Extensions;
using Dapplo.Frickler.Ui.ViewModels;
using Dapplo.Log;
using Dapplo.Utils;
using Dapplo.Windows.Advapi32;
using Microsoft.Win32;
using Polly;
using Polly.Retry;

namespace Dapplo.Frickler.Modules
{
    /// <summary>
    /// This takes care of monitoring the registry for changes in the network/internet settings which the proxy needs to know of
    /// </summary>
    [Service(nameof(RegistryMonitorModule), nameof(FiddlerModule), TaskSchedulerName = "ui")]
    public class RegistryMonitorModule : IStartup, IShutdown
    {
        private static readonly LogSource Log = new LogSource();
        private const string InternetSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
        private readonly ToastConductor _toastConductor;
        private readonly IFiddlerModule _fiddlerModule;
        private readonly SynchronizationContext _uiSynchronizationContext;
        private readonly Func<IEnumerable<DictionaryChangeInfo<string, string>>, InternetSettingsChangedToastViewModel> _internetSettingsChangedToastViewModelFactory;
        private IDisposable _monitorObservable;
        private static readonly RetryPolicy RetryPolicy = Policy.Handle<Exception>().WaitAndRetry(3, retryCount => TimeSpan.FromMilliseconds(retryCount * 200));

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
            Func<IEnumerable<DictionaryChangeInfo<string, string>>, InternetSettingsChangedToastViewModel> internetSettingsChangedToastViewModelFactory)
        {
            _toastConductor = toastConductor;
            _fiddlerModule = fiddlerModule;
            _uiSynchronizationContext = uiSynchronizationContext;
            _internetSettingsChangedToastViewModelFactory = internetSettingsChangedToastViewModelFactory;
        }

        /// <inheritdoc />
        public void Startup()
        {
            MonitorInternetSettingsChanges();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _toastConductor.TryClose();
            _monitorObservable?.Dispose();
        }

        /// <summary>
        /// Create a string for the internet settings, which can be used to compare for changes
        /// </summary>
        /// <param name="hive">RegistryHive</param>
        /// <returns>string</returns>
        private IDictionary<string, string> SerializeInternetSettings(RegistryHive hive)
        {
            var serializedValue = new SortedDictionary<string, string>();
            using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
            using (var internetSettingsRegistryKey = baseKey.OpenSubKey(InternetSettingsKey))
            {
                if (internetSettingsRegistryKey == null)
                {
                    return serializedValue;
                }
                foreach(var valueName in internetSettingsRegistryKey.GetValueNames().ToList().OrderBy(s => s))
                {
                    // Ignore some values
                    if (valueName == "AutoDetect")
                    {
                        continue;
                    }
                    object value = internetSettingsRegistryKey.GetValue(valueName);
                    if (value == null)
                    {
                        continue;
                    }
                    string stringValue = value as string ?? value.ToString();

                    switch (internetSettingsRegistryKey.GetValueKind(valueName))
                    {
                        case RegistryValueKind.DWord:
                        case RegistryValueKind.QWord:
                            stringValue = $"0x{value:X8}";
                            break;
                        case RegistryValueKind.Binary:
                            if (value is byte[] binaryValue)
                            {
                                stringValue = string.Join(" ", binaryValue.Select(b => b.ToString("X2")));
                            }
                            break;
                    }

                    serializedValue[valueName] = stringValue;
                }
            }

            return serializedValue;
        }

        /// <summary>
        /// This makes sure the internet settings are monitored for changes
        /// </summary>
        private void MonitorInternetSettingsChanges()
        {
            var currentLocalMachineSettings = SerializeInternetSettings(RegistryHive.LocalMachine);
            Log.Info().WriteLine("Current LocalMachine settings:");
            Log.Info().WriteLine(string.Join("\r\n", currentLocalMachineSettings.Select(kv => $"{kv.Key} = {kv.Value}")));
            var currentUserSettings = SerializeInternetSettings(RegistryHive.CurrentUser);
            Log.Info().WriteLine("Current CurrentUser settings:");
            Log.Info().WriteLine(string.Join("\r\n", currentUserSettings.Select(kv => $"{kv.Key} = {kv.Value}")));
            var localMachineMonitor = RegistryMonitor.ObserveChanges(RegistryHive.LocalMachine, InternetSettingsKey).ObserveOn(_uiSynchronizationContext).Select(unit => (currentLocalMachineSettings, RetryPolicy.Execute(() => SerializeInternetSettings(RegistryHive.LocalMachine)))).Where(settings => settings.currentLocalMachineSettings.DetectChanges(settings.Item2).Any());
            var currentUserMonitor = RegistryMonitor.ObserveChanges(RegistryHive.CurrentUser, InternetSettingsKey).ObserveOn(_uiSynchronizationContext).Select(unit => (currentUserSettings, RetryPolicy.Execute(() => SerializeInternetSettings(RegistryHive.CurrentUser)))).Where(settings => settings.currentUserSettings.DetectChanges(settings.Item2).Any());

            _monitorObservable = new CompositeDisposable
            {
                localMachineMonitor.Merge(currentUserMonitor).Throttle(TimeSpan.FromSeconds(10)).Subscribe(s => ProcessInternetSettingsChange(s.Item1, s.Item2))
            };
            Log.Debug().WriteLine("Now monitoring changes to the Internet Settings!");
        }

        private void ProcessInternetSettingsChange(IDictionary<string, string> before, IDictionary<string, string> after)
        {
            Log.Info().WriteLine("Network settings for have been changed, restarting the fiddlerModule. Modifications:\r\n");
            var changes = before.DetectChanges(after).ToList();
            Log.Info().WriteLine(string.Join("\r\n", changes));
            // Make sure while restarting, other changes don't disturb the restart
            _monitorObservable?.Dispose();
            UiContext.RunOn(() =>
            {
                _toastConductor.ActivateItem(_internetSettingsChangedToastViewModelFactory(changes));
            });
            Task.Run(async () =>
            {
                try
                {
                    await Task.Run(() => _fiddlerModule.Shutdown()).ConfigureAwait(true);
                    await Task.Delay(1000).ConfigureAwait(true);
                    await Task.Run(() => _fiddlerModule.Startup()).ConfigureAwait(true);
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
