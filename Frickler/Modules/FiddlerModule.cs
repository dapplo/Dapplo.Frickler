using System.ComponentModel.Composition;
using Dapplo.Log;
using Fiddler;
using Frickler.Configuration;

namespace Frickler.Modules
{
    /// <summary>
    /// The actual fiddler code
    /// </summary>
    [Export(typeof(IFiddlerModule))]
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
        public void Startup()
        {
            if (!_fiddlerConfiguration.IsEnabled)
            {
                return;
            }
            // Call Startup to tell FiddlerCore to begin listening on the specified port,
            // and optionally register as the system proxy and optionally decrypt HTTPS traffic.
            FiddlerApplication.Startup(_fiddlerConfiguration.ProxyPort, _fiddlerConfiguration.IsSystemProxy, false, false);

            // This enables the automatic authentication
            FiddlerApplication.BeforeRequest += OnBeforeRequest;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            FiddlerApplication.BeforeRequest -= OnBeforeRequest;
            FiddlerApplication.Shutdown();
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
