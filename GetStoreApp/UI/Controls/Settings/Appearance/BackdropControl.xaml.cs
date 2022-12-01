using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class BackdropControl : UserControl
    {
        public BackdropControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
