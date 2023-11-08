using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// UWP 应用管理页面
    /// </summary>
    public sealed partial class UWPAppPage : Page, INotifyPropertyChanged
    {
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<DictionaryEntry> BreadCollection { get; } = new ObservableCollection<DictionaryEntry>()
        {
            new DictionaryEntry(){ Key = ResourceService.GetLocalized("UWPApp/AppList"), Value = "AppList" },
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public UWPAppPage()
        {
            InitializeComponent();
        }

        #region 第一部分：应用管理页面——挂载的事件

        /// <summary>
        /// 打开设置
        /// </summary>
        private async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            DictionaryEntry breadItem = (DictionaryEntry)args.Item;
            if (BreadCollection.Count is 2)
            {
                if (breadItem.Value.Equals(BreadCollection[0].Value))
                {
                    BackToAppList();
                }
            }
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                AppList.SearchText = SearchText;
                AppList.InitializeData(true);
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            if (autoSuggestBox is not null && autoSuggestBox.Text.Equals(string.Empty))
            {
                AppList.SearchText = string.Empty;
                AppList.InitializeData();
            }
        }

        #endregion 第一部分：应用管理页面——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 查看应用信息
        /// </summary>
        public void ShowAppInformation(Dictionary<string, object> packageInstance)
        {
            if (packageInstance is not null)
            {
                AppInfo.InitializeAppInfo(packageInstance);
                BreadCollection.Add(new DictionaryEntry()
                {
                    Key = ResourceService.GetLocalized("UWPApp/AppInformation"),
                    Value = "AppInformation"
                });
            }
        }

        /// <summary>
        /// 返回到应用信息页面
        /// </summary>
        public void BackToAppList()
        {
            if (BreadCollection.Count is 2)
            {
                BreadCollection.RemoveAt(1);
            }
        }

        /// <summary>
        /// 确定当前选择的索引是否为目标控件
        /// </summary>
        private Visibility IsCurrentControl(int selectedIndex, int index)
        {
            return selectedIndex.Equals(index) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
