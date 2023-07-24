using GetStoreApp.Models.Controls.About;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：项目引用控件
    /// </summary>
    public sealed partial class ReferenceControl : Expander
    {
        //项目引用信息
        public List<KeyValuePairModel> ReferenceList { get; } = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel(){ Key = "Microsoft.Windows.CsWinRT",Value = "https://github.com/microsoft/cswinrt"},
            new KeyValuePairModel(){ Key = "Microsoft.WindowsAppSDK",Value = "https://github.com/microsoft/windowsappsdkt"},
            new KeyValuePairModel(){ Key = "Microsoft.WindowsPackageManager.ComInterop",Value = "https://github.com/microsoft/winget-cli"},
            new KeyValuePairModel(){ Key = "Mile.Aria2",Value = "https://github.com/ProjectMile/Mile.Aria2"},
        };

        public ReferenceControl()
        {
            InitializeComponent();
        }
    }
}
