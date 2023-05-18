using GetStoreApp.Models.Dialogs.CommonDialogs.About;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Dialogs.About
{
    /// <summary>
    /// 桌面程序参数对话框视图模型
    /// </summary>
    public sealed class DesktopStartupArgsViewModel
    {
        public string SampleShort { get; } = @"GetStoreApp.exe ""https://www.microsoft.com/store/productId/9WZDNCRFJBMP""";

        public string SampleFull { get; } = @"GetStoreApp.exe -t ""pid"" -c ""rt"" ""9WZDNCRFJBMP""";

        public List<StartupArgsModel> DesktopStartupArgsList { get; } = new List<StartupArgsModel>
        {
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("Dialog/Type") ,Argument="-t; --type",IsRequired=ResourceService.GetLocalized("Dialog/No"),ArgumentContent=@"""url"",""pid"",""pfn"",""cid"""},
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("Dialog/Channel"),Argument="-c; --channel",IsRequired=ResourceService.GetLocalized("Dialog/No"),ArgumentContent=@"""wif"",""wis"",""rp"",""rt"""},
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("Dialog/Link"),Argument="-l; --link",IsRequired=ResourceService.GetLocalized("Dialog/Yes"),ArgumentContent=string.Format("[{0}]",ResourceService.GetLocalized("Dialog/LinkContent")) }
        };

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnCloseDialogClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((ExtendedContentDialog)button.Tag).Hide();
            }
        }
    }
}
