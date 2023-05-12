using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        public WinGetPage()
        {
            InitializeComponent();
            ViewModel.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ViewModel.SelectedIndex))
            {
                switch (ViewModel.SelectedIndex)
                {
                    // 搜索应用页面
                    case 0:
                        {
                            break;
                        }
                    // 已安装应用页面
                    case 1:
                        {
                            break;
                        }
                    // 可升级应用页面
                    case 2:
                        {
                            break;
                        }
                }
            }
        }
    }
}
