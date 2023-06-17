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
            {"Microsoft.Windows.CsWinRT","https://github.com/microsoft/cswinrt" },
            {"Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk" },
            {"Microsoft.WindowsPackageManager.ComInterop","https://github.com/microsoft/winget-cl" },
            {"Mile.Aria2","https://github.com/ProjectMile/Mile.Aria2" },
        };
    }
}
