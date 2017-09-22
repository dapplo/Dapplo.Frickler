using System.ComponentModel.Composition;
using Dapplo.Addons;
using Dapplo.Log;
using Fiddler;
using Frickler.Configuration;

namespace Frickler.Modules
{
    /// <summary>
    /// The actual fiddler code
    /// </summary>
    [Export(typeof(IFiddlerModule))]
    [StartupAction, ShutdownAction]
    public class FiddlerModule : IFiddlerModule
    {
        private static readonly LogSource Log = new LogSource();
        private readonly IFiddlerConfiguration _fiddlerConfiguration;

        /// <summary>
        /// Constructor for the module
        /// </summary>
        /// <param name="fiddlerConfiguration"></param>
        [ImportingConstructor]
        public FiddlerModule(IFiddlerConfiguration fiddlerConfiguration)
        {
            _fiddlerConfiguration = fiddlerConfiguration;
        }

        /// <inheritdoc />
        public void Start()
        {
            if (!_fiddlerConfiguration.IsEnabled || FiddlerApplication.IsStarted())
            {
                return;
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
