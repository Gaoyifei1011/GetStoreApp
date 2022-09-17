using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class TopMostViewModel : ObservableRecipient
    {
        private ITopMostService TopMostService { get; } = IOCHelper.GetService<ITopMostService>();

        private bool _topMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set { SetProperty(ref _topMostValue, value); }
        }

        // 是否开启应用窗口置顶
        public IAsyncRelayCommand TopMostCommand => new AsyncRelayCommand<bool>(async
                (param) =>
        {
            await TopMostService.SetTopMostValueAsync(param);
            await TopMostService.SetAppTopMostAsync();
            TopMostValue = param;
        });

        public TopMostViewModel()
        {
            TopMostValue = TopMostService.TopMostValue;
        }
    }
}
