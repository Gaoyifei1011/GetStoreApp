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
    /// <summary>
    /// 设置页面：历史记录设置用户控件视图模型
    /// </summary>
    public sealed class HistoryRecordViewModel : ViewModelBase
    {
        public List<HistoryLiteNumModel> HistoryLiteNumList { get; } = HistoryRecordService.HistoryLiteNumList;

        public List<HistoryJumpListNumModel> HistoryJumpListNumList { get; } = HistoryRecordService.HistoryJumpListNumList;

        private HistoryLiteNumModel _historyLiteItem = HistoryRecordService.HistoryLiteNum;

        public HistoryLiteNumModel HistoryLiteItem
        {
            get { return _historyLiteItem; }

            set
            {
                _historyLiteItem = value;
                OnPropertyChanged();
            }
        }

        private HistoryJumpListNumModel _historyJumpListItem = HistoryRecordService.HistoryJumpListNum;

        public HistoryJumpListNumModel HistoryJumpListItem
        {
            get { return _historyJumpListItem; }

            set
            {
                _historyJumpListItem = value;
                OnPropertyChanged();
            }
        }

        // 主页面“历史记录”显示数目修改
        public IRelayCommand HistoryLiteSelectCommand => new RelayCommand<string>(async (historyLiteIndex) =>
        {
            HistoryLiteItem = HistoryLiteNumList[Convert.ToInt32(historyLiteIndex)];
            await HistoryRecordService.SetHistoryLiteNumAsync(HistoryLiteItem);
            Messenger.Default.Send(HistoryLiteItem, MessageToken.HistoryLiteNum);
        });

        // 任务栏右键菜单列表“历史记录”显示数目修改
        public IRelayCommand HistoryJumpListSelectCommand => new RelayCommand<string>(async (historyJumpListIndex) =>
        {
            HistoryJumpListItem = HistoryJumpListNumList[Convert.ToInt32(historyJumpListIndex)];
            await HistoryRecordService.SetHistoryJumpListNumAsync(HistoryJumpListItem);
            await HistoryRecordService.UpdateHistoryJumpListAsync(HistoryJumpListItem);
        });
    }
}
