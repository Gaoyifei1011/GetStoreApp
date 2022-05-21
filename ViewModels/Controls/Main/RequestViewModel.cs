using GalaSoft.MvvmLight.Messaging;
using GetStoreApp.Core.Models;
using GetStoreApp.Services.Main;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Main
{
    public class RequestViewModel : ObservableObject
    {
        private string RegionCodeName = RegionSettings.RegionCodeName;

        // 样例标题
        private string _sampleTitle = LanguageSettings.GetResources("/MainResources/LinkExample");

        public string SampleTitle
        {
            get { return _sampleTitle; }
            set { _sampleTitle = value; }
        }

        // 样例链接
        private string _sampleLink = "https://www.microsoft.com/store/productId/9NSWSBXN8K03";

        public string SampleLink
        {
            get { return _sampleLink; }
            set { _sampleLink = value; }
        }

        // ComboBox选中的MainType
        private MainTypeModel _selectedMainType;

        public MainTypeModel SelectedMainType
        {
            get { return _selectedMainType; }

            set { SetProperty(ref _selectedMainType, value); }
        }

        // ComboBox选中的MainChannel
        private MainChannelModel _selectedMainChannel;

        public MainChannelModel SelectedMainChannel
        {
            get { return _selectedMainChannel; }

            set { SetProperty(ref _selectedMainChannel, value); }
        }

        // Main_Link的占位符文本
        private string _mainLinkPHText;

        public string MainLinkPHText
        {
            get { return _mainLinkPHText; }

            set { SetProperty(ref _mainLinkPHText, value); }
        }

        // MainType ComboBox的下拉框选择改变ICommand实现
        private ICommand _mainTypeSCCommand;

        public ICommand MainTypeSCCommand
        {
            get { return _mainTypeSCCommand; }

            set { SetProperty(ref _mainTypeSCCommand, value); }
        }

        // 主页面“获取链接”按钮的ICommand实现
        private ICommand _mainGetLinksCommand;

        public ICommand MainGetLinksCommand
        {
            get { return _mainGetLinksCommand; }

            set { SetProperty(ref _mainGetLinksCommand, value); }
        }

        // Main_Link的文本
        private string _mainLinkText;

        public string MainLinkText
        {
            get { return _mainLinkText; }

            set { SetProperty(ref _mainLinkText, value); }
        }

        public IReadOnlyList<MainTypeModel> MainTypeList = MainViewModel.MainTypeList;

        public IReadOnlyList<MainChannelModel> MainChannelList = MainViewModel.MainChannelList;

        public RequestViewModel()
        {
            // 初始化Main_Link的占位符文本
            MainLinkPHText = SampleTitle + SampleLink;

            // 响应MainType的SelectionChanged事件
            MainTypeSCCommand = new RelayCommand(SetPlaceHolderText);

            // 响应“获取链接”按钮的点击事件
            MainGetLinksCommand = new RelayCommand(async () => { await MainGetLinks_ClickedAsync(); });

            // 初始化最初选中的MainType
            SelectedMainType = MainTypeList[0];

            // 初始化最初选中的MainChannel
            SelectedMainChannel = MainChannelList[2];

            Messenger.Default.Register(this, "SelectedRegion", (string obj) => { RegionCodeName = obj; });
        }

        // 设置Main_Link的占位符文本
        private void SetPlaceHolderText()
        {
            if (SelectedMainType.InternalName == MainTypeList[0].InternalName)
            {
                SampleLink = "https://www.microsoft.com/store/productId/9NSWSBXN8K03";
                MainLinkPHText = SampleTitle + SampleLink;
            }
            else if (SelectedMainType.InternalName == MainTypeList[1].InternalName)
            {
                SampleLink = "9NKSQGP7F2NH";
                MainLinkPHText = SampleTitle + SampleLink;
            }
            else if (SelectedMainType.InternalName == MainTypeList[2].InternalName)
            {
                SampleLink = "Microsoft.WindowsStore_8wekyb3d8bbwe";
                MainLinkPHText = SampleTitle + SampleLink;
            }
            else if (SelectedMainType.InternalName == MainTypeList[3].InternalName)
            {
                SampleLink = "d58c3a5f-ca63-4435-842c-7814b5ff91b7";
                MainLinkPHText = SampleTitle + SampleLink;
            }
            // 当MainType发生改变时，清空填入的链接内容
            MainLinkText = string.Empty;
        }

        // 点击“获取链接按钮”获取链接
        private async Task MainGetLinks_ClickedAsync()
        {
            // 输入文本为空时，使用占位符文本
            if (string.IsNullOrEmpty(MainLinkText))
            {
                MainLinkText = SampleLink;
            }

            // 获取数据时的相关控件状态
            GettingLinksState();

            // 获取数据
            HtmlRequestService htmlRequestService = new HtmlRequestService();

            // 生成请求的内容
            string content = htmlRequestService.GenerateContent(type: SelectedMainType.InternalName, url: MainLinkText, ring: SelectedMainChannel.InternalName, language: RegionCodeName);
            // 通过Post请求获得网页中的数据
            HttpRequestDataModel HttpRequestData = await htmlRequestService.HttpRequestAsync(content);

            // 正常返回，解析标签内容
            if (HttpRequestData.RequestId == 0)
            {
                HtmlParseService htmlParseService = new HtmlParseService(HttpRequestData);
                bool flag = htmlParseService.IsSuccessfulRequest();

                // 返回成功的结果
                Messenger.Default.Send(true, "MainResult");

                // 服务器返回有链接的数据
                if (flag)
                {
                    // 链接获取完成后的相关控件状态，通过网页API反馈的结果来输入展示状态的参数
                    GetLinkFinishedState(3);

                    // 解析文本内容
                    Messenger.Default.Send(htmlParseService.HtmlParseCID(), "MainCategoryId");

                    // 解析获取到的数据
                    Messenger.Default.Send(htmlParseService.HtmlParseLinks(), "ResultDataList");

                    // 结果列表中获取到的条目数量
                    Messenger.Default.Send(true, "MainResultCount");

                    // 更新所有历史记录数据，成功时更新
                    Messenger.Default.Send(SelectedMainType, "SelectedMainType");
                    Messenger.Default.Send(SelectedMainChannel, "SelectedMainChannel");
                    Messenger.Default.Send(MainLinkText, "MainLinkText");
                    Messenger.Default.Send(true, "UpdateHistory");
                }
                // 服务器没有返回带有链接的数据
                else
                {
                    Messenger.Default.Send(true, "MainResult");
                    // 链接获取完成后的相关控件状态，通过网页API反馈的结果来输入展示状态的参数
                    GetLinkFinishedState(1);
                }
            }
            // 从服务器获取数据的时候发生了错误
            else
            {
                // 返回失败的结果
                Messenger.Default.Send(false, "MainResult");

                // 链接获取完成后的相关控件状态，通过网页API反馈的结果来输入展示状态的参数
                GetLinkFinishedState(0);

                //TODO:在详细信息按钮中查询具体异常
            }
        }

        // 获取链接时的相关控件状态
        private void GettingLinksState()
        {
            // 设置Main_Status_Image图标：提示状态
            Messenger.Default.Send(2, "MainStatImage");

            // 点击按钮获取网页信息后，设置Main_Status_Info文本：正在获取中
            Messenger.Default.Send("Main_Status_Info/Get", "MainStatInfoText");

            // 显示Main_Status_Progressring圆环动画
            Messenger.Default.Send(true, "MainStatPrRingVisValue");

            // Main_Status_Progressring圆环动画未激活时，激活圆环
            Messenger.Default.Send(true, "MainStatPrRingActValue");
        }

        // 链接获取完成后的相关控件状态
        private void GetLinkFinishedState(int state)
        {
            // 设置Main_Status_Image图标，根据传入的值来选择
            Messenger.Default.Send(state, "MainStatImage");

            switch (state)
            {
                case 0:
                    {
                        // 设置Main_Status_Info文本：获取失败
                        Messenger.Default.Send("Main_Status_Info/Error", "MainStatInfoText");
                        break;
                    }
                case 1:
                    {
                        // 设置Main_Status_Info文本：获取发生了异常，警示状态
                        Messenger.Default.Send("Main_Status_Info/Warning", "MainStatInfoText");
                        break;
                    }
                case 2:
                    {
                        break;
                    }
                case 3:
                    {
                        // 设置Main_Status_Info文本：获取成功
                        Messenger.Default.Send("Main_Status_Info/Success", "MainStatInfoText");
                        break;
                    }
                default:
                    {
                        // 设置 文本：获取失败
                        Messenger.Default.Send("Main_Status_Info/Error", "MainStatInfoText");
                        break;
                    }
            }

            // 关闭圆环显示
            Messenger.Default.Send(false, "MainStatPrRingActValue");
        }
    }
}
