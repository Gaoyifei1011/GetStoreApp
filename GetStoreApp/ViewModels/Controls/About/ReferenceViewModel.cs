using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class ReferenceViewModel : ObservableRecipient
    {
        public Dictionary<string, string> ReferenceDict { get; } = new Dictionary<string, string>
        {
            {"Aira2","https://aria2.github.io" },
            {"CommunityToolkit.Mvvm","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"CommunityToolkit.WinUI.UI.Controls.DataGrid" ,"https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/datagrid"},
            {"CommunityToolkit.WinUI.Notifications","https://www.nuget.org/packages/CommunityToolkit.WinUI.Notifications" },
            {"HtmlAgilityPack","http://html-agility-pack.net" },
            {"Microsoft.Data.Sqlite","https://docs.microsoft.com/dotnet/standard/data/sqlite" },
            {"Microsoft.Extensions.Hosting","https://www.nuget.org/packages/Microsoft.Extensions.Hosting" },
            {"Microsoft.Windows.CsWin32","https://github.com/Microsoft/CsWin32" },
            {"Microsoft.Windows.CsWinRT","https://github.com/microsoft/cswinrt" },
            {"Microsoft.Windows.SDK.BuildTools","https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"Microsoft.Xaml.Behaviors.WinUI.Managed","https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.WinUI.Managed" },
            {"NETStandard.Library","https://www.nuget.org/packages/NETStandard.Library" },
            {"Newtonsoft.Json" ,"https://www.newtonsoft.com/json"},
            {"PInvoke.SHCore","https://github.com/dotnet/pinvoke" },
            {"System.Management","https://www.nuget.org/packages/System.Management" },
            {"Template Studio","https://github.com/microsoft/TemplateStudio" },
            {"WinUIEx","https://dotmorten.github.io/WinUIEx" }
        };
    }
}
