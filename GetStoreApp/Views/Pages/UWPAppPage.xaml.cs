using GetStoreApp.Models.Controls.UWPApp;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// UWP 应用管理页面
    /// </summary>
    public sealed partial class UWPAppPage : Page, INotifyPropertyChanged
    {
        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private BreadModel AppInfoBreadModel { get; } = new BreadModel()
        {
            DisplayName = ResourceService.GetLocalized("UWPApp/AppInformation"),
            InternalName = "AppInformation"
        };

        public ObservableCollection<BreadModel> BreadDataList { get; } = new ObservableCollection<BreadModel>()
        {
            new BreadModel(){ DisplayName = ResourceService.GetLocalized("UWPApp/AppList"), InternalName = "AppList" },
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public UWPAppPage()
        {
            InitializeComponent();
            UWPAppFrame.Navigate(typeof(AppListPage), null);
        }

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        public void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            BreadModel breadItem = args.Item as BreadModel;
            if (BreadDataList.Count is 2)
            {
                if (breadItem is not null && breadItem.InternalName == BreadDataList[0].InternalName)
                {
                    BackToAppList();
                }
            }
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        public async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            AppListPage appListPage = UWPAppFrame.Content as AppListPage;
            if (appListPage.Content is not null)
            {
                appListPage.SearchText = SearchText;
                await appListPage.InitializeDataAsync(true);
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        public async void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            if (autoSuggestBox is not null && autoSuggestBox.Text == string.Empty)
            {
                AppListPage appListPage = UWPAppFrame.Content as AppListPage;
                if (appListPage.Content is not null)
                {
                    appListPage.SearchText = string.Empty;
                    await appListPage.InitializeDataAsync();
                }
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        public async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }

        /// <summary>
        /// 查看应用信息
        /// </summary>
        public void ShowAppInformation(Package packageInstance)
        {
            if (packageInstance is not null)
            {
                UWPAppFrame.Navigate(typeof(AppInfoPage), packageInstance, new SlideNavigationTransitionInfo()
                {
                    Effect = SlideNavigationTransitionEffect.FromRight
                });
                BreadDataList.Add(AppInfoBreadModel);
            }
        }

        /// <summary>
        /// 返回到应用信息页面
        /// </summary>
        public void BackToAppList()
        {
            if (UWPAppFrame.CanGoBack)
            {
                UWPAppFrame.GoBack();
                BreadDataList.Remove(AppInfoBreadModel);
            }
        }
    }
}
