using GetStoreApp.Helpers.Controls.Store;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 微软商店页面：请求控件
    /// </summary>
    public sealed partial class RequestControl : Grid, INotifyPropertyChanged
    {
        private HistoryLiteControl historyLiteInstance;

        private StatusBarControl statusBarInstance;

        private ResultControl resultInstance;

        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        public static List<string> SampleLinkList { get; } = new List<string>
        {
            "https://www.microsoft.com/store/productId/9NSWSBXN8K03",
            "9NKSQGP7F2NH",
            "Microsoft.WindowsStore_8wekyb3d8bbwe",
            "d58c3a5f-ca63-4435-842c-7814b5ff91b7"
        };

        private string SampleTitle { get; } = ResourceService.GetLocalized("Store/SampleTitle");

        private string SampleLink { get; set; }

        private TypeModel _selectedType;

        public TypeModel SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        private ChannelModel _selectedChannel;

        public ChannelModel SelectedChannel
        {
            get { return _selectedChannel; }

            set
            {
                _selectedChannel = value;
                OnPropertyChanged();
            }
        }

        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set
            {
                _linkPlaceHolderText = value;
                OnPropertyChanged();
            }
        }

        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set
            {
                _linkText = value;
                OnPropertyChanged();
            }
        }

        private bool _isGettingLinks;

        public bool IsGettingLinks
        {
            get { return _isGettingLinks; }

            set
            {
                _isGettingLinks = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RequestControl()
        {
            InitializeComponent();
            SelectedType = Convert.ToInt32(DesktopLaunchService.LaunchArgs["TypeName"]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(DesktopLaunchService.LaunchArgs["TypeName"])];

            SelectedChannel = Convert.ToInt32(DesktopLaunchService.LaunchArgs["ChannelName"]) is -1 ? ChannelList[3] : ChannelList[Convert.ToInt32(DesktopLaunchService.LaunchArgs["ChannelName"])];

            LinkText = DesktopLaunchService.LaunchArgs["Link"] is null ? string.Empty : (string)DesktopLaunchService.LaunchArgs["Link"];

            SampleLink = SampleLinkList[0];

            LinkPlaceHolderText = SampleTitle + SampleLink;

            IsGettingLinks = false;
        }

        public bool IsTypeItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        public bool IsChannelItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// 类型修改选择后修改样例文本
        /// </summary>
        public void OnTypeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedType = TypeList[Convert.ToInt32(item.Tag)];
                SampleLink = SampleLinkList[TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName)];
                LinkPlaceHolderText = SampleTitle + SampleLink;

                LinkText = string.Empty;
            }
        }

        /// <summary>
        /// 通道选择修改
        /// </summary>
        public void OnChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedChannel = ChannelList[Convert.ToInt32(item.Tag)];
            }
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await GetLinksAsync();
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public async void OnGetLinksClicked(object sender, RoutedEventArgs args)
        {
            await GetLinksAsync();
        }

        /// <summary>
        /// 初始化微软商店页面其他控件实例
        /// </summary>
        public void InitializeStorePageControl(HistoryLiteControl historyLiteControl, StatusBarControl statusBarControl, ResultControl resultControl)
        {
            historyLiteInstance = historyLiteControl;
            statusBarInstance = statusBarControl;
            resultInstance = resultControl;
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        private async Task GetLinksAsync()
        {
            bool ResultControlVisable;

            string CategoryId = string.Empty;

            IsGettingLinks = true;

            List<ResultModel> ResultDataList = new List<ResultModel>();

            if (string.IsNullOrEmpty(LinkText))
            {
                LinkText = SampleLink;
            }

            // 记录当前选定的选项和填入的内容
            int CurrentType = TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName);

            int CurrentChannel = ChannelList.FindIndex(item => item.InternalName == SelectedChannel.InternalName);

            string CurrentLink = LinkText;

            // 设置获取数据时的相关控件状态
            statusBarInstance.SetControlState(0);

            // 生成请求的内容
            string generateContent = GenerateContentHelper.GenerateRequestContent(
                SelectedType.InternalName,
                LinkText,
                SelectedChannel.InternalName);

            // 获取网页反馈回的原始数据
            RequestModel httpRequestData = await HtmlRequestHelper.HttpRequestAsync(generateContent);

            // 检查服务器返回获取的状态
            int state = HtmlRequestStateHelper.CheckRequestState(httpRequestData);

            // 设置结果控件的显示状态
            ResultControlVisable = state is 1 or 2;

            IsGettingLinks = false;

            // 成功状态下解析数据
            if (state is 1)
            {
                HtmlParseHelper.InitializeParseData(httpRequestData);

                CategoryId = HtmlParseHelper.HtmlParseCID();

                ResultDataList = HtmlParseHelper.HtmlParseLinks();

                ResultListFilter(ref ResultDataList);
            }

            // 显示结果
            statusBarInstance.SetControlState(state);
            resultInstance.ResultControlVisable = ResultControlVisable;
            resultInstance.CategoryId = CategoryId;
            resultInstance.ResultDataList.Clear();
            foreach (ResultModel resultItem in ResultDataList)
            {
                resultItem.IsSelected = false;
                resultInstance.ResultDataList.Add(resultItem);
                await Task.Delay(1);
            }

            // 成功状态下更新历史记录
            if (state is 1)
            {
                await UpdateHistoryAsync(CurrentType, CurrentChannel, CurrentLink);

                await UpdateTaskbarJumpListAsync(CurrentType, CurrentChannel, CurrentLink);

                await historyLiteInstance.GetHistoryLiteDataListAsync();
            }
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
        /// </summary>
        private async Task UpdateHistoryAsync(int currentType, int currentChannel, string currentLink)
        {
            // 添加时间戳
            long TimeStamp = GenerateTimeStamp();

            await HistoryXmlService.AddAsync(new HistoryModel
            {
                CreateTimeStamp = TimeStamp,
                HistoryKey = HashAlgorithmHelper.GenerateHistoryKey(TypeList[currentType].InternalName, ChannelList[currentChannel].InternalName, currentLink),
                HistoryType = TypeList[currentType].InternalName,
                HistoryChannel = ChannelList[currentChannel].InternalName,
                HistoryLink = currentLink
            });
        }

        /// <summary>
        /// 更新应用任务栏跳转列表内容
        /// </summary>
        private async Task UpdateTaskbarJumpListAsync(int currentType, int currentChannel, string currentLink)
        {
            if (Program.ApplicationRoot.TaskbarJumpList is not null)
            {
                int isDuplicatedIndex = -1;

                for (int index = 0; index < Program.ApplicationRoot.TaskbarJumpList.Items.Count; index++)
                {
                    JumpListItem item = Program.ApplicationRoot.TaskbarJumpList.Items[index];
                    if (item.DisplayName == currentLink)
                    {
                        isDuplicatedIndex = index;
                    }
                }

                // 无重复元素，直接添加
                if (isDuplicatedIndex is -1)
                {
                    if (HistoryRecordService.HistoryJumpListNum.SelectedValue is not "Unlimited")
                    {
                        int count = Convert.ToInt32(HistoryRecordService.HistoryJumpListNum.SelectedValue);

                        while (Program.ApplicationRoot.TaskbarJumpList.Items.Count >= count)
                        {
                            Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(0);
                        }
                    }
                    JumpListItem jumpListItem = JumpListItem.CreateWithArguments(string.Format("JumpList {0} {1} {2}", TypeList[currentType].ShortName, ChannelList[currentChannel].ShortName, currentLink), currentLink);
                    jumpListItem.GroupName = ResourceService.GetLocalized("Window/JumpListGroupName");
                    jumpListItem.Logo = new Uri("ms-appx:///Assets/Images/Jumplist.png");
                    Program.ApplicationRoot.TaskbarJumpList.Items.Add(jumpListItem);
                }
                // 有重复元素，移动到最后一位
                else
                {
                    Program.ApplicationRoot.TaskbarJumpList.Items.Add(Program.ApplicationRoot.TaskbarJumpList.Items[isDuplicatedIndex]);
                    Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(isDuplicatedIndex);
                }

                await Program.ApplicationRoot.TaskbarJumpList.SaveAsync();
            }
        }

        /// <summary>
        /// 按设置选项设置的内容过滤列表
        /// </summary>
        private void ResultListFilter(ref List<ResultModel> resultDataList)
        {
            if (LinkFilterService.StartWithEFilterValue)
            {
                resultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (LinkFilterService.BlockMapFilterValue)
            {
                resultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        private long GenerateTimeStamp()
        {
            TimeSpan TimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(TimeSpan.TotalSeconds);
        }
    }
}
