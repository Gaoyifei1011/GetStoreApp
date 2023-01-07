using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    public sealed class ReferenceViewModel
    {
        //项目引用信息
        public Dictionary<string, string> ReferenceDict { get; } = new Dictionary<string, string>
        {
            {"Aira2","https://aria2.github.io" },
            {"HtmlAgilityPack","http://html-agility-pack.net" },
            {"Microsoft.Data.Sqlite.Core","https://docs.microsoft.com/dotnet/standard/data/sqlite" },
            {"Microsoft.Windows.CsWinRT","https://github.com/microsoft/cswinrt" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"SQLitePCLRaw.bundle_winsqlite3","https://github.com/ericsink/SQLitePCL.raw" },
        };
    }
}
