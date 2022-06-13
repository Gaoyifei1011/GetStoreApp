using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class BackdropViewModel : ObservableRecipient
    {
        private string _selectedBackdrop = BackdropService.ApplicationBackdrop;

        public string SelectedBackdrop
        {
            get
            {
                return _selectedBackdrop;
            }

            set
            {
                SetProperty(ref _selectedBackdrop, value);

                BackdropHelper.CurrentBackdrop = _selectedBackdrop;
                BackdropHelper.SetBackdrop();
                BackdropService.SetBackdrop(_selectedBackdrop);
            }
        }

        public List<BackdropData> BackdropList { get; set; } = new List<BackdropData>();

        public BackdropViewModel()
        {
            InitialIzeBackdropList();
        }

        /// <summary>
        /// 系统版本号小于22000，不添加云母（Mica）
        /// </summary>
        private void InitialIzeBackdropList()
        {
            SystemInfoHelper systemInfoHelper = new SystemInfoHelper();

            ulong BuildNumber = systemInfoHelper.GetSystemVersion()["BuildNumber"];

            if (BuildNumber >= 22000)
            {
                BackdropList.Add(new BackdropData() { DisplayName = LanguageService.GetResources("/Settings/BackdropMica"), InternalName = "Mica" });
            }

            BackdropList.Add(new BackdropData() { DisplayName = LanguageService.GetResources("/Settings/BackdropArylic"), InternalName = "Acrylic" });
            BackdropList.Add(new BackdropData() { DisplayName = LanguageService.GetResources("/Settings/BackdropDefault"), InternalName = "Default" });
        }
    }
}
