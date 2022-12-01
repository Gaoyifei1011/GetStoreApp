using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class ThemeControl : UserControl
    {
        public ThemeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName,string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
