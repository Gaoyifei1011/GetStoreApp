using GetStoreApp.Helpers.Converters;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;

namespace GetStoreApp.UI.Pages
{
    /// <summary>
    /// 应用信息页面
    /// </summary>
    public sealed partial class AppInfoPage : Page, INotifyPropertyChanged
    {
        private static string Unknown { get; } = ResourceService.GetLocalized("UWPApp/Unknown");

        private string _displayName = string.Empty;

        public string DisplayName
        {
            get { return _displayName; }

            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }

            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _publisher = string.Empty;

        public string Publisher
        {
            get { return _publisher; }

            set
            {
                _publisher = value;
                OnPropertyChanged();
            }
        }

        private string _appVersion;

        public string AppVersion
        {
            get { return _appVersion; }

            set
            {
                _appVersion = value;
                OnPropertyChanged();
            }
        }

        private string _installedDate;

        public string InstalledDate
        {
            get { return _installedDate; }

            set
            {
                _installedDate = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                InitializeAppInfo(args.Parameter as Package);
            }
            else
            {
                SetDefaultAppInfo();
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        private void InitializeAppInfo(Package currentPackage)
        {
            try { DisplayName = string.IsNullOrEmpty(currentPackage.DisplayName) ? Unknown : currentPackage.DisplayName; } catch { DisplayName = Unknown; }
            try { Description = string.IsNullOrEmpty(currentPackage.Description) ? Unknown : currentPackage.Description; } catch { Description = Unknown; }
            try { Publisher = string.IsNullOrEmpty(currentPackage.PublisherDisplayName) ? Unknown : currentPackage.PublisherDisplayName; } catch { Publisher = Unknown; }
            try
            {
                AppVersion = string.Format("{0}.{1}.{2}.{3}",
                    currentPackage.Id.Version.Major,
                    currentPackage.Id.Version.Minor,
                    currentPackage.Id.Version.Build,
                    currentPackage.Id.Version.Revision
                    );
            }
            catch
            {
                AppVersion = "0.0.0.0";
            }
            try { InstalledDate = currentPackage.InstalledDate.ToString("yyyy/MM/dd HH:mm", StringConverterHelper.AppCulture); } catch { InstalledDate = "1970/01/01 00:00"; }
        }

        /// <summary>
        /// 输入的应用包有误，恢复到默认值
        /// </summary>
        private void SetDefaultAppInfo()
        {
            DisplayName = Unknown;
            Description = Unknown;
            Publisher = Unknown;
            AppVersion = "0.0.0.0";
            InstalledDate = "1970/01/01 00:00";
        }
    }
}
