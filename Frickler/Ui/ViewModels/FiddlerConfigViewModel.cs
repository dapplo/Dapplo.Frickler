using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Frickler.Configuration;
using Frickler.Modules;

namespace Frickler.Ui.ViewModels
{
    /// <summary>
    /// The fiddler config ViewModel
    /// </summary>
    [Export(typeof(IConfigScreen))]
    public sealed class FiddlerConfigViewModel : SimpleConfigScreen
    {
        private readonly IFiddlerModule _fiddlerModule;

        /// <summary>
        /// Used from the View
        /// </summary>
        public IFiddlerConfiguration FiddlerConfiguration { get; }
        /// <summary>
        /// Used from the View
        /// </summary>
        public IFricklerTranslations FricklerTranslations { get; }

        /// <summary>
        /// construct the ViewModel
        /// </summary>
        /// <param name="fiddlerConfiguration">IFiddlerConfiguration</param>
        /// <param name="fiddlerModule">IFiddlerModule</param>
        /// <param name="fricklerTranslations">IFricklerTranslations</param>
        [ImportingConstructor]
        public FiddlerConfigViewModel(IFiddlerConfiguration fiddlerConfiguration, IFiddlerModule fiddlerModule, IFricklerTranslations fricklerTranslations)
        {
            _fiddlerModule = fiddlerModule;
            Id = "C_Fiddler";
            FiddlerConfiguration = fiddlerConfiguration;
            FricklerTranslations = fricklerTranslations;
            fricklerTranslations.CreateDisplayNameBinding(this, nameof(IFricklerTranslations.Title));
        }

        /// <summary>
        /// Activate the ViewModel
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            _fiddlerModule.Shutdown();
        }

        /// <summary>
        /// Deactivate the ViewModel
        /// </summary>
        /// <param name="close"></param>
        protected override void OnDeactivate(bool close)
        {
            _fiddlerModule.Start();
            base.OnDeactivate(close);
        }
    }
}
