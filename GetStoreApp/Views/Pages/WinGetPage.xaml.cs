using GetStoreApp.Models.Controls.WinGet;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page, INotifyPropertyChanged
    {
        public readonly object InstallingAppsObject = new object();

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

        public XamlUICommand CancelInstallCommand { get; } = new XamlUICommand();

        public ObservableCollection<InstallingAppsModel> InstallingAppsList = new ObservableCollection<InstallingAppsModel>();

        public Dictionary<string, CancellationTokenSource> InstallingStateDict = new Dictionary<string, CancellationTokenSource>();

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetPage()
        {
            InitializeComponent();

            CancelInstallCommand.ExecuteRequested += (sender, args) =>
            {
                string appId = args.Parameter as string;
                if (appId is not null)
                {
                    lock (InstallingAppsObject)
                    {
                        if (InstallingStateDict.TryGetValue(appId, out CancellationTokenSource tokenSource))
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                tokenSource.Cancel();
                                tokenSource.Dispose();
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        public bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted, bool needReverseValue)
        {
            bool result = isOfficialVersionExisted || isDevVersionExisted;
            if (needReverseValue)
            {
                return !result;
            }
            else
            {
                return result;
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
        /// 初始化 WinGet 程序包
        /// </summary>
        private void OnInitializeSuccessLoaded()
        {
            SearchApps.WinGetInstance = this;
            UpgradableApps.WinGetInstance = this;
        }
    }
}
