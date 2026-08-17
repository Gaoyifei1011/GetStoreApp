using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 语言模型
    /// </summary>
    internal sealed partial class LanguageModel : INotifyPropertyChanged
    {
        internal KeyValuePair<string, string> LanguageInfo { get; set; }

        private bool _isChecked;

        internal bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
