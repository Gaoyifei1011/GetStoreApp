using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：项目引用用户控件视图模型
    /// </summary>
    public sealed class ReferenceViewModel
    {
        //项目引用信息
        public Dictionary<string, string> ReferenceDict { get; } = new Dictionary<string, string>
        {
            {"Aira2","https://aria2.github.io" },
            {"Microsoft.Data.Sqlite.Core","https://docs.microsoft.com/dotnet/standard/data/sqlite" },
            {"Microsoft.Windows.SDK.Contracts","https://aka.ms/WinSDKProjectURL" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"Mile.Xaml","https://github.com/ProjectMile/Mile.Xaml" },
            {"SQLitePCLRaw.core","https://github.com/ericsink/SQLitePCL.raw" },
            {"SQLitePCLRaw.provider.winsqlite3","https://github.com/ericsink/SQLitePCL.raw" },
        };
    }
}
