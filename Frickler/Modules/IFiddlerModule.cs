namespace Frickler.Modules
{
    /// <summary>
    /// The interface for the Fiddler module
    /// </summary>
    public interface IFiddlerModule
    {

        /// <summary>
        /// Start fiddler
        /// </summary>
        void Startup();

        /// <summary>
        /// Shutdown fiddler
        /// </summary>
        void Shutdown();
    }
}