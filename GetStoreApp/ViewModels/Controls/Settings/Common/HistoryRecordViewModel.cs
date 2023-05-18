using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

        /// <summary>
        /// 微软商店页面“历史记录”显示数目修改
        /// </summary>
        public async void OnHistoryLiteSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                HistoryLiteItem = HistoryLiteNumList[Convert.ToInt32(item.Tag)];
                await HistoryRecordService.SetHistoryLiteNumAsync(HistoryLiteItem);
                Messenger.Default.Send(HistoryLiteItem, MessageToken.HistoryLiteNum);
            }
        }

        /// <summary>
        /// 任务栏右键菜单列表“历史记录”显示数目修改
        /// </summary>
        public async void OnHistoryJumpListSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                HistoryJumpListItem = HistoryJumpListNumList[Convert.ToInt32(item.Tag)];
                await HistoryRecordService.SetHistoryJumpListNumAsync(HistoryJumpListItem);
                await HistoryRecordService.UpdateHistoryJumpListAsync(HistoryJumpListItem);
            }
        }
    }
}
