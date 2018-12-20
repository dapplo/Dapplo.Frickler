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
using System.Globalization;
using System.Threading;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Dapplo.Addons.Bootstrapper;
using Dapplo.CaliburnMicro.Dapp;
using Dapplo.Config.Ini.Converters;
using Dapplo.Frickler.Ui.ViewModels;
using Dapplo.Log;

#if DEBUG
using Dapplo.Log.Loggers;
#else
using Dapplo.Log.LogFile;
#endif

#endregion

namespace Dapplo.Frickler
{
    /// <summary>
    ///     This takes care or starting the Application
    /// </summary>
    public static class Startup
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Start the application
        /// </summary>
        [STAThread]
        public static int Main()
        {
#if DEBUG
            // Initialize a debug logger for Dapplo packages
            LogSettings.RegisterDefaultLogger<DebugLogger>(LogLevels.Verbose);
#else
            LogSettings.RegisterDefaultLogger<ForwardingLogger>(LogLevels.Debug);
#endif

            // TODO: Set via build
            StringEncryptionTypeConverter.RgbIv = "dlgjowejgogkklwj";
            StringEncryptionTypeConverter.RgbKey = "lsjvkwhvwujkagfauguwcsjgu2wueuff";

            // Use this to setup the culture of your UI
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            Log.Info().WriteLine("Windows version {0}", Environment.OSVersion.Version);
            var applicationConfig = ApplicationConfigBuilder
                .Create()
                .WithApplicationName("Frickler")
                .WithMutex("AD5323E2-7614-46F2-8F80-2F8667970367")
                .WithoutCopyOfEmbeddedAssemblies()
                .WithoutCopyOfAssembliesToProbingPath()
                .WithCaliburnMicro()
                .BuildApplicationConfig();

            var application = new Dapplication(applicationConfig)
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            // Prevent multiple instances
            if (application.WasAlreadyRunning)
            {
                Log.Warn().WriteLine("{0} was already running.", applicationConfig.ApplicationName);
                // Don't start the dapplication, exit with 0
                application.Shutdown(-1);
                return -1;
            }

            RegisterErrorHandlers(application);

            application.Run();
            return 0;
        }

        /// <summary>
        /// Make sure all exception handlers are hooked
        /// </summary>
        /// <param name="application">Dapplication</param>
        private static void RegisterErrorHandlers(Dapplication application)
        {
            application.OnUnhandledAppDomainException += (exception, b) => DisplayErrorViewModel(exception);
            application.OnUnhandledDispatcherException += DisplayErrorViewModel;
            application.OnUnhandledTaskException += DisplayErrorViewModel;
        }

        /// <summary>
        /// Show the exception
        /// </summary>
        /// <param name="exception">Exception</param>
        private static void DisplayErrorViewModel(Exception exception)
        {
            var windowManager = Dapplication.Current.Bootstrapper.Container.Resolve<IWindowManager>();
            var errorViewModel = Dapplication.Current.Bootstrapper.Container.Resolve<ErrorViewModel>();
            if (windowManager == null || errorViewModel == null)
            {
                return;
            }

            errorViewModel.SetExceptionToDisplay(exception);
            windowManager.ShowWindow(errorViewModel);
        }
    }
}