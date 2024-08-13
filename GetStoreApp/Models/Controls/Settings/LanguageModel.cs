using System.Collections;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.Settings
{
    /// <summary>
    /// 语言模型
    /// </summary>
    public sealed partial class LanguageModel : INotifyPropertyChanged
    {
        public DictionaryEntry LangaugeInfo { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
