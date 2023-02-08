using Windows.Foundation.Collections;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：项目引用用户控件视图模型
    /// </summary>
    public sealed class ReferenceViewModel
    {
        //项目引用信息
        public StringMap ReferenceDict { get; } = new StringMap
        {
            {"Aira2","https://aria2.github.io" },
            {"Microsoft.Data.Sqlite.Core","https://docs.microsoft.com/dotnet/standard/data/sqlite" },
            {"Microsoft.Windows.SDK.BuildTools*","https://aka.ms/WinSDKProjectURL" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"SQLitePCLRaw.core","https://github.com/ericsink/SQLitePCL.raw" },
            {"SQLitePCLRaw.provider.winsqlite3","https://github.com/ericsink/SQLitePCL.raw" },
            {"System.Memory*","https://dot.net" },
        };
    }
}
