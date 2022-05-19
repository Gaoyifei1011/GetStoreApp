using GetStoreApp.Helpers;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Threading.Tasks;

using Windows.ApplicationModel;

namespace GetStoreApp.ViewModels.Pages
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/settings.md
    public class SettingsViewModel : ObservableObject
    {
        // 版本信息
        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        public SettingsViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
