using System.ComponentModel;
using Dapplo.Language;

namespace Frickler.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Language("ContextMenu")]
    public interface IContextMenuTranslations : ILanguage, INotifyPropertyChanged
    {
        [DefaultValue("Exit")]
        string Exit { get; }
        [DefaultValue("Frickler")]
        string Title { get; }
    }
}
