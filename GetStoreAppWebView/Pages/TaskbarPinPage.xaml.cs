using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppWebView.Pages
{
    /// <summary>
    /// 固定应用到任务栏提示页面
    /// </summary>
    public sealed partial class TaskbarPinPage : Page, INotifyPropertyChanged
    {
        private bool _enableBackdropMaterial;

        public bool EnableBackdropMaterial
        {
            get { return _enableBackdropMaterial; }

            set
            {
                if (!Equals(_enableBackdropMaterial, value))
                {
                    _enableBackdropMaterial = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableBackdropMaterial)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TaskbarPinPage()
        {
            InitializeComponent();

            if (ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush)))
            {
                EnableBackdropMaterial = true;
                VisualStateManager.GoToState(TaskbarPinPageRoot, "MicaBackdrop", false);
            }
            else
            {
                EnableBackdropMaterial = false;
                VisualStateManager.GoToState(TaskbarPinPageRoot, "DesktopAcrylicBackdrop", false);
            }
        }
    }
}
