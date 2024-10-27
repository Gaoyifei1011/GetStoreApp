using GetStoreAppInstaller.Helpers.Backdrop;
using GetStoreAppInstaller.UI.Backdrop;
using Mile.Xaml;
using System.ComponentModel;
using System.Windows.Forms;
using Windows.UI.Composition.Desktop;

namespace GetStoreAppInstaller.Views.Windows
{
    public partial class MainWindow : Form
    {
        private DesktopWindowTarget DesktopWindowTarget { get; set; }
        private readonly WindowsXamlHost xamlHost = new();
        private readonly MicaBackdrop currentSystemBackdrop = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            Current = this;
            CenterToScreen();
            Size = new System.Drawing.Size(900, 600);
            Controls.Add(xamlHost);
            xamlHost.AutoSize = true;
            xamlHost.Dock = DockStyle.Fill;
            xamlHost.Child = new MainPage();

            DesktopWindowTarget = BackdropHelper.InitializeDesktopWindowTarget(Handle, false);

            currentSystemBackdrop = new MicaBackdrop(DesktopWindowTarget, xamlHost.Child as global::Windows.UI.Xaml.FrameworkElement, Handle)
            {
                Kind = MicaKind.Base,
            };

            currentSystemBackdrop.InitializeBackdrop();
        }
    }
}
