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
            { "CommandLineParser","https://github.com/commandlineparser/commandline"},
            {"CommunityToolkit.Mvvm","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"CommunityToolkit.WinUI.Notifications","https://www.nuget.org/packages/CommunityToolkit.WinUI.Notifications" },
            {"CommunityToolkit.WinUI.UI.Behaviors","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"CommunityToolkit.WinUI.UI.Controls.Core","https://github.com/CommunityToolkit/WindowsCommunityToolkit" },
            {"H.NotifyIcon.WinUI","https://github.com/HavenDV/H.NotifyIcon" },
            {"HtmlAgilityPack","http://html-agility-pack.net" },
            {"Microsoft.Data.Sqlite.Core","https://docs.microsoft.com/dotnet/standard/data/sqlite" },
            {"Microsoft.Extensions.Hosting","https://www.nuget.org/packages/Microsoft.Extensions.Hosting" },
            {"Microsoft.Windows.CsWin32","https://github.com/Microsoft/CsWin32" },
            {"Microsoft.Windows.CsWinRT","https://github.com/microsoft/cswinrt" },
            {"Microsoft.Windows.SDK.BuildTools","https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools" },
            {"Microsoft.Windows.SDK.Contracts","https://aka.ms/WinSDKProjectURL"},
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"Microsoft.Xaml.Behaviors.WinUI.Managed","https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.WinUI.Managed" },
            {"Microsoft-Windows10-APICodePack-Shell","https://github.com/bau-global/Windows-API-Code-Pack-1.1"},
            {"PInvoke.User32","https://github.com/dotnet/pinvoke" },
            {"SQLitePCLRaw.bundle_winsqlite3","https://github.com/ericsink/SQLitePCL.raw" },
            {"System.Management","https://www.nuget.org/packages/System.Management" },
            {"Template Studio","https://github.com/microsoft/TemplateStudio" },
            {"WinUIEx","https://dotmorten.github.io/WinUIEx" }
        };
    }
}
