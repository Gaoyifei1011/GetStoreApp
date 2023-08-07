using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 以管理员身份运行应用时提示框
    /// </summary>
    public sealed partial class ElevatedRunningDialog : ContentDialog
    {
        public ElevatedRunningDialog()
        {
            InitializeComponent();
        }
    }
}
