using System.ComponentModel.Composition;
using Dapplo.CaliburnMicro.Toasts.ViewModels;
using Dapplo.Frickler.Configuration;

namespace Dapplo.Frickler.Ui.ViewModels
{
    /// <summary>
    /// A toast which informs the user that the network settings have been changed
    /// </summary>
    [Export]
    public class NetworkSettingsChangedToastViewModel : ToastBaseViewModel
    {
        private readonly IFricklerTranslations _fricklerTranslations;

        /// <inheritdoc />
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
