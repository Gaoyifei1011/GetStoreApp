using GetStoreApp.Models.Base;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：感谢用户控件视图模型
    /// </summary>
    public sealed class ThanksViewModel
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
    }
}
