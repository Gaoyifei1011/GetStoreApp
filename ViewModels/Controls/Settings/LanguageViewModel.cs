using GetStoreApp.Core.Models;
using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class LanguageViewModel : ObservableObject
    {
        // 语言设置
        private string _selectedLanguage = LanguageSelectorService.PriLangCodeName;

        public string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                LanguageSelectorService.SetLanguage(value);
                SetProperty(ref _selectedLanguage, value);
            }
        }

        // 语言列表
        public List<LanguageModel> LanguageList = LanguageSelectorService.LanguageList;
    }
}
