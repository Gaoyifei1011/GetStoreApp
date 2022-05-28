using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Models;
using GetStoreApp.Services.Home;
using GetStoreApp.ViewModels.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class RequestViewModel : ObservableRecipient
    {
        // 样例标题
        private string SampleTitle { get; } = HomeViewModel.SampleTitle;

        private string SampleLink { get; set; } = HomeViewModel.SampleLinkList[0];

        // ComboBox选中的HomeType
        private HomeTypeModel _selectedType;

        public HomeTypeModel SelectedType
        {
            get { return _selectedType; }

            set { SetProperty(ref _selectedType, value); }
        }

        // ComboBox选中的HomeChannel
        private HomeChannelModel _selectedChannel;

        public HomeChannelModel SelectedChannel
        {
            get { return _selectedChannel; }

            set { SetProperty(ref _selectedChannel, value); }
        }

        // HomeLink的占位符文本
        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set { SetProperty(ref _linkPlaceHolderText, value); }
        }

        // HomeLink的文本
        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set { SetProperty(ref _linkText, value); }
        }

        // HomeType ComboBox的下拉框选择改变ICommand实现
        private ICommand _typeSelectionChangedCommand;

        public ICommand TypeSelectionChangedCommand
        {
            get { return _typeSelectionChangedCommand; }

            set { SetProperty(ref _typeSelectionChangedCommand, value); }
        }

        // 主页面“获取链接”按钮的ICommand实现
        private ICommand _getLinksCommand;

        public ICommand GetLinksCommand
        {
            get { return _getLinksCommand; }

            set { SetProperty(ref _getLinksCommand, value); }
        }

        public IReadOnlyList<HomeTypeModel> TypeList = HomeViewModel.TypeList;

        public IReadOnlyList<HomeChannelModel> ChannelList = HomeViewModel.ChannelList;

        public RequestViewModel()
        {
            // 初始化HomeLink的占位符文本
            LinkPlaceHolderText = SampleTitle + SampleLink;

            // 响应HomeType的SelectionChanged事件
            TypeSelectionChangedCommand = new RelayCommand(SetPlaceHolderText);

            // 响应“获取链接”按钮的点击事件
            GetLinksCommand = new RelayCommand(async () => { await GetLinksClickedAsync(); });

            // 初始化最初选中的HomeType
            SelectedType = TypeList[0];

            // 初始化最初选中的HomeChannel
            SelectedChannel = ChannelList[3];
        }

        private void SetPlaceHolderText()
        {
            if (SelectedType.InternalName == TypeList[0].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[0];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else if (SelectedType.InternalName == TypeList[1].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[1];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else if (SelectedType.InternalName == TypeList[2].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[2];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else if (SelectedType.InternalName == TypeList[3].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[3];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }
            else
            {
                // 当MainType发生改变时，清空填入的链接内容
                SampleLink = string.Empty;
                LinkPlaceHolderText = string.Empty;
            }
            LinkText = string.Empty;
        }

        // 点击“获取链接按钮”获取链接
        private async Task GetLinksClickedAsync()
        {
            // 输入文本为空时，使用占位符文本
            if (string.IsNullOrEmpty(LinkText))
            {
                LinkText = SampleLink;
            }

            // 获取链接
            using GetLinksService getLinksService = new GetLinksService();
            // 获取数据
            HttpRequestDataModel httpRequestData = await getLinksService.RequestDataAsync(SelectedType.InternalName, LinkText, SelectedChannel.InternalName);

            // 解析数据
            getLinksService.ParseData(httpRequestData);
        }
    }
}