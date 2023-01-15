using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Controls
{
    /// <summary>
    /// 修改后的DropButton按钮，初始化完成后修改鼠标指针为链接选择状态
    /// </summary>
    public partial class ResultDropDownButton : DropDownButton
    {
        public ResultDropDownButton()
        {
            InitializeComponent();
        }

        public void ResultDropDownButtonLoaded(object sender, RoutedEventArgs args)
        {
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        }
    }
}
