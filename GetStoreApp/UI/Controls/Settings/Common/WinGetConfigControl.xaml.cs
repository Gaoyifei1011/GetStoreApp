using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// WinGet 程序包设置控件视图
    /// </summary>
    public sealed partial class WinGetConfigControl : UserControl
    {
        public WinGetConfigControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 判断两个版本是否共同存在
        /// </summary>
        public bool IsBothVersionExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted && isDevVersionExisted;
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        public bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted || isDevVersionExisted;
        }
    }
}
