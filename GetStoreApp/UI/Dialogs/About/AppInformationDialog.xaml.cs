using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框视图
    /// </summary>
    public sealed partial class AppInformationDialog : ExtendedContentDialog
    {
        public AppInformationDialog()
        {
            InitializeComponent();
            ViewModel.InitializeAppInformation();
        }
    }
}
