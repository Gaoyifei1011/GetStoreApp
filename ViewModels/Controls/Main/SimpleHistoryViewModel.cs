using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Core.Models;
using GetStoreApp.Services.Settings;
using GetStoreApp.Services.Shell;
using GetStoreApp.Views;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace GetStoreApp.ViewModels.Controls.Main
{
    public class SimpleHistoryViewModel : ObservableObject
    {
        // 设置主页面历史记录的最多显示个数
        private int SimpleHisDataListMaxSize = SimpleHistoryItemSettings.SimpleHisItemValue;

        public ObservableCollection<HistoryDisplayDataModel> SimpleHistoryDataList = new ObservableCollection<HistoryDisplayDataModel>();

        // ComboBox选中的MainType
        private MainTypeModel SelectedMainType;

        // ComboBox选中的MainChannel
        private MainChannelModel SelectedMainChannel;

        // Main_Link的占位符文本
        private string MainLinkText;

        // Main_His_Links按钮的名称显示
        private string _mainHisGetLinksContent;

        public string MainHisGetLinksContent
        {
            get { return _mainHisGetLinksContent; }

            set { SetProperty(ref _mainHisGetLinksContent, value); }
        }

        // 主页面“查看更多”按钮的ICommand实现
        private ICommand _mainHisVMCommand;

        public ICommand MainHisVMCommand
        {
            get { return _mainHisVMCommand; }

            set { SetProperty(ref _mainHisVMCommand, value); }
        }

        // 主页面历史记录“获取链接”按钮的ICommand实现
        private ICommand _mainHisGetLinksCommand;

        public ICommand MainHisGetLinksCommand
        {
            get { return _mainHisGetLinksCommand; }

            set { SetProperty(ref _mainHisGetLinksCommand, value); }
        }

        public SimpleHistoryViewModel()
        {
            MainHisGetLinksContent = LanguageSettings.GetResources("Main_History_GetLinks");

            // 响应“查看更多”按钮的点击事件
            MainHisVMCommand = new RelayCommand(MainHisVM_Clicked);

            MainHisGetLinksCommand = new RelayCommand<object>(
                async (param) =>
                {
                    await MainHisGetLinks_ClickedAsync(param);
                });

            Messenger.Default.Register(this, "SelectedHisItemValue", (int obj) => { SimpleHisDataListMaxSize = obj; });

            Messenger.Default.Register(this, "SelectedMainType", (MainTypeModel obj) => { SelectedMainType = obj; });

            Messenger.Default.Register(this, "SelectedMainChannel", (MainChannelModel obj) => { SelectedMainChannel = obj; });

            Messenger.Default.Register(this, "MainLinkText", (string obj) => { MainLinkText = obj; });

            Messenger.Default.Register(this, "UpdateHistory", async (bool obj) => { if (obj) { await UpdateHistoryAsync(); } });
        }

        // 点击更多按钮切换到“历史记录”页面
        private void MainHisVM_Clicked()
        {
            NavigationService.Navigate(typeof(HistoryPage), null, new DrillInNavigationTransitionInfo());
        }

        // 点击“获取链接按钮”获取链接
        private async Task MainHisGetLinks_ClickedAsync(object sender)
        {
            HistoryDisplayDataModel item = sender as HistoryDisplayDataModel;

            //TODO: Only for test——未来替换为获取网页API信息
            ContentDialog dialog = new ContentDialog
            {
                Title = "Test Get Links Button",
                Content = string.Format("Main_Type:{0}\nMain_Channel:{1}\nMain_PlaceHolderText:{2}", item.TypeName, item.ChannelName, item.LinkName),
                PrimaryButtonText = "测试",
                CloseButtonText = "关闭"
            };
            await dialog.ShowAsync();
            // end
        }

        // 点击获取后更新所有历史记录数据
        private async Task UpdateHistoryAsync()
        {
            // 添加一个判断，该数据是否来自于主页面历史记录的点击，如果是，不更新历史记录
            // 主页面（Mini）历史记录添加数据：For Test
            if (SimpleHistoryDataList.Count == 0)
            {
                // 数量个数小于指定的队列长度，添加
                SimpleHistoryDataList.Add(new HistoryDisplayDataModel
                    (
                    TypeName: SelectedMainType.DisplayName,
                    TypeInternalName: SelectedMainType.InternalName,
                    ChannelName: SelectedMainChannel.DisplayName,
                    ChannelInternalName: SelectedMainChannel.InternalName,
                    LinkName: MainLinkText
                    ));
            }
            else if (SimpleHistoryDataList.Count > 0 && SimpleHistoryDataList.Count < SimpleHisDataListMaxSize)
            {
                SimpleHistoryDataList.Insert(0, new HistoryDisplayDataModel
                    (
                    TypeName: SelectedMainType.DisplayName,
                    TypeInternalName: SelectedMainType.InternalName,
                    ChannelName: SelectedMainChannel.DisplayName,
                    ChannelInternalName: SelectedMainChannel.InternalName,
                    LinkName: MainLinkText
                    ));
            }
            else
            {
                // 数量个数等于队列长度，删除最后一个并下移
                SimpleHistoryDataList.RemoveAt(SimpleHistoryDataList.Count - 1);
                SimpleHistoryDataList.Insert(0, new HistoryDisplayDataModel
                    (
                    TypeName: SelectedMainType.DisplayName,
                    TypeInternalName: SelectedMainType.InternalName,
                    ChannelName: SelectedMainChannel.DisplayName,
                    ChannelInternalName: SelectedMainChannel.InternalName,
                    LinkName: MainLinkText
                    ));
            }

            //TODO 更新数据库记录数据
            await Task.CompletedTask;
        }
    }
}
