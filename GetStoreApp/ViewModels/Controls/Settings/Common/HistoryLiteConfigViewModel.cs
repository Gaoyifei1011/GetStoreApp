using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using System;
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
        public IRelayCommand HistoryLiteSelectCommand => new RelayCommand<string>(async (historyLiteIndex) =>
        {
            HistoryLiteItem = HistoryLiteNumList[Convert.ToInt32(historyLiteIndex)];
            await HistoryLiteNumService.SetHistoryLiteNumAsync(HistoryLiteItem);
            Messenger.Default.Send(HistoryLiteItem, MessageToken.HistoryLiteNum);
        });
    }
}
