using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Helpers.Root;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
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
        public IRelayCommand TopMostCommand => new RelayCommand<bool>(async (topMostValue) =>
        {
            await TopMostService.SetTopMostValueAsync(topMostValue);
            await TopMostService.SetAppTopMostAsync();
            TopMostValue = topMostValue;
        });

        public TopMostViewModel()
        {
            TopMostValue = TopMostService.TopMostValue;
        }
    }
}
