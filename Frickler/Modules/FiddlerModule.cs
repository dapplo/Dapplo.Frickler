#region Usings

using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using Dapplo.Addons;
using Dapplo.Log;
using Fiddler;
using Frickler.Configuration;

#endregion

namespace Frickler.Modules
{
	/// <summary>
	///     The actual fiddler code
	/// </summary>
	[Export(typeof(IFiddlerModule))]
    [StartupAction]
    [ShutdownAction]
    public class FiddlerModule : IFiddlerModule
    {
        private static readonly LogSource Log = new LogSource();
        private readonly IFiddlerConfiguration _fiddlerConfiguration;
        private const string HttpsProxyVariable = "HTTPS_PROXY";
        private const string HttpProxyVariable = "HTTP_PROXY";
        private IDisposable _proxyDisposable;

	    /// <summary>
	    ///     Constructor for the module
	    /// </summary>
	    /// <param name="fiddlerConfiguration"></param>
	    [ImportingConstructor]
        public FiddlerModule(IFiddlerConfiguration fiddlerConfiguration)
        {
            _fiddlerConfiguration = fiddlerConfiguration;
        }

	    /// <summary>
	    ///     Setup the proxy environment
	    /// </summary>
	    /// <returns></returns>
	    private IDisposable ModifyProxyEnvironment()
        {
            var httpsProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpsProxyVariable, EnvironmentVariableTarget.Machine);
            var httpProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpProxyVariable, EnvironmentVariableTarget.Machine);

            var httpProxy = $"http://localhost:{_fiddlerConfiguration.ProxyPort}/";
            var httpsProxy = $"http://localhost:{_fiddlerConfiguration.ProxyPort}/";
            SetProxyEnvironmentVariables(httpProxy, httpsProxy);
            return Disposable.Create(() => SetProxyEnvironmentVariables(httpsProxyFromEnvironmentVariable, httpProxyFromEnvironmentVariable));
        }

        private void SetProxyEnvironmentVariables(string httpProxy, string httpsProxy)
        {
            Environment.SetEnvironmentVariable(HttpProxyVariable, httpProxy, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(HttpsProxyVariable, httpsProxy, EnvironmentVariableTarget.Machine);
        }

        /// <inheritdoc />
        public void Start()
        {
            if (!_fiddlerConfiguration.IsEnabled || FiddlerApplication.IsStarted())
            {
                return;
            }
            _proxyDisposable?.Dispose();
            if (_fiddlerConfiguration.ManageProxyEnvironmentVariables)
            {
                _proxyDisposable = ModifyProxyEnvironment();
            }

            var startupFlags = FiddlerCoreStartupFlags.ChainToUpstreamGateway | FiddlerCoreStartupFlags.OptimizeThreadPool;
            if (_fiddlerConfiguration.IsSystemProxy)
            {
                startupFlags |= FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            }
            // Call Startup to tell FiddlerCore to begin listening on the specified port,
            // and optionally register as the system proxy and optionally decrypt HTTPS traffic.
            FiddlerApplication.Startup(_fiddlerConfiguration.ProxyPort, startupFlags);

            // This enables the automatic authentication
            FiddlerApplication.BeforeRequest += OnBeforeRequest;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            if (!FiddlerApplication.IsStarted())
            {
                return;
            }
            _proxyDisposable?.Dispose();
            FiddlerApplication.BeforeRequest -= OnBeforeRequest;
            FiddlerApplication.oProxy.Detach();
        }

        private void OnBeforeRequest(Session session)
        {
            Log.Debug().WriteLine("Request to: {0}", session.fullUrl);

            if (_fiddlerConfiguration.AutomaticallyAuthenticate)
            {
                // This enables the automatic authentication
                session["X-AutoAuth"] = "(default)";
            }
        }
    }
}