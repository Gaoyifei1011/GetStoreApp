using GetStoreApp.Models.Base;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：项目引用用户控件视图模型
    /// </summary>
    public sealed class ReferenceViewModel
    {
        //项目引用信息
        public List<KeyValuePairModel> ReferenceList { get; } = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel(){ Key = "Microsoft.Windows.CsWinRT",Value = "https://github.com/microsoft/cswinrt"},
            new KeyValuePairModel(){ Key = "Microsoft.WindowsAppSDK",Value = "https://github.com/microsoft/windowsappsdkt"},
            new KeyValuePairModel(){ Key = "Microsoft.WindowsPackageManager.ComInterop",Value = "https://github.com/microsoft/winget-cli"},
            new KeyValuePairModel(){ Key = "Mile.Aria2",Value = "https://github.com/ProjectMile/Mile.Aria2"},
        };
    }
}
