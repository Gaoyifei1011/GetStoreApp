using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 网络连接异常应用内通知
    /// </summary>
    public sealed partial class NetWorkErrorTip : TeachingTip
    {
        public NetWorkErrorTip()
        {
            InitializeComponent();
            Content = ResourceService.GetLocalized("Notification/NetWorkError");
        }
    }
}
