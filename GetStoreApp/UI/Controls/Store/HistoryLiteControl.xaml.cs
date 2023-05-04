using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 微软商店页面：部分历史记录用户控件视图
    /// </summary>
    public sealed partial class HistoryLiteControl : UserControl
    {
        public string Fillin { get; } = ResourceService.GetLocalized("Store/Fillin");

        public string FillinToolTip { get; } = ResourceService.GetLocalized("Store/FillinToolTip");

        public string Copy { get; } = ResourceService.GetLocalized("Store/Copy");

        public string CopyToolTip { get; } = ResourceService.GetLocalized("Store/CopyToolTip");

        public HistoryLiteControl()
        {
            InitializeComponent();
        }
    }
}
