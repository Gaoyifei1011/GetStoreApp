using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Controls
{
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
