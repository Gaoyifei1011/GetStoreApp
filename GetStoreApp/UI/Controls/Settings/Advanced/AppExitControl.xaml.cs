using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class AppExitControl : UserControl
    {
        public AppExitControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName,string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
