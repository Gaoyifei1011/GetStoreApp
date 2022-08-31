using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Home;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class RequestViewModel : ObservableRecipient
    {
        private IRegionService RegionService { get; } = IOCHelper.GetService<IRegionService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private IHistoryDBService HistoryDBService { get; } = IOCHelper.GetService<IHistoryDBService>();

        private ILinkFilterService LinkFilterService { get; } = IOCHelper.GetService<ILinkFilterService>();

        private bool BlockMapFilterValue { get; set; }

        private bool StartsWithEFilterValue { get; set; }

        public List<GetAppTypeModel> TypeList => ResourceService.TypeList;

        public List<GetAppChannelModel> ChannelList => ResourceService.ChannelList;

        public static IReadOnlyList<string> SampleLinkList { get; } = new List<string>
        {
            "https://www.microsoft.com/store/productId/9NSWSBXN8K03",
            "9NKSQGP7F2NH",
            "Microsoft.WindowsStore_8wekyb3d8bbwe",
            "d58c3a5f-ca63-4435-842c-7814b5ff91b7"
        };

        private string SampleTitle => ResourceService.GetLocalized("/Home/SampleTitle");

        private string SampleLink { get; set; }

        private int _selectedType = 0;

        public int SelectedType
        {
            get { return _selectedType; }

            set { SetProperty(ref _selectedType, value); }
        }

        private int _selectedChannel = 3;

        public int SelectedChannel
        {
            get { return _selectedChannel; }

            set { SetProperty(ref _selectedChannel, value); }
        }

        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set { SetProperty(ref _linkPlaceHolderText, value); }
        }

        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set { SetProperty(ref _linkText, value); }
        }

        public IAsyncRelayCommand TypeSelectCommand => new AsyncRelayCommand(SetPlaceHolderTextAsync);

        public IAsyncRelayCommand GetLinksCommand => new AsyncRelayCommand(GetLinksAsync);

        public RequestViewModel()
        {
            BlockMapFilterValue = LinkFilterService.BlockMapFilterValue;
            StartsWithEFilterValue = LinkFilterService.StartWithEFilterValue;

            SampleLink = SampleLinkList[0];

            LinkPlaceHolderText = SampleTitle + SampleLink;

            WeakReferenceMessenger.Default.Register<RequestViewModel, StartsWithEFilterMessage>(this, (requestViewModel, startsWithEFilterMessage) =>
            {
                requestViewModel.StartsWithEFilterValue = startsWithEFilterMessage.Value;
            });

            WeakReferenceMessenger.Default.Register<RequestViewModel, BlockMapFilterMessage>(this, (requestViewModel, blockMapFilterService) =>
            {
                requestViewModel.BlockMapFilterValue = blockMapFilterService.Value;
            });

            WeakReferenceMessenger.Default.Register<RequestViewModel, FillinMessage>(this, (requestViewModel, fillinMessage) =>
            {
                requestViewModel.SelectedType = TypeList.FindIndex(item => item.InternalName.Equals(fillinMessage.Value.HistoryType));
                requestViewModel.SelectedChannel = ChannelList.FindIndex(item => item.InternalName.Equals(fillinMessage.Value.HistoryChannel));
                requestViewModel.LinkText = fillinMessage.Value.HistoryLink;
            });
        }

        private async Task SetPlaceHolderTextAsync()
        {
            SampleLink = SampleLinkList[SelectedType];
            LinkPlaceHolderText = SampleTitle + SampleLink;

            LinkText = string.Empty;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        private async Task GetLinksAsync()
        {
            bool ResultControlVisable;

            string CategoryId = string.Empty;

            List<ResultModel> ResultDataList = new List<ResultModel>();

            if (string.IsNullOrEmpty(LinkText))
            {
                LinkText = SampleLink;
            }

            // 记录当前选定的选项和填入的内容
            int CurrentType = SelectedType;

            int CurrentChannel = SelectedChannel;

            string CurrentLink = LinkText;

            // 设置获取数据时的相关控件状态
            WeakReferenceMessenger.Default.Send(new StatusBarStateMessage(0));

            // 生成请求的内容
            GenerateContentService generateContentService = new GenerateContentService();
            string content = generateContentService.GenerateContent(
                TypeList[SelectedType].InternalName,
                LinkText,
                ChannelList[SelectedChannel].InternalName,
                RegionService.AppRegion.ISO2);

            // 获取网页反馈回的原始数据
            HtmlRequestService htmlRequestService = new HtmlRequestService();
            RequestModel httpRequestData = await htmlRequestService.HttpRequestAsync(content);

            // 检查服务器返回获取的状态
            HtmlRequestStateService htmlRequestStateService = new HtmlRequestStateService();
            int state = htmlRequestStateService.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            ResultControlVisable = state is 1 or 2;

            // 成功状态下解析数据，并更新相应的历史记录
            if (state == 1)
            {
                HtmlParseService htmlParseService = new HtmlParseService(httpRequestData);

                CategoryId = htmlParseService.HtmlParseCID();

                ResultDataList = htmlParseService.HtmlParseLinks();

                ResultListFilter(ref ResultDataList);

                await UpdateHistoryAsync(CurrentType, CurrentChannel, CurrentLink);

                WeakReferenceMessenger.Default.Send(new HistoryMessage(true));
            }

            // 发送消息
            WeakReferenceMessenger.Default.Send(new StatusBarStateMessage(state));

            WeakReferenceMessenger.Default.Send(new ResultControlVisableMessage(ResultControlVisable));

            WeakReferenceMessenger.Default.Send(new ResultCategoryIdMessage(CategoryId));

            WeakReferenceMessenger.Default.Send(new ResultDataListMessage(ResultDataList));
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容和数据库中的内容
        /// </summary>
        private async Task UpdateHistoryAsync(int currentType, int currentChannel, string currentLink)
        {
            // 拼接准备要生成唯一的MD5值的内容
            string Content = string.Format("{0} {1} {2}", TypeList[currentType].InternalName, ChannelList[currentChannel].InternalName, currentLink);

            // 添加时间戳
            long TimeStamp = GenerateTimeStamp();

            // 生成唯一的MD5值
            string UniqueKey = GenerateUniqueKey(Content);

            await HistoryDBService.AddAsync(new HistoryModel
            {
                CreateTimeStamp = TimeStamp,
                HistoryKey = UniqueKey,
                HistoryType = TypeList[currentType].InternalName,
                HistoryChannel = ChannelList[currentChannel].InternalName,
                HistoryLink = currentLink
            });
        }

        private long GenerateTimeStamp()
        {
            TimeSpan TimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(TimeSpan.TotalSeconds);
        }

        private string GenerateUniqueKey(string content)
        {
            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(content));

            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder str = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++) str.Append(data[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            // 返回十六进制字符串
            return str.ToString();
        }

        private void ResultListFilter(ref List<ResultModel> resultDataList)
        {
            // 按要求过滤列表内容
            if (StartsWithEFilterValue)
            {
                resultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (BlockMapFilterValue)
            {
                resultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
