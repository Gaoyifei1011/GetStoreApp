using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class HistoryLiteControl : UserControl
    {
        public string Fillin { get; } = ResourceService.GetLocalized("Home/Fillin");

        public string FillinToolTip { get; } = ResourceService.GetLocalized("Home/FillinToolTip");

        public string Copy { get; } = ResourceService.GetLocalized("Home/Copy");

        public string CopyToolTip { get; } = ResourceService.GetLocalized("Home/CopyToolTip");

        public HistoryLiteControl()
        {
            InitializeComponent();
        }
    }
}
