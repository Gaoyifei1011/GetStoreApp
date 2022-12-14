using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

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

        public bool IsItemLoaded(int backdropIndex,int backdropListCount)
        {
            return backdropIndex < backdropListCount;
        }
    }
}
