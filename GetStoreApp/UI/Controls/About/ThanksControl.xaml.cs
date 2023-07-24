using GetStoreApp.Models.Controls.About;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：感谢用户控件
    /// </summary>
    public sealed partial class ThanksControl : Expander
    {
        //项目感谢者信息
        public List<KeyValuePairModel> ThanksList { get; } = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel(){ Key = "AndromedaMelody",Value = "https://github.com/AndromedaMelody" },
            new KeyValuePairModel(){ Key = "cnbluefire",Value = "https://github.com/cnbluefire" },
            new KeyValuePairModel(){ Key = "飞翔",Value = "https://fionlen.azurewebsites.net" },
            new KeyValuePairModel(){ Key = "MouriNaruto",Value = "https://github.com/MouriNaruto" },
            new KeyValuePairModel(){ Key = "TaylorShi",Value = "https://github.com/TaylorShi" },
            new KeyValuePairModel(){ Key = "wherewhere",Value = "https://github.com/wherewhere" },
        };

        public ThanksControl()
        {
            InitializeComponent();
        }
    }
}
