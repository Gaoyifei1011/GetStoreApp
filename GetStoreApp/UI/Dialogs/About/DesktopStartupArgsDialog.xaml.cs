using GetStoreApp.Models.Dialogs.About;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 桌面程序参数对话框
    /// </summary>
    public sealed partial class DesktopStartupArgsDialog : ContentDialog
    {
        private List<StartupArgsModel> DesktopStartupArgsList { get; } = new List<StartupArgsModel>
        {
            new StartupArgsModel()
            {
                ArgumentName = ResourceService.GetLocalized("Dialog/Type") ,
                Argument = "-t; --type",
                IsRequired = ResourceService.GetLocalized("Dialog/No"),
                ArgumentContent = @"""url"",""pid"""
            },
            new StartupArgsModel()
            {
                ArgumentName = ResourceService.GetLocalized("Dialog/Channel"),
                Argument = "-c; --channel",
                IsRequired = ResourceService.GetLocalized("Dialog/No"),
                ArgumentContent = @"""wif"",""wis"",""rp"",""rt"""
            },
            new StartupArgsModel()
            {
                ArgumentName = ResourceService.GetLocalized("Dialog/Link"),
                Argument = "-l; --link",
                IsRequired = ResourceService.GetLocalized("Dialog/Yes"),
                ArgumentContent = ResourceService.GetLocalized("Dialog/LinkContent")
            }
        };

        public DesktopStartupArgsDialog()
        {
            InitializeComponent();
        }
    }
}
