using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class DesktopStartupArgsViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public string SampleShort { get; } = @"GetStoreApp.exe ""https://www.microsoft.com/store/productId/9WZDNCRFJBMP""";

        public string SampleFull { get; } = @"GetStoreApp.exe -t ""pid"" -c ""rt"" ""9WZDNCRFJBMP""";

        public List<StartupArgsModel> StartupArgsList => new List<StartupArgsModel>
        {
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("/Dialog/Type") ,Argument="-t; --type",IsRequired=ResourceService.GetLocalized("/Dialog/No"),ArgumentContent=@"""url"",""pid"",""pfn"",""cid"""},
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("/Dialog/Channel"),Argument="-c; --channel",IsRequired=ResourceService.GetLocalized("/Dialog/No"),ArgumentContent=@"""wif"",""wis"",""rp"",""rt"""},
            new StartupArgsModel(){ArgumentName = ResourceService.GetLocalized("/Dialog/Link"),Argument="-l; --link",IsRequired=ResourceService.GetLocalized("/Dialog/Yes"),ArgumentContent=string.Format("[{0}]",ResourceService.GetLocalized("/Dialog/LinkContent")) }
        };

        // 关闭窗口
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });
    }
}
