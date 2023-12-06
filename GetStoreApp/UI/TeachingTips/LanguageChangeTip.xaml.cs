using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 语言设置修改应用内通知
    /// </summary>
    public sealed partial class LanguageChangeTip : TeachingTip
    {
        public LanguageChangeTip()
        {
            InitializeComponent();
            Content = ResourceService.GetLocalized("Notification/LanguageChange");
        }
    }
}
