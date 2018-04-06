using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Toasts.ViewModels;
using Dapplo.Frickler.Configuration;

namespace Dapplo.Frickler.Ui.ViewModels
{
    [Export]
    public class NetworkSettingsChangedToastViewModel : ToastBaseViewModel
    {
        private readonly IFricklerTranslations _fricklerTranslations;

        [ImportingConstructor]
        public NetworkSettingsChangedToastViewModel(IFricklerTranslations fricklerTranslations)
        {
            _fricklerTranslations = fricklerTranslations;
        }

        /// <summary>
        /// This contains the message for the ViewModel
        /// </summary>
        public string Message => _fricklerTranslations.NetworkSettingsChanged;

    }
}
