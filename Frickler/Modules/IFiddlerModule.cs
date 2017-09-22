#region Usings

using Dapplo.Addons;

#endregion

namespace Frickler.Modules
{
    /// <summary>
    ///     The interface for the Fiddler module
    /// </summary>
    public interface IFiddlerModule : IStartupAction, IShutdownAction
    {
    }
}