using System.ComponentModel;
using Dapplo.Language;

namespace Frickler.Configuration
{
    [Language("Frickler")]
    public interface IFricklerTranslations : ILanguage
    {
        [DefaultValue("Automatically authenticate")]
        string AutomaticallyAutomaticallyAuthenticate { get; }

        [DefaultValue("Proxy Port")]
        string ProxyPort { get; }
    }
}
