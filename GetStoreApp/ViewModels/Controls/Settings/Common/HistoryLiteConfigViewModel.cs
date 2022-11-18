using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class HistoryLiteConfigViewModel : ViewModelBase
    {
        public List<HistoryLiteNumModel> HistoryLiteNumList => HistoryLiteNumService.HistoryLiteNumList;

        private HistoryLiteNumModel _historyLiteItem;

        public HistoryLiteNumModel HistoryLiteItem
        {
            get { return _historyLiteItem; }

            set
            {
                _historyLiteItem = value;
                OnPropertyChanged();
            }
        }

        // 主页面“历史记录”显示数目修改
        public IRelayCommand HistoryLiteItemSelectCommand => new RelayCommand(async () =>
        {
            await HistoryLiteNumService.SetHistoryLiteNumAsync(HistoryLiteItem);
            WeakReferenceMessenger.Default.Send(new HistoryLiteNumMessage(HistoryLiteItem));
        });

        public HistoryLiteConfigViewModel()
        {
            HistoryLiteItem = HistoryLiteNumService.HistoryLiteNum;
        }
    }
}
