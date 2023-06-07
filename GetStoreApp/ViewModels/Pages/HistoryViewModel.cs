using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// 历史记录页面数据模型
    /// </summary>
    public sealed class HistoryViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object HistoryDataListLock = new object();

        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        public ObservableCollection<HistoryModel> HistoryDataList { get; } = new ObservableCollection<HistoryModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        private bool _isHistoryEmpty = true;

        public bool IsHistoryEmpty
        {
            get { return _isHistoryEmpty; }

            set
            {
                _isHistoryEmpty = value;
                OnPropertyChanged();
            }
        }

        private bool _isHistoryEmptyAfterFilter = true;

        public bool IsHistoryEmptyAfterFilter
        {
            get { return _isHistoryEmptyAfterFilter; }

            set
            {
                _isHistoryEmptyAfterFilter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 时间排序的规则，false为递减排序，true为递增排序
        /// </summary>
        private bool _timeSortOrder = false;

        public bool TimeSortOrder
        {
            get { return _timeSortOrder; }

            set
            {
                _timeSortOrder = value;
                OnPropertyChanged();
            }
        }

        private string _typeFilter = "None";

        public string TypeFilter
        {
            get { return _typeFilter; }

            set
            {
                _typeFilter = value;
                OnPropertyChanged();
            }
        }

        private string _channelFilter = "None";

        public string ChannelFilter
        {
            get { return _channelFilter; }

            set
            {
                _channelFilter = value;
                OnPropertyChanged();
            }
        }

        // 填入指定项目的内容
        public XamlUICommand FillinCommand { get; } = new XamlUICommand();

        // 复制指定项目的内容
        public XamlUICommand CopyCommand { get; } = new XamlUICommand();

        /// <summary>
        /// 进入多选模式
        /// </summary>
        public void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
        }

        /// <summary>
        /// 显示浮出控件
        /// </summary>
        public void OnFlyoutShowClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 按时间进行排序
        /// </summary>
        public async void OnTimeSortClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                TimeSortOrder = Convert.ToBoolean(item.Tag);
                await GetHistoryDataListAsync();
            }
        }

        /// <summary>
        /// 按类型进行过滤
        /// </summary>
        public async void OnTypeFilterClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                TypeFilter = Convert.ToString(item.Tag);
                await GetHistoryDataListAsync();
            }
        }

        /// <summary>
        /// 按通道进行过滤
        /// </summary>
        public async void OnChannelFilterClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                ChannelFilter = Convert.ToString(item.Tag);
                await GetHistoryDataListAsync();
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetHistoryDataListAsync();
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        public void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 复制选定项目的内容
        /// </summary>
        public async void OnCopySelectedClicked(object sender, RoutedEventArgs args)
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected is true).ToList();

            if (SelectedHistoryDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            SelectedHistoryDataList.ForEach(selectedHistoryData =>
            {
                stringBuilder.AppendLine(string.Format("{0}\t{1}\t{2}",
                TypeList.Find(i => i.InternalName.Equals(selectedHistoryData.HistoryType)).DisplayName,
                ChannelList.Find(i => i.InternalName.Equals(selectedHistoryData.HistoryChannel)).DisplayName,
                selectedHistoryData.HistoryLink));
            });

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());

            new HistoryCopyNotification(true, true, SelectedHistoryDataList.Count).Show();
        }

        /// <summary>
        /// 删除选定的项目
        /// </summary>
        public async void OnDeleteClicked(object sender, RoutedEventArgs args)
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected is true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedHistoryDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog(DeleteArgs.History).ShowAsync();

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteResult = await HistoryXmlService.DeleteAsync(SelectedHistoryDataList);

                if (DeleteResult)
                {
                    // 确保线程安全
                    lock (HistoryDataListLock)
                    {
                        SelectedHistoryDataList.ForEach(historyItem => HistoryDataList.Remove(historyItem));
                    }
                }

                if (HistoryDataList.Count is 0)
                {
                    if (TypeFilter is "None" || ChannelFilter is "None")
                    {
                        IsHistoryEmpty = true;
                        IsHistoryEmptyAfterFilter = false;
                    }
                    else
                    {
                        IsHistoryEmpty = false;
                        IsHistoryEmptyAfterFilter = true;
                    }
                }
            }
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            HistoryModel historyItem = (HistoryModel)args.ClickedItem;
            int ClickedIndex = HistoryDataList.IndexOf(historyItem);

            lock (HistoryDataListLock)
            {
                HistoryDataList[ClickedIndex].IsSelected = !HistoryDataList[ClickedIndex].IsSelected;
            }
        }

        /// <summary>
        /// 导航到历史记录页面时，历史记录数据列表初始化，从数据库中存储的列表中加载
        /// </summary>
        public async void OnNavigatedTo()
        {
            await GetHistoryDataListAsync();
        }

        public HistoryViewModel()
        {
            FillinCommand.ExecuteRequested += (sender, args) =>
            {
                HistoryModel historyItem = args.Parameter as HistoryModel;
                if (historyItem is not null)
                {
                    NavigationService.NavigateTo(typeof(StorePage), new object[] { AppNaviagtionArgs.Store, historyItem.HistoryType, historyItem.HistoryChannel, historyItem.HistoryLink });
                }
            };

            CopyCommand.ExecuteRequested += (sender, args) =>
            {
                HistoryModel historyItem = (HistoryModel)args.Parameter;
                if (historyItem is not null)
                {
                    string copyContent = string.Format("{0}\t{1}\t{2}",
                        TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType)).DisplayName,
                        ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel)).DisplayName,
                        historyItem.HistoryLink);
                    CopyPasteHelper.CopyToClipBoard(copyContent);
                    new HistoryCopyNotification(true, false).Show();
                }
            };
        }

        /// <summary>
        /// 从数据库中加载数据
        /// </summary>
        private async Task GetHistoryDataListAsync()
        {
            (List<HistoryModel>, bool, bool) HistoryAllData = await HistoryXmlService.QueryAllAsync(TimeSortOrder, TypeFilter, ChannelFilter);

            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = HistoryAllData.Item1;
            // 数据库中的历史记录表是否为空
            IsHistoryEmpty = HistoryAllData.Item2;

            // 经过筛选后历史记录是否为空
            IsHistoryEmptyAfterFilter = HistoryAllData.Item3;

            // 保证线程安全
            lock (HistoryDataListLock)
            {
                HistoryDataList.Clear();
            }

            // 保证线程安全
            lock (HistoryDataListLock)
            {
                HistoryRawList.ForEach(HistoryDataList.Add);
            }
        }
    }
}
