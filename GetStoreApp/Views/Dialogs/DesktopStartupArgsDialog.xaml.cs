using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 桌面程序参数对话框
    /// </summary>
    public sealed partial class DesktopStartupArgsDialog : ContentDialog
    {
        private readonly string ChannelString = ResourceService.GetLocalized("Dialog/Channel");
        private readonly string LinkContentString = ResourceService.GetLocalized("Dialog/LinkContent");
        private readonly string LinkString = ResourceService.GetLocalized("Dialog/Link");
        private readonly string NoString = ResourceService.GetLocalized("Dialog/No");
        private readonly string TypeString = ResourceService.GetLocalized("Dialog/Type");
        private readonly string YesString = ResourceService.GetLocalized("Dialog/Yes");

        private List<StartupArgsModel> DesktopStartupArgsList { get; } = [];

        public DesktopStartupArgsDialog()
        {
            InitializeComponent();

            DesktopStartupArgsList.Add(new StartupArgsModel()
            {
                ArgumentName = TypeString,
                Argument = "-t; --type",
                IsRequired = NoString,
                ArgumentContent = @"""url"",""pid"""
            });
            DesktopStartupArgsList.Add(new StartupArgsModel()
            {
                ArgumentName = ChannelString,
                Argument = "-c; --channel",
                IsRequired = NoString,
                ArgumentContent = @"""wif"",""wis"",""rp"",""rt"""
            });
            DesktopStartupArgsList.Add(new StartupArgsModel()
            {
                ArgumentName = LinkString,
                Argument = "-l; --link",
                IsRequired = YesString,
                ArgumentContent = LinkContentString
            });
        }
    }
}
