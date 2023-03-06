using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：感谢用户控件视图模型
    /// </summary>
    public sealed class ThanksViewModel
    {
        public Dictionary<string, string> ThanksDict { get; } = new Dictionary<string, string>
        {
            {"AndromedaMelody","https://github.com/AndromedaMelody" },
            {"cnbluefire","https://github.com/cnbluefire" },
            {"飞翔","https://fionlen.azurewebsites.net"},
            {"MouriNaruto","https://github.com/MouriNaruto" },
            {"TaylorShi","https://github.com/TaylorShi" }
        };
    }
}
