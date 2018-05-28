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
// 

#region Usings

using System;
using System.Reactive.Disposables;
using Dapplo.Frickler.Configuration;
using Dapplo.Log;
using Fiddler;

#endregion

namespace Dapplo.Frickler.Modules
{
    /// <summary>
    ///     The actual fiddler code
    /// </summary>
    public class FiddlerModule : IFiddlerModule
    {
        private static readonly LogSource Log = new LogSource();
        private const string HttpsProxyVariable = "HTTPS_PROXY";
        private const string HttpProxyVariable = "HTTP_PROXY";
        private readonly IFiddlerConfiguration _fiddlerConfiguration;
        private IDisposable _proxyDisposable;

        /// <summary>
        ///     Constructor for the module
        /// </summary>
        /// <param name="fiddlerConfiguration"></param>
        public FiddlerModule(IFiddlerConfiguration fiddlerConfiguration)
        {
            _fiddlerConfiguration = fiddlerConfiguration;
        }

        /// <inheritdoc />
        public void Start()
        {
            if (!_fiddlerConfiguration.IsEnabled)
            {
                Log.Info().WriteLine("Fiddler is not enabled.");
                return;
            }
            Log.Info().WriteLine("Starting Fiddler.");
            if (FiddlerApplication.IsStarted())
            {
                Log.Warn().WriteLine("Fiddler already started");
                return;
            }
            _proxyDisposable?.Dispose();
            if (_fiddlerConfiguration.ManageProxyEnvironmentVariables)
            {
                _proxyDisposable = ModifyProxyEnvironment();
            }

            var startupFlags =  FiddlerCoreStartupFlags.ChainToUpstreamGateway | FiddlerCoreStartupFlags.OptimizeThreadPool;
            if (_fiddlerConfiguration.IsSystemProxy)
            {
                startupFlags |= FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            }

            Log.Info().WriteLine("Starting Fiddler proxy, on http://localhost:{0} with {1}", _fiddlerConfiguration.ProxyPort, startupFlags);
            // Call Startup to tell FiddlerCore to begin listening on the specified port,
            // and optionally register as the system proxy and optionally decrypt HTTPS traffic.
            FiddlerApplication.Startup(_fiddlerConfiguration.ProxyPort, startupFlags);

            // This enables the automatic authentication
            if (_fiddlerConfiguration.AutomaticallyAuthenticate)
            {
                Log.Info().WriteLine("Enabling automatic authentication.");
                FiddlerApplication.BeforeRequest += OnBeforeRequest;
            }
            // This makes logging of error possible
            FiddlerApplication.ResponseHeadersAvailable += OnResponseHeadersAvailable;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            Log.Info().WriteLine("Stopping Fiddler.");
            FiddlerApplication.BeforeRequest -= OnBeforeRequest;
            FiddlerApplication.ResponseHeadersAvailable -= OnResponseHeadersAvailable;
            _proxyDisposable?.Dispose();
            if (!FiddlerApplication.IsStarted())
            {
                Log.Warn().WriteLine("No shutdown, as Fiddler was not started.");
                return;
            }

            if (!FiddlerApplication.oProxy.IsAttached)
            {
                return;
            }

            Log.Info().WriteLine("Detaching proxy.");
            FiddlerApplication.oProxy.Detach();
        }

        /// <summary>
        ///     Setup the proxy environment variables
        /// </summary>
        /// <returns>IDisposable which returns the environment variables back to the original value when disposed</returns>
        private IDisposable ModifyProxyEnvironment()
        {
            Log.Info().WriteLine("Setting the proxy environment variables.");
            // These are the new values
            var httpProxy = $"http://localhost:{_fiddlerConfiguration.ProxyPort}/";
            var httpsProxy = $"http://localhost:{_fiddlerConfiguration.ProxyPort}/";

            // Get the current HTTP_PROXY value
            EnvironmentVariableTarget targetHttpProxyEnvironmentVariable = EnvironmentVariableTarget.User;
            var httpProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpProxyVariable, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(httpProxyFromEnvironmentVariable))
            {
                targetHttpProxyEnvironmentVariable = EnvironmentVariableTarget.Machine;
                httpProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpProxyVariable, EnvironmentVariableTarget.Machine);
            }
            Log.Info().WriteLine("Previous {0} value: {1}", HttpProxyVariable, httpProxyFromEnvironmentVariable);
            var httpTarget = SetEnvironmentVariables(HttpProxyVariable, httpProxy, targetHttpProxyEnvironmentVariable);

            // Get the current HTTPS_PROXY value
            EnvironmentVariableTarget targetHttpsProxyEnvironmentVariable = EnvironmentVariableTarget.User;
            var httpsProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpsProxyVariable, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(httpsProxyFromEnvironmentVariable))
            {
                targetHttpsProxyEnvironmentVariable = EnvironmentVariableTarget.Machine;
                httpsProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpsProxyVariable, EnvironmentVariableTarget.Machine);
            }
            Log.Info().WriteLine("Previous {0} value: {1}", HttpsProxyVariable, httpsProxyFromEnvironmentVariable);
            var httpsTarget = SetEnvironmentVariables(HttpsProxyVariable, httpsProxy, targetHttpsProxyEnvironmentVariable);

            return Disposable.Create(() =>
            {
                Log.Info().WriteLine("Restoring the proxy environment variables.");
                SetEnvironmentVariables(HttpsProxyVariable, httpsProxyFromEnvironmentVariable, httpsTarget);
                SetEnvironmentVariables(HttpProxyVariable, httpProxyFromEnvironmentVariable, httpTarget);
            });
        }

        /// <summary>
        ///     Apply the environment variables, try machine and than user
        /// </summary>
        /// <param name="variable">string with the variable name</param>
        /// <param name="value">string with the value</param>
        /// <param name="target">EnvironmentVariableTarget where to set the variable</param>
        /// <returns>EnvironmentVariableTarget where the value was set</returns>
        private EnvironmentVariableTarget SetEnvironmentVariables(string variable, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine)
        {
            if (variable == null)
            {
                throw new ArgumentNullException(nameof(variable));
            }

            if (target == EnvironmentVariableTarget.Machine)
            {
                try
                {
                    Environment.SetEnvironmentVariable(variable, value, target);
                }
                catch (Exception ex)
                {
                    Log.Warn().WriteLine(ex, "Not able to set the variable {0} for {1}", variable, target);
                    target = EnvironmentVariableTarget.User;
                }
            }
            if (target == EnvironmentVariableTarget.User)
            {
                try
                {
                    Environment.SetEnvironmentVariable(variable, value, target);
                }
                catch (Exception ex)
                {
                    Log.Warn().WriteLine(ex, "Not able to set the variable {0} for {1}", variable, target);
                    target = EnvironmentVariableTarget.Process;
                }
            }
            if (target == EnvironmentVariableTarget.Process)
            {
                try
                {
                    Environment.SetEnvironmentVariable(variable, value, target);
                }
                catch (Exception ex)
                {
                    Log.Warn().WriteLine(ex, "Not able to set the variable {0} for {1}", variable, target);
                }
            }

            Log.Info().WriteLine("Set {0} to {1} for {2}", value, value, target);

            return target;
        }

        /// <summary>
        ///     Used for logging of request
        /// </summary>
        /// <param name="oSession">Session</param>
        private void OnResponseHeadersAvailable(Session oSession)
        {
            if (oSession?.ResponseHeaders?.HTTPResponseCode == null)
            {
                return;
            }
            if (oSession.ResponseHeaders.HTTPResponseCode < 400)
            {
                if (Log.IsVerboseEnabled())
                {
                    Log.Verbose().WriteLine("{0}|{1}", oSession.ResponseHeaders.HTTPResponseCode, oSession.fullUrl);
                }
                return;
            }
            Log.Error().WriteLine("{0}|{1}", oSession.ResponseHeaders.HTTPResponseCode, oSession.fullUrl);
        }

        /// <summary>
        ///     Used to enable the proxy authentication
        /// </summary>
        /// <param name="session"></param>
        private void OnBeforeRequest(Session session)
        {
            if (session == null)
            {
                return;
            }
            if (_fiddlerConfiguration.AutomaticallyAuthenticate)
            {
                // This enables the automatic authentication
                session["X-AutoAuth"] = "(default)";
            }
        }
    }
}