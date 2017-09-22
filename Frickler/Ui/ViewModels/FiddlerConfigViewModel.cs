#region Usings

using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Frickler.Configuration;
using Frickler.Modules;

#endregion

namespace Frickler.Ui.ViewModels
{
    /// <summary>
    ///     The fiddler config ViewModel
    /// </summary>
    [Export(typeof(IConfigScreen))]
    public sealed class FiddlerConfigViewModel : SimpleConfigScreen
    {
        private readonly IFiddlerModule _fiddlerModule;

        /// <summary>
        ///     Used from the View
        /// </summary>
        public IFiddlerConfiguration FiddlerConfiguration { get; }

        /// <summary>
        ///     Used from the View
        /// </summary>
        public IFricklerTranslations FricklerTranslations { get; }

        /// <summary>
        ///     construct the ViewModel
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

        /// <inheritdoc />
        public override void Commit()
        {
            _fiddlerModule.Shutdown();
            base.Commit();
            _fiddlerModule.Start();
        }
    }
}