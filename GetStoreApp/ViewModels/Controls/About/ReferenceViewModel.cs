using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class ReferenceViewModel : ObservableRecipient
    {
        //项目引用信息
        public Dictionary<string, string> ReferenceDict => new Dictionary<string, string>
        {
            {"Aira2","https://aria2.github.io" },
            {"Aria2.NET","https://github.com/rogerfar/Aria2.NET" },
            {"CommunityToolkit.Mvvm","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"CommunityToolkit.WinUI.Notifications","https://www.nuget.org/packages/CommunityToolkit.WinUI.Notifications" },
            {"CommunityToolkit.WinUI.UI.Behaviors","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"CommunityToolkit.WinUI.UI.Controls","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"H.NotifyIcon.WinUI","https://github.com/HavenDV/H.NotifyIcon" },
            {"HtmlAgilityPack","http://html-agility-pack.net" },
            {"Microsoft.Data.Sqlite","https://docs.microsoft.com/dotnet/standard/data/sqlite" },
            {"Microsoft.Extensions.Hosting","https://www.nuget.org/packages/Microsoft.Extensions.Hosting" },
            {"Microsoft.Windows.CsWin32","https://github.com/Microsoft/CsWin32" },
            {"Microsoft.Windows.SDK.BuildTools","https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"Microsoft.Xaml.Behaviors.WinUI.Managed","https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.WinUI.Managed" },
            {"PInvoke.SHCore","https://github.com/dotnet/pinvoke" },
            {"System.Management","https://www.nuget.org/packages/System.Management" },
            {"Template Studio","https://github.com/microsoft/TemplateStudio" },
            {"WinUIEx","https://dotmorten.github.io/WinUIEx" }
        };
    }
}
