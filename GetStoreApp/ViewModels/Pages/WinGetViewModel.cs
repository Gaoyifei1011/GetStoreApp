using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// WinGet 程序包页面数据模型
    /// </summary>
    public sealed class WinGetViewModel : ViewModelBase
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

        // 取消安装
        public XamlUICommand CancelInstallCommand { get; } = new XamlUICommand();

        public ObservableCollection<InstallingAppsModel> InstallingAppsList = new ObservableCollection<InstallingAppsModel>();

        public Dictionary<string, CancellationTokenSource> InstallingStateDict = new Dictionary<string, CancellationTokenSource>();

        public WinGetViewModel()
        {
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
    }
}
