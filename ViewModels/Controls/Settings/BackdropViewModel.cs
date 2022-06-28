using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class BackdropViewModel : ObservableRecipient
    {
        private string _selectedBackdrop = BackdropService.ApplicationBackdrop;

        public string SelectedBackdrop
        {
            get { return _selectedBackdrop; }

            set { SetProperty(ref _selectedBackdrop, value); }
        }

        public IAsyncRelayCommand BackdropSelectCommand { get; set; }

        public List<BackdropModel> BackdropList { get; set; } = new List<BackdropModel>();

        public BackdropViewModel()
        {
            InitialIzeBackdropList();

            BackdropSelectCommand = new AsyncRelayCommand(BackdropSelectAsync);
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
                BackdropList.Add(new BackdropModel { DisplayName = LanguageService.GetResources("/Settings/BackdropMica"), InternalName = "Mica" });
            }

            BackdropList.Add(new BackdropModel { DisplayName = LanguageService.GetResources("/Settings/BackdropArylic"), InternalName = "Acrylic" });
            BackdropList.Add(new BackdropModel { DisplayName = LanguageService.GetResources("/Settings/BackdropDefault"), InternalName = "Default" });
        }

        /// <summary>
        /// 设置背景色
        /// </summary>
        private async Task BackdropSelectAsync()
        {
            BackdropHelper.CurrentBackdrop = SelectedBackdrop;
            BackdropHelper.SetBackdrop();
            BackdropService.SetBackdrop(SelectedBackdrop);
            await Task.CompletedTask;
        }
    }
}
