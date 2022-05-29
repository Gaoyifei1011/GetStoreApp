using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Home;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Home
{
    /// <summary>
    /// 请求控件对应的ViewModel
    /// The request control corresponds to the ViewModel
    /// </summary>
    public class RequestViewModel : ObservableRecipient
    {
        /// <summary>
        /// 从API获取信息时对应的国家/地区
        /// The country/region that corresponds to the information obtained from the API
        /// </summary>
        private string RegionCodeName { get; set; } = RegionService.RegionCodeName;

        /// <summary>
        /// 是否要过滤扩展名以“e”开头的文件
        /// Whether you want to filter files whose extensions begin with "e"
        /// </summary>
        private bool StartsWithEFilterValue { get; set; } = LinkFilterService.LinkFilterValue[0];

        /// <summary>
        /// 是否要过滤扩展名以“blockmap”开头的文件
        /// Whether you want to filter files whose extensions begin with "blockmap"
        /// </summary>
        private bool BlockMapFilterValue { get; set; } = LinkFilterService.LinkFilterValue[1];

        /// <summary>
        /// 样例标题
        /// Sample title
        /// </summary>
        private string SampleTitle { get; } = HomeViewModel.SampleTitle;

        /// <summary>
        /// 样例链接
        /// Sample link
        /// </summary>
        private string SampleLink { get; set; } = HomeViewModel.SampleLinkList[0];

        /// <summary>
        /// ComboBox选中的HomeType
        /// ComboBox selects homeType
        /// </summary>
        private HomeType _selectedType;

        public HomeType SelectedType
        {
            get { return _selectedType; }

            set { SetProperty(ref _selectedType, value); }
        }

        /// <summary>
        /// ComboBox选中的HomeChannel
        /// HomeChannel selected by ComboBox
        /// </summary>
        private HomeChannel _selectedChannel;

        public HomeChannel SelectedChannel
        {
            get { return _selectedChannel; }

            set { SetProperty(ref _selectedChannel, value); }
        }

        /// <summary>
        /// HomeLink的占位符文本
        /// Placeholder text for HomeLink
        /// </summary>
        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set { SetProperty(ref _linkPlaceHolderText, value); }
        }

        /// <summary>
        /// omeLink的文本
        /// HomeLink text
        /// </summary>
        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set { SetProperty(ref _linkText, value); }
        }

        /// <summary>
        /// HomeType ComboBox的下拉框选择发生变化的ICommand
        /// The drop-down box of the HomeType ComboBox selects ICommand for which the change
        /// </summary>
        private ICommand _typeSelectionChangedCommand;

        public ICommand TypeSelectionChangedCommand
        {
            get { return _typeSelectionChangedCommand; }

            set { SetProperty(ref _typeSelectionChangedCommand, value); }
        }

        /// <summary>
        /// 主页面“获取链接”按钮的ICommand
        /// ICommand of the main page "Get Link" button
        /// </summary>
        private ICommand _getLinksCommand;

        public ICommand GetLinksCommand
        {
            get { return _getLinksCommand; }

            set { SetProperty(ref _getLinksCommand, value); }
        }

        /// <summary>
        /// HomeType下拉框的数据源
        /// The data source for the HomeType drop-down box
        /// </summary>
        public IReadOnlyList<HomeType> TypeList = HomeViewModel.TypeList;

        /// <summary>
        /// HomeChannel下拉框的数据源
        /// Data source for the HomeChannel drop-down box
        /// </summary>
        public IReadOnlyList<HomeChannel> ChannelList = HomeViewModel.ChannelList;

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

            // 注册消息接收
            Messenger.Register<RequestViewModel, RegionMessage>(this, (requestViewModel, regionMessage) =>
            {
                requestViewModel.RegionCodeName = regionMessage.Value;
            });

            // 注册过滤状态消息
            Messenger.Register<RequestViewModel, StartsWithEFilterMessage>(this, (requestViewModel, startsWithEFilterMessage) =>
            {
                requestViewModel.StartsWithEFilterValue = startsWithEFilterMessage.Value;
            });

            Messenger.Register<RequestViewModel, BlockMapFilterMessage>(this, (requestViewModel, blockMapFilterService) =>
            {
                requestViewModel.BlockMapFilterValue = blockMapFilterService.Value;
            });
        }

        /// <summary>
        /// HomeType下拉框选项发生改变时修改对应的占位符文本内容
        /// Modify the corresponding placeholder text content when the HomeType drop-down box option changes
        /// </summary>
        private void SetPlaceHolderText()
        {
            // 选择的是URL(Link)
            if (SelectedType.InternalName == TypeList[0].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[0];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }

            // 选择的是ProductID
            else if (SelectedType.InternalName == TypeList[1].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[1];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }

            // 选择的是PackageFamilyName
            else if (SelectedType.InternalName == TypeList[2].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[2];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }

            // 选择的是CategoryID
            else if (SelectedType.InternalName == TypeList[3].InternalName)
            {
                SampleLink = HomeViewModel.SampleLinkList[3];
                LinkPlaceHolderText = SampleTitle + SampleLink;
            }

            // 其他异常选择时，清空所有内容
            else
            {
                SampleLink = string.Empty;
                LinkPlaceHolderText = string.Empty;
            }

            // 选择修改后清空输入的文本内容
            LinkText = string.Empty;
        }

        /// <summary>
        /// 点击“获取链接按钮”获取链接
        /// Click the "Get Link button" to get the link
        /// </summary>
        /// <returns></returns>
        private async Task GetLinksClickedAsync()
        {
            // 结果控件显示状态
            bool ResultControlVisable = false;

            // 解析得到的CategoryId
            string CategoryId = string.Empty;

            // 解析得到的信息列表
            List<ResultData> ResultDataList = new();

            // 输入文本为空时，使用占位符文本
            if (string.IsNullOrEmpty(LinkText))
            {
                LinkText = SampleLink;
            }

            // 设置获取数据时的相关控件状态
            Messenger.Send(new StatusBarStateMessage(2));

            // 生成请求的内容
            GenerateContentService generateContentService = new();
            string content = generateContentService.GenerateContent(SelectedType.InternalName, LinkText, SelectedChannel.InternalName, RegionCodeName);

            // 获取网页反馈回的原始数据
            HtmlRequestService htmlRequestService = new();
            HttpRequestData httpRequestData = await htmlRequestService.HttpRequestAsync(content);

            // 检查服务器返回获取的状态
            HtmlRequestStateService htmlRequestStateService = new();
            int state = htmlRequestStateService.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            if (state == 1 || state == 3)
            {
                ResultControlVisable = true;
            }

            // 成功状态下解析数据
            if (state == 3)
            {
                HtmlParseService htmlParseService = new(httpRequestData);

                CategoryId = htmlParseService.HtmlParseCID();

                ResultDataList = htmlParseService.HtmlParseLinks();

                // 按要求过滤列表内容
                if (StartsWithEFilterValue)
                {
                    ResultDataList.RemoveAll(item =>
                    item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                    item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                    );
                }

                if (BlockMapFilterValue)
                {
                    ResultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
                }
            }

            // 发送消息
            Messenger.Send(new StatusBarStateMessage(state));

            Messenger.Send(new ResultControlVisableMessage(ResultControlVisable));

            Messenger.Send(new ResultCategoryIdMessage(CategoryId));

            Messenger.Send(new ResultDataListMessage(ResultDataList));
        }
    }
}
