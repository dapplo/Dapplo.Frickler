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
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using Dapplo.Addons;
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
        private const string HttpsProxyVariable = "HTTPS_PROXY";
        private const string HttpProxyVariable = "HTTP_PROXY";
        private static readonly LogSource Log = new LogSource();
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
            if (_fiddlerConfiguration.AutomaticallyAuthenticate)
            {
                FiddlerApplication.BeforeRequest += OnBeforeRequest;
            }
            // This makes logging of error possible
            FiddlerApplication.ResponseHeadersAvailable += OnResponseHeadersAvailable;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            FiddlerApplication.BeforeRequest -= OnBeforeRequest;
            FiddlerApplication.ResponseHeadersAvailable -= OnResponseHeadersAvailable;
            if (!FiddlerApplication.IsStarted())
            {
                return;
            }
            _proxyDisposable?.Dispose();
            FiddlerApplication.oProxy.Detach();
        }

        /// <summary>
        ///     Setup the proxy environment variables
        /// </summary>
        /// <returns>IDisposable which returns the environment variables back to the original value when disposed</returns>
        private IDisposable ModifyProxyEnvironment()
        {
            // These are the new values
            var httpProxy = $"http://localhost:{_fiddlerConfiguration.ProxyPort}/";
            var httpsProxy = $"http://localhost:{_fiddlerConfiguration.ProxyPort}/";

            // Get the current HTTP_PROXY value
            var httpProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpProxyVariable, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(httpProxyFromEnvironmentVariable))
            {
                httpProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpProxyVariable, EnvironmentVariableTarget.User);
            }
            var httpTarget = SetEnvironmentVariables(HttpProxyVariable, httpProxy);

            // Get the current HTTP_PROXY value
            var httpsProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpsProxyVariable, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(httpsProxyFromEnvironmentVariable))
            {
                httpsProxyFromEnvironmentVariable = Environment.GetEnvironmentVariable(HttpsProxyVariable, EnvironmentVariableTarget.User);
            }
            var httpsTarget = SetEnvironmentVariables(HttpsProxyVariable, httpsProxy);

            return Disposable.Create(() =>
            {
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
                    return target;
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
                    return target;
                }
                catch (Exception ex)
                {
                    Log.Warn().WriteLine(ex, "Not able to set the variable {0} for {1}", variable, target);
                    target = EnvironmentVariableTarget.Process;
                }
            }
            Environment.SetEnvironmentVariable(variable, value, target);
            return target;
        }

        /// <summary>
        ///     Used for logging of request
        /// </summary>
        /// <param name="oSession">Session</param>
        private void OnResponseHeadersAvailable(Session oSession)
        {
            if (oSession.ResponseHeaders.HTTPResponseCode < 400)
            {
                Log.Verbose().WriteLine("{0}|{1}", oSession.ResponseHeaders.HTTPResponseCode, oSession.fullUrl);
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
            if (_fiddlerConfiguration.AutomaticallyAuthenticate)
            {
                // This enables the automatic authentication
                session["X-AutoAuth"] = "(default)";
            }
        }
    }
}