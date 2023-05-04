using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// 微软商店页面视图模型
    /// </summary>
    public sealed class StoreViewModel : ViewModelBase
    {
        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set
            {
                _useInsVisValue = value;
                OnPropertyChanged();
            }
        }

        // 了解应用具体的使用说明
        public IRelayCommand UseInstructionCommand => new RelayCommand(() =>
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.Instructions);
        });

        public StoreViewModel()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
        }

        public void OnNavigatedTo()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
        }
    }
}
