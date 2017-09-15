using System.ComponentModel.Composition;
using Caliburn.Micro;
using Dapplo.CaliburnMicro;
using Frickler.Configuration;
using Frickler.Modules;

namespace Frickler.ViewModels
{
    /// <summary>
    /// The frickler ViewModel
    /// </summary>
    [Export(typeof(IShell))]
    public class FricklerViewModel : Screen, IShell
    {
        private readonly IFiddlerModule _fiddlerModule;

        /// <summary>
        /// Used from the View
        /// </summary>
        public IFiddlerConfiguration FiddlerConfiguration { get; }

        /// <summary>
        /// construct the ViewModel
        /// </summary>
        /// <param name="fiddlerConfiguration">IFiddlerConfiguration</param>
        /// <param name="fiddlerModule">IFiddlerModule</param>
        [ImportingConstructor]
        public FricklerViewModel(IFiddlerConfiguration fiddlerConfiguration, IFiddlerModule fiddlerModule)
        {
            _fiddlerModule = fiddlerModule;
            FiddlerConfiguration = fiddlerConfiguration;
        }

        /// <summary>
        /// Activate the ViewModel
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            _fiddlerModule.Startup();
        }

        /// <summary>
        /// Deactivate the ViewModel
        /// </summary>
        /// <param name="close"></param>
        protected override void OnDeactivate(bool close)
        {
            _fiddlerModule.Shutdown();
            base.OnDeactivate(close);
        }
    }
}
