using Windows.Foundation.Collections;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：感谢用户控件视图模型
    /// </summary>
    public sealed class ThanksViewModel
    {
        public StringMap ThanksDict { get; } = new StringMap
        {
            {"AndromedaMelody","https://github.com/AndromedaMelody" },
            {"飞翔","https://fionlen.azurewebsites.net"},
            {"MouriNaruto","https://github.com/MouriNaruto" },
            {"TaylorShi","https://github.com/TaylorShi" }
        };
    }
}
