namespace GetStoreAppInstaller
{
    public sealed partial class XamlIslandsApp : Application
    {
        public XamlIslandsApp()
        {
            this.ThreadInitialize();
            this.InitializeComponent();
        }

        public void Close()
        {
            this.Exit();
            this.ThreadUninitialize();
        }
    }
}
