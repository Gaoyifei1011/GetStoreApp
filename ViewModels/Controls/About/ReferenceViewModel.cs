using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class ReferenceViewModel : ObservableRecipient
    {
        public Dictionary<string, string> ReferenceDict { get; } = new Dictionary<string, string>
        {
            {"Aira2","https://aria2.github.io/" },
            {"CommunityToolkit.Mvvm","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"CommunityToolkit.WinUI.UI.Controls.DataGrid" ,"https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/datagrid"},
            {"HtmlAgilityPack","http://html-agility-pack.net/" },
            {"Microsoft.Data.Sqlite","https://docs.microsoft.com/dotnet/standard/data/sqlite/" },
            {"Microsoft.Extensions.Hosting","https://dot.net/" },
            {"Microsoft.Windows.SDK.BuildTools","https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"Microsoft.Xaml.Behaviors.WinUI.Managed","https://github.com/Microsoft/XamlBehaviors" },
            {"NETStandard.Library","https://dotnet.microsoft.com/zh-cn/" },
            {"Template Studio","https://github.com/microsoft/TemplateStudio" },
            {"WinUIEx","https://dotmorten.github.io/WinUIEx" }
        };
    }
}
