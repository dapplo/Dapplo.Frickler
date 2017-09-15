using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Dapplo.CaliburnMicro.Dapp;
using Dapplo.Log;
using Dapplo.Log.Loggers;

namespace Frickler
{
    /// <summary>
    ///     This takes care or starting the Application
    /// </summary>
    public static class Startup
    {
        /// <summary>
        ///     Start the application
        /// </summary>
        [STAThread]
        public static void Main()
        {
#if DEBUG
            // Initialize a debug logger for Dapplo packages
            LogSettings.RegisterDefaultLogger<DebugLogger>(LogLevels.Debug);
#endif

            // Use this to setup the culture of your UI
            var cultureInfo = CultureInfo.GetCultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            var application = new Dapplication("Frickler", "AD5323E2-7614-46F2-8F80-2F8667970367")
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            // Load the Application.Demo.* assemblies
            application.Bootstrapper.FindAndLoadAssemblies("Application.Demo.*");
            application.Run();
        }
    }
}
