using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.App;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class HistoryItemViewModel : ObservableRecipient
    {
        private readonly INavigationService _navigationService;

        private int HistoryItemValue { get; set; } = HistoryItemValueService.HistoryItemValue;

        private HistoryItemData _selectedHistoryItem;

        public HistoryItemData SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }

            set { SetProperty(ref _selectedHistoryItem, value); }
        }

        private IAsyncRelayCommand _viewAllCommand;

        public IAsyncRelayCommand ViewAllCommand
        {
            get { return _viewAllCommand; }

            set { SetProperty(ref _viewAllCommand, value); }
        }

        private ICommand _copyCommand;

        public ICommand CopyCommand
        {
            get { return _copyCommand; }

            set { SetProperty(ref _copyCommand, value); }
        }

        private IAsyncRelayCommand _fillinCommand;

        public IAsyncRelayCommand FillinCommand
        {
            get { return _fillinCommand; }

            set { SetProperty(ref _fillinCommand, value); }
        }

        public ObservableCollection<HistoryItemData> HistoryItemList { get; set; } = new ObservableCollection<HistoryItemData>();

        public HistoryItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ViewAllCommand = new AsyncRelayCommand(ViewAllAsync);

            FillinCommand = new AsyncRelayCommand(FillinAsync);

            CopyCommand = new RelayCommand<object>(async (param) => { await CopyAsync(param); });

            // List列表初始化，可以从数据库获得的列表中加载
            InitialList();

            Messenger.Register<HistoryItemViewModel, HistoryItemMessage>(this, async (historyItemViewModel, historyItemMessage) =>
            {
                await SuccessfullyRequestedUpdateListAsync(historyItemMessage.Value);
            });

            Messenger.Register<HistoryItemViewModel, HistoryItemValueMessage>(this, async (historyItemViewModel, historyItemValueMessage) =>
            {
                HistoryItemValue = historyItemValueMessage.Value;
                await SettingsChangedUpdateListAsync();
            });
        }

        // 从数据库中获取
        private void InitialList()
        {
            //TODO：从数据库中加载
        }

        // 成功获取数据后，更新历史记录
        private async Task SuccessfullyRequestedUpdateListAsync(HistoryItemData historyItemData)
        {
            int Position = 0;
            // 添加数据比对，如果比对成功，修改列表元素位置，不添加
            bool CheckResult = CheckRepeatElement(ref Position, historyItemData);

            // 如果存在相等的，删除原来的元素
            if (CheckResult)
            {
                HistoryItemList.RemoveAt(Position);
            }

            // 添加元素
            // 数量个数大于等于设置中的数值，删除最后一个并下移
            if (HistoryItemList.Count >= HistoryItemValue)
            {
                HistoryItemList.RemoveAt(HistoryItemList.Count - 1);
            }

            HistoryItemList.Insert(0, historyItemData);
            await Task.CompletedTask;
        }

        // 设置中的选项做出相应的调整后，如有需要，更新列表记录
        private async Task SettingsChangedUpdateListAsync()
        {
            // 当前元素个数大于设置中设定的值，删除元素到指定数目
            while (HistoryItemList.Count > HistoryItemValue)
            {
                HistoryItemList.RemoveAt(HistoryItemList.Count - 1);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 检查列表中是否有重复的元素，如果有，返回True和索引
        /// </summary>
        private bool CheckRepeatElement(ref int position, HistoryItemData historyItemData)
        {
            // 如果相等，修改索引，直接返回
            for (int i = 0; i < HistoryItemList.Count; i++)
            {
                if (historyItemData.HistoryItemKey == HistoryItemList[i].HistoryItemKey)
                {
                    position = i;
                    return true;
                }
            }

            // 没有重复的元素
            return false;
        }

        private async Task ViewAllAsync()
        {
            _navigationService.NavigateTo(typeof(HistoryViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 将选中的历史记录条目填入到选择控件中，然后点击“获取链接”即可获取
        /// </summary>
        private async Task FillinAsync()
        {
            if (SelectedHistoryItem == null) return;

            Messenger.Send(new FillinMessage(SelectedHistoryItem));
            await Task.CompletedTask;
        }

        private async Task CopyAsync(object sender)
        {
            if (sender is not string content) return;

            // 复制链接到剪贴板
            else if (content == "Link")
            {
                CopyPasteService.CopyStringToClicpBoard(SelectedHistoryItem.HistoryItemLink);
            }

            // 复制全部内容
            else if (content == "CopyAll")
            {
                string CopyContent = string.Format("{0}\n{1}\n{2}", SelectedHistoryItem.HistoryItemType.DisplayName, SelectedHistoryItem.HistoryItemChannel.DisplayName, SelectedHistoryItem.HistoryItemLink);
                CopyPasteService.CopyStringToClicpBoard(CopyContent);
            }

            await Task.CompletedTask;
        }
    }
}
