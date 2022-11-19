using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public sealed class HistoryLiteConfigViewModel : ViewModelBase
    {
        public List<HistoryLiteNumModel> HistoryLiteNumList { get; } = HistoryLiteNumService.HistoryLiteNumList;

        private HistoryLiteNumModel _historyLiteItem = HistoryLiteNumService.HistoryLiteNum;

        public HistoryLiteNumModel HistoryLiteItem
        {
            get { return _historyLiteItem; }

            set
            {
                _historyLiteItem = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 主页面“历史记录”显示数目修改
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                await HistoryLiteNumService.SetHistoryLiteNumAsync(HistoryLiteItem);
                Messenger.Default.Send(HistoryLiteItem, MessageToken.HistoryLiteNum);
            }
        }
    }
}
