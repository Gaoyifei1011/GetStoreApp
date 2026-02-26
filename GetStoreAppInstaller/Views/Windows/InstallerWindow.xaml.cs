using GetStoreAppInstaller.Extensions.Backdrop;
using GetStoreAppInstaller.Extensions.DataType.Classes;
using GetStoreAppInstaller.Extensions.DataType.Enums;
using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Services.Settings;
using GetStoreAppInstaller.Views.NotificationTips;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.SHCore;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Uxtheme;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.ApplicationModel.Resources;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Management.Deployment;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppInstaller.Views.Windows
{
    /// <summary>
    /// 应用安装器窗口
    /// </summary>
    public sealed partial class InstallerWindow : Window, INotifyPropertyChanged
    {
        private readonly string msresource = "ms-resource:";
        private readonly string AppInstallFailedString = ResourceService.GetLocalized("Installer/AppInstallFailed");
        private readonly string AppInstallFailed1String = ResourceService.GetLocalized("Installer/AppInstallFailed1");
        private readonly string AppInstallFailed2String = ResourceService.GetLocalized("Installer/AppInstallFailed2");
        private readonly string AppInstallFailed3String = ResourceService.GetLocalized("Installer/AppInstallFailed3");
        private readonly string AppInstallSelfString = ResourceService.GetLocalized("Installer/AppInstallSelf");
        private readonly string AppInstallSuccessfullyString = ResourceService.GetLocalized("Installer/AppInstallSuccessfully");
        private readonly string AppInstallSuccessfully1String = ResourceService.GetLocalized("Installer/AppInstallSuccessfully1");
        private readonly string AppInstalledNotNewVersionString = ResourceService.GetLocalized("Installer/AppInstalledNotNewVersion");
        private readonly string AppInstalledNewVersionString = ResourceService.GetLocalized("Installer/AppInstalledNewVersion");
        private readonly string AppNotInstallString = ResourceService.GetLocalized("Installer/AppNotInstall");
        private readonly string BundleHeaderString = ResourceService.GetLocalized("Installer/BundleHeader");
        private readonly string HoursString = ResourceService.GetLocalized("Installer/Hours");
        private readonly string InstallProgressString = ResourceService.GetLocalized("Installer/InstallProgress");
        private readonly string NoneString = ResourceService.GetLocalized("Installer/None");
        private readonly string NoString = ResourceService.GetLocalized("Installer/No");
        private readonly string NotAvailableString = ResourceService.GetLocalized("Installer/NotAvailable");
        private readonly string OpenInstallerFileString = ResourceService.GetLocalized("Installer/OpenInstallerFile");
        private readonly string OpenPackageString = ResourceService.GetLocalized("Installer/OpenPackage");
        private readonly string PackageBundleString = ResourceService.GetLocalized("Installer/PackageBundle");
        private readonly string PackageString = ResourceService.GetLocalized("Installer/Package");
        private readonly string PrepareInstallString = ResourceService.GetLocalized("Installer/PrepareInstall");
        private readonly string RunningAdministratorString = ResourceService.GetLocalized("Installer/RunningAdministrator");
        private readonly string TitleString = ResourceService.GetLocalized("Installer/Title");
        private readonly string UnsupportedFileTypeString = ResourceService.GetLocalized("Installer/UnsupportedFileType");
        private readonly string UnsupportedMultiFilesString = ResourceService.GetLocalized("Installer/UnsupportedMultiFiles");
        private readonly string WaitInstallString = ResourceService.GetLocalized("Installer/WaitInstall");
        private readonly string YesString = ResourceService.GetLocalized("Installer/Yes");
        private readonly Guid CLSID_AppxFactory = new("5842A140-FF9F-4166-8F5C-62F5B7B0C781");
        private readonly Guid CLSID_AppxBundleFactory = new("378E0446-5384-43B7-8877-E7DBDD883446");
        private readonly global::Windows.Management.Deployment.PackageManager packageManager = new();
        private readonly PackageDeploymentManager packageDeploymentManager = PackageDeploymentManager.GetDefault();
        private readonly ContentIsland contentIsland;
        private readonly InputKeyboardSource inputKeyboardSource;
        private readonly InputPointerSource inputPointerSource;
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly OverlappedPresenter overlappedPresenter;
        private readonly SUBCLASSPROC installerWindowSubClassProc;
        private readonly IAppxFactory3 appxFactory;
        private readonly IAppxBundleFactory2 appxBundleFactory;

        [GeneratedRegex("""scale-(\d{3})""", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex ScaleRegex { get; }

        private double rasterizationScale;
        private string fileName = string.Empty;
        private IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> installPackageWithProgress;

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!string.Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
                }
            }
        }

        private SystemBackdrop _windowSystemBackdrop;

        public SystemBackdrop WindowSystemBackdrop
        {
            get { return _windowSystemBackdrop; }

            set
            {
                if (!Equals(_windowSystemBackdrop, value))
                {
                    _windowSystemBackdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowSystemBackdrop)));
                }
            }
        }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        private bool _isParseEmpty;

        public bool IsParseEmpty
        {
            get { return _isParseEmpty; }

            set
            {
                if (!Equals(_isParseEmpty, value))
                {
                    _isParseEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsParseEmpty)));
                }
            }
        }

        private bool _isLoadCompleted;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private bool _canDragFile;

        public bool CanDragFile
        {
            get { return _canDragFile; }

            set
            {
                if (!Equals(_canDragFile, value))
                {
                    _canDragFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanDragFile)));
                }
            }
        }

        private bool _isParseSuccessfully;

        public bool IsParseSuccessfully
        {
            get { return _isParseSuccessfully; }

            set
            {
                if (!Equals(_isParseSuccessfully, value))
                {
                    _isParseSuccessfully = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsParseSuccessfully)));
                }
            }
        }

        private PackageFileType _packageFileType;

        public PackageFileType PackageFileType
        {
            get { return _packageFileType; }

            set
            {
                if (!Equals(_packageFileType, value))
                {
                    _packageFileType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFileType)));
                }
            }
        }

        private ImageSource _packageIconImage;

        public ImageSource PackageIconImage
        {
            get { return _packageIconImage; }

            set
            {
                if (!Equals(_packageIconImage, value))
                {
                    _packageIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageIconImage)));
                }
            }
        }

        private string _packageName;

        public string PackageName
        {
            get { return _packageName; }

            set
            {
                if (!string.Equals(_packageName, value))
                {
                    _packageName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageName)));
                }
            }
        }

        private string _publisherDisplayName;

        public string PublisherDisplayName
        {
            get { return _publisherDisplayName; }

            set
            {
                if (!string.Equals(_publisherDisplayName, value))
                {
                    _publisherDisplayName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublisherDisplayName)));
                }
            }
        }

        private Version _version;

        public Version Version
        {
            get { return _version; }

            set
            {
                if (!string.Equals(_version, value))
                {
                    _version = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Version)));
                }
            }
        }

        private string _packageDescription;

        public string PackageDescription
        {
            get { return _packageDescription; }

            set
            {
                if (!string.Equals(_packageDescription, value))
                {
                    _packageDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageDescription)));
                }
            }
        }

        private string _packageFamilyName;

        public string PackageFamilyName
        {
            get { return _packageFamilyName; }

            set
            {
                if (!string.Equals(_packageFamilyName, value))
                {
                    _packageFamilyName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFamilyName)));
                }
            }
        }

        private string _packageFullName;

        public string PackageFullName
        {
            get { return _packageFullName; }

            set
            {
                if (!string.Equals(_packageFullName, value))
                {
                    _packageFullName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFullName)));
                }
            }
        }

        private string _supportedArchitecture;

        public string SupportedArchitecture
        {
            get { return _supportedArchitecture; }

            set
            {
                if (!string.Equals(_supportedArchitecture, value))
                {
                    _supportedArchitecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SupportedArchitecture)));
                }
            }
        }

        private string _isFramework;

        public string IsFramework
        {
            get { return _isFramework; }

            set
            {
                if (!string.Equals(_isFramework, value))
                {
                    _isFramework = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFramework)));
                }
            }
        }

        private string _appInstalledState;

        public string AppInstalledState
        {
            get { return _appInstalledState; }

            set
            {
                if (!string.Equals(_appInstalledState, value))
                {
                    _appInstalledState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppInstalledState)));
                }
            }
        }

        private string _appInstallerSourceLink;

        public string AppInstallerSourceLink
        {
            get { return _appInstallerSourceLink; }

            set
            {
                if (!string.Equals(_appInstallerSourceLink, value))
                {
                    _appInstallerSourceLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppInstallerSourceLink)));
                }
            }
        }

        private bool _isAppInstallerSourceLinkExisted;

        public bool IsAppInstallerSourceLinkExisted
        {
            get { return _isAppInstallerSourceLinkExisted; }

            set
            {
                if (!Equals(_isAppInstallerSourceLinkExisted, value))
                {
                    _isAppInstallerSourceLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppInstallerSourceLinkExisted)));
                }
            }
        }

        private string _packageSourceLink;

        public string PackageSourceLink
        {
            get { return _packageSourceLink; }

            set
            {
                if (!string.Equals(_packageSourceLink, value))
                {
                    _packageSourceLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageSourceLink)));
                }
            }
        }

        private bool _isPackageSourceLinkExisted;

        public bool IsPackageSourceLinkExisted
        {
            get { return _isPackageSourceLinkExisted; }

            set
            {
                if (!Equals(_isPackageSourceLinkExisted, value))
                {
                    _isPackageSourceLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPackageSourceLinkExisted)));
                }
            }
        }

        private string _packageType;

        public string PackageType
        {
            get { return _packageType; }

            set
            {
                if (!string.Equals(_packageType, value))
                {
                    _packageType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageType)));
                }
            }
        }

        private string _hoursBetweenUpdateChecks;

        public string HoursBetweenUpdateChecks
        {
            get { return _hoursBetweenUpdateChecks; }

            set
            {
                if (!Equals(_hoursBetweenUpdateChecks, value))
                {
                    _hoursBetweenUpdateChecks = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoursBetweenUpdateChecks)));
                }
            }
        }

        private string _updateBlocksActivation;

        public string UpdateBlocksActivation
        {
            get { return _updateBlocksActivation; }

            set
            {
                if (!string.Equals(_updateBlocksActivation, value))
                {
                    _updateBlocksActivation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateBlocksActivation)));
                }
            }
        }

        private string _showPrompt;

        public string ShowPrompt
        {
            get { return _showPrompt; }

            set
            {
                if (!string.Equals(_showPrompt, value))
                {
                    _showPrompt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowPrompt)));
                }
            }
        }

        private string _forceUpdateFromAnyVersion;

        public string ForceUpdateFromAnyVersion
        {
            get { return _forceUpdateFromAnyVersion; }

            set
            {
                if (!string.Equals(_forceUpdateFromAnyVersion, value))
                {
                    _forceUpdateFromAnyVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceUpdateFromAnyVersion)));
                }
            }
        }

        private string _automaticBackgroundTask;

        public string AutomaticBackgroundTask
        {
            get { return _automaticBackgroundTask; }

            set
            {
                if (!string.Equals(_automaticBackgroundTask, value))
                {
                    _automaticBackgroundTask = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutomaticBackgroundTask)));
                }
            }
        }

        private bool _isAppInstalled;

        public bool IsAppInstalled
        {
            get { return _isAppInstalled; }

            set
            {
                if (!Equals(_isAppInstalled, value))
                {
                    _isAppInstalled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppInstalled)));
                }
            }
        }

        private bool _isUpdateSettingsExisted;

        public bool IsUpdateSettingsExisted
        {
            get { return _isUpdateSettingsExisted; }

            set
            {
                if (!Equals(_isUpdateSettingsExisted, value))
                {
                    _isUpdateSettingsExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdateSettingsExisted)));
                }
            }
        }

        private bool _isInstalling;

        public bool IsInstalling
        {
            get { return _isInstalling; }

            set
            {
                if (!Equals(_isInstalling, value))
                {
                    _isInstalling = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalling)));
                }
            }
        }

        private double _installProgressValue;

        public double InstallProgressValue
        {
            get { return _installProgressValue; }

            set
            {
                if (!Equals(_installProgressValue, value))
                {
                    _installProgressValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallProgressValue)));
                }
            }
        }

        private bool _isInstallWaiting;

        public bool IsInstallWaiting
        {
            get { return _isInstallWaiting; }

            set
            {
                if (!Equals(_isInstallWaiting, value))
                {
                    _isInstallWaiting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstallWaiting)));
                }
            }
        }

        private bool _isInstallFailed;

        public bool IsInstallFailed
        {
            get { return _isInstallFailed; }

            set
            {
                if (!Equals(_isInstallFailed, value))
                {
                    _isInstallFailed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstallFailed)));
                }
            }
        }

        private bool _isCancelInstall;

        public bool IsCancelInstall
        {
            get { return _isCancelInstall; }

            set
            {
                if (!Equals(_isCancelInstall, value))
                {
                    _isCancelInstall = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCancelInstall)));
                }
            }
        }

        private string _installStateString;

        public string InstallStateString
        {
            get { return _installStateString; }

            set
            {
                if (!string.Equals(_installStateString, value))
                {
                    _installStateString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallStateString)));
                }
            }
        }

        private string _installFailedInformation;

        public string InstallFailedInformation
        {
            get { return _installFailedInformation; }

            set
            {
                if (!string.Equals(_installFailedInformation, value))
                {
                    _installFailedInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallFailedInformation)));
                }
            }
        }

        private ObservableCollection<TargetDeviceFamilyModel> TargetDeviceFamilyCollection { get; } = [];

        private ObservableCollection<DependencyModel> DependencyCollection { get; } = [];

        private ObservableCollection<CapabilityModel> CapabilitiesCollection { get; } = [];

        private ObservableCollection<ApplicationModel> ApplicationCollection { get; } = [];

        private ObservableCollection<CultureInfo> LanguageCollection { get; } = [];

        private ObservableCollection<InstallDependencyModel> InstallDependencyCollection { get; } = [];

        private Dictionary<string, string> CapabilityDict { get; } = new()
        {
            { "accessorymanager", ResourceService.GetLocalized("Installer/CapabilityAccessoryManager") },
            { "activity", ResourceService.GetLocalized("Installer/CapabilityActivity") },
            { "allappmods", ResourceService.GetLocalized("Installer/CapabilityAllAppMods") },
            { "alljoyn", ResourceService.GetLocalized("Installer/CapabilityAllJoyn") },
            { "allowelevation", ResourceService.GetLocalized("Installer/CapabilityAllowElevation") },
            { "appbroadcastservices", ResourceService.GetLocalized("Installer/CapabilityAppBroadcastServices") },
            { "appcaptureservices", ResourceService.GetLocalized("Installer/CapabilityAppCaptureServices") },
            { "appcapturesettings", ResourceService.GetLocalized("Installer/CapabilityAppCaptureSettings") },
            { "appdiagnostics", ResourceService.GetLocalized("Installer/CapabilityAppDiagnostics") },
            { "applicensing", ResourceService.GetLocalized("Installer/CapabilityAppLicensing") },
            { "appointments", ResourceService.GetLocalized("Installer/CapabilityAppointments") },
            { "appointmentssystem", ResourceService.GetLocalized("Installer/CapabilityAppointmentsSystem") },
            { "audiodeviceconfiguration", ResourceService.GetLocalized("Installer/CapabilityAudioDeviceConfiguration") },
            { "backgroundmediaplayback", ResourceService.GetLocalized("Installer/CapabilityBackgroundMediaPlayback") },
            { "backgroundmediarecording", ResourceService.GetLocalized("Installer/CapabilityBackgroundMediaRecording") },
            { "backgroundspatialperception", ResourceService.GetLocalized("Installer/CapabilityBackgroundSpatialPerception") },
            { "backgroundvoip", ResourceService.GetLocalized("Installer/CapabilityBackgroundVoIP") },
            { "blockedchatmessages", ResourceService.GetLocalized("Installer/CapabilityBlockedChatMessages") },
            { "bluetooth", ResourceService.GetLocalized("Installer/CapabilityBluetooth") },
            { "broadfilesystemaccess", ResourceService.GetLocalized("Installer/CapabilityBroadFileSystemAccess") },
            { "cameraprocessingextension", ResourceService.GetLocalized("Installer/CapabilityCameraProcessingExtension") },
            { "cellulardevicecontrol", ResourceService.GetLocalized("Installer/CapabilityCellularDeviceControl") },
            { "cellulardeviceidentity", ResourceService.GetLocalized("Installer/CapabilityCellularDeviceIdentity") },
            { "cellularmessaging", ResourceService.GetLocalized("Installer/CapabilityCellularMessaging") },
            { "chat", ResourceService.GetLocalized("Installer/CapabilityChat") },
            { "chatsystem", ResourceService.GetLocalized("Installer/CapabilityChatSystem") },
            { "codegeneration", ResourceService.GetLocalized("Installer/CapabilityCodeGeneration") },
            { "confirmappclose", ResourceService.GetLocalized("Installer/CapabilityConfirmAppClose") },
            { "contacts", ResourceService.GetLocalized("Installer/CapabilityContacts") },
            { "contactssystem", ResourceService.GetLocalized("Installer/CapabilityContactsSystem") },
            { "cortanapermissions", ResourceService.GetLocalized("Installer/CapabilityCortanaPermissions") },
            { "cortanaspeechaccessory", ResourceService.GetLocalized("Installer/CapabilityCortanaSpeechAccessory") },
            { "custominstallactions", ResourceService.GetLocalized("Installer/CapabilityCustomInstallActions") },
            { "developmentmodenetwork", ResourceService.GetLocalized("Installer/CapabilityDevelopmentModeNetwork") },
            { "devicemanagementdmaccount", ResourceService.GetLocalized("Installer/CapabilityDeviceManagementDmAccount") },
            { "devicemanagementemailaccount", ResourceService.GetLocalized("Installer/CapabilityDeviceManagementEmailAccount") },
            { "devicemanagementfoundation", ResourceService.GetLocalized("Installer/CapabilityDeviceManagementFoundation") },
            { "devicemanagementwapsecuritypolicies", ResourceService.GetLocalized("Installer/CapabilityDeviceManagementWapSecurityPolicies") },
            { "deviceportalprovider", ResourceService.GetLocalized("Installer/CapabilityDevicePortalProvider") },
            { "deviceunlock", ResourceService.GetLocalized("Installer/CapabilityDeviceUnlock") },
            { "documentslibrary", ResourceService.GetLocalized("Installer/CapabilityDocumentsLibrary") },
            { "dualsimtiles", ResourceService.GetLocalized("Installer/CapabilityDualSimTiles") },
            { "email", ResourceService.GetLocalized("Installer/CapabilityEmail") },
            { "emailsystem", ResourceService.GetLocalized("Installer/CapabilityEmailSystem") },
            { "enterpriseauthentication", ResourceService.GetLocalized("Installer/CapabilityEnterpriseAuthentication") },
            { "enterprisecloudsso", ResourceService.GetLocalized("Installer/CapabilityEnterpriseCloudSSO") },
            { "enterprisedatapolicy", ResourceService.GetLocalized("Installer/CapabilityEnterpriseDataPolicy") },
            { "enterprisedevicelockdown", ResourceService.GetLocalized("Installer/CapabilityEnterpriseDeviceLockdown") },
            { "expandedresources", ResourceService.GetLocalized("Installer/CapabilityExpandedResources") },
            { "extendedbackgroundtasktime", ResourceService.GetLocalized("Installer/CapabilityExtendedBackgroundTaskTime") },
            { "extendedexecutionbackgroundaudio", ResourceService.GetLocalized("Installer/CapabilityExtendedExecutionBackgroundAudio") },
            { "extendedexecutioncritical", ResourceService.GetLocalized("Installer/CapabilityExtendedExecutionCritical") },
            { "extendedexecutionunconstrained", ResourceService.GetLocalized("Installer/CapabilityExtendedExecutionUnconstrained") },
            { "firstsigninsettings", ResourceService.GetLocalized("Installer/CapabilityFirstSignInSettings") },
            { "gamebarservices", ResourceService.GetLocalized("Installer/CapabilityGameBarServices") },
            { "gamelist", ResourceService.GetLocalized("Installer/CapabilityGameList") },
            { "gamemonitor", ResourceService.GetLocalized("Installer/CapabilityGameMonitor") },
            { "gazeinput", ResourceService.GetLocalized("Installer/CapabilityGazeInput") },
            { "globalmediacontrol", ResourceService.GetLocalized("Installer/CapabilityGlobalMediaControl") },
            { "graphicscapture", ResourceService.GetLocalized("Installer/CapabilityGraphicsCapture") },
            { "graphicscaptureprogrammatic", ResourceService.GetLocalized("Installer/CapabilityGraphicsCaptureProgrammatic") },
            { "graphicscapturewithoutborder", ResourceService.GetLocalized("Installer/CapabilityGraphicsCaptureWithoutBorder") },
            { "humaninterfacedevice", ResourceService.GetLocalized("Installer/CapabilityHumanInterfaceDevice") },
            { "humanpresence", ResourceService.GetLocalized("Installer/CapabilityHumanPresence") },
            { "inputforegroundobservation", ResourceService.GetLocalized("Installer/CapabilityInputForegroundObservation") },
            { "inputinjectionbrokered", ResourceService.GetLocalized("Installer/CapabilityInputInjectionBrokered") },
            { "inputobservation", ResourceService.GetLocalized("Installer/CapabilityInputObservation") },
            { "inputsuppression", ResourceService.GetLocalized("Installer/CapabilityInputSuppression") },
            { "internetclient", ResourceService.GetLocalized("Installer/CapabilityInternetClient") },
            { "internetclientserver", ResourceService.GetLocalized("Installer/CapabilityInternetClientServer") },
            { "interopservices", ResourceService.GetLocalized("Installer/CapabilityInteropServices") },
            { "localsystemservices", ResourceService.GetLocalized("Installer/CapabilityLocalSystemServices") },
            { "location", ResourceService.GetLocalized("Installer/CapabilityLocation") },
            { "locationhistory", ResourceService.GetLocalized("Installer/CapabilityLocationHistory") },
            { "locationsystem", ResourceService.GetLocalized("Installer/CapabilityLocationSystem") },
            { "lowlevel", ResourceService.GetLocalized("Installer/CapabilityLowLevel") },
            { "lowleveldevices", ResourceService.GetLocalized("Installer/CapabilityLowLevelDevices") },
            { "microphone", ResourceService.GetLocalized("Installer/CapabilityMicrophone") },
            { "microsoft.cellularsarconfiguration_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftCellularSARConfiguration") },
            { "microsoft.classicappcompat_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftClassicAppCompat") },
            { "microsoft.classicappcompatelevated_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftClassicAppCompatElevated") },
            { "microsoft.classicappinstaller_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftClassicAppInstaller") },
            { "microsoft.coreappactivation_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftCoreAppActivation") },
            { "microsoft.delegatedwebfeatures_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftDelegatedWebFeatures") },
            { "microsoft.deployfulltrustonhost_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftDeployFullTrustOnHost") },
            { "microsoft.esimmanagement_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftESIMManagement") },
            { "microsoft.nonuserconfigurablestartuptasks_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftNonUserConfigurableStartupTasks") },
            { "microsoft.notsupportedincorev1_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftNotSupportedInCoreV1") },
            { "microsoft.ondemandhotspotcontrol_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftOnDemandHotspotControl") },
            { "microsoft.requiresnonsmode_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftRequiresNonSMode") },
            { "microsoft.secondaryauthenticationfactorforlogon_8wekyb3d8bbwe", ResourceService.GetLocalized("Installer/CapabilityMicrosoftSecondaryAuthenticationFactorForLogon") },
            { "modifiableapp", ResourceService.GetLocalized("Installer/CapabilityModifiableApp") },
            { "musiclibrary", ResourceService.GetLocalized("Installer/CapabilityMusicLibrary") },
            { "networkconnectionmanagerprovisioning", ResourceService.GetLocalized("Installer/CapabilityNetworkConnectionManagerProvisioning") },
            { "networkdataplanprovisioning", ResourceService.GetLocalized("Installer/CapabilityNetworkDataPlanProvisioning") },
            { "networkdatausagemanagement", ResourceService.GetLocalized("Installer/CapabilityNetworkDataUsageManagement") },
            { "networkingvpnprovider", ResourceService.GetLocalized("Installer/CapabilityNetworkingVpnProvider") },
            { "objects3d", ResourceService.GetLocalized("Installer/CapabilityObjects3D") },
            { "oemdeployment", ResourceService.GetLocalized("Installer/CapabilityOemDeployment") },
            { "oempublicdirectory", ResourceService.GetLocalized("Installer/CapabilityOemPublicDirectory") },
            { "oneprocessvoip", ResourceService.GetLocalized("Installer/CapabilityOneProcessVoIP") },
            { "optional", ResourceService.GetLocalized("Installer/CapabilityOptional") },
            { "packagedservices", ResourceService.GetLocalized("Installer/CapabilityPackagedServices") },
            { "packagemanagement", ResourceService.GetLocalized("Installer/CapabilityPackageManagement") },
            { "packagepolicysystem", ResourceService.GetLocalized("Installer/CapabilityPackagePolicySystem") },
            { "packagequery", ResourceService.GetLocalized("Installer/CapabilityPackageQuery") },
            { "packagewriteredirectioncompatibilityshim", ResourceService.GetLocalized("Installer/CapabilityPackageWriteRedirectionCompatibilityShim") },
            { "phonecall", ResourceService.GetLocalized("Installer/CapabilityPhoneCall") },
            { "phonecallhistory", ResourceService.GetLocalized("Installer/CapabilityPhoneCallHistory") },
            { "phonecallhistorysystem", ResourceService.GetLocalized("Installer/CapabilityPhoneCallHistorySystem") },
            { "phonelinetransportmanagement", ResourceService.GetLocalized("Installer/CapabilityPhoneLineTransportManagement") },
            { "pictureslibrary", ResourceService.GetLocalized("Installer/CapabilityPicturesLibrary") },
            { "pointofservice", ResourceService.GetLocalized("Installer/CapabilityPointOfService") },
            { "previewinkworkspace", ResourceService.GetLocalized("Installer/CapabilityPreviewInkWorkspace") },
            { "previewpenworkspace", ResourceService.GetLocalized("Installer/CapabilityPreviewPenWorkspace") },
            { "previewstore", ResourceService.GetLocalized("Installer/CapabilityPreviewStore") },
            { "previewuicomposition", ResourceService.GetLocalized("Installer/CapabilityPreviewUiComposition") },
            { "privatenetworkclientserver", ResourceService.GetLocalized("Installer/CapabilityPrivateNetworkClientServer") },
            { "protectedapp", ResourceService.GetLocalized("Installer/CapabilityProtectedApp") },
            { "proximity", ResourceService.GetLocalized("Installer/CapabilityProximity") },
            { "radios", ResourceService.GetLocalized("Installer/CapabilityRadios") },
            { "recordedcallsfolder", ResourceService.GetLocalized("Installer/CapabilityRecordedCallsFolder") },
            { "remotepassportauthentication", ResourceService.GetLocalized("Installer/CapabilityRemotePassportAuthentication") },
            { "remotesystem", ResourceService.GetLocalized("Installer/CapabilityRemoteSystem") },
            { "removablestorage", ResourceService.GetLocalized("Installer/CapabilityRemovableStorage") },
            { "runfulltrust", ResourceService.GetLocalized("Installer/CapabilityRunFullTrust") },
            { "screenduplication", ResourceService.GetLocalized("Installer/CapabilityScreenDuplication") },
            { "secondaryauthenticationfactor", ResourceService.GetLocalized("Installer/CapabilitySecondaryAuthenticationFactor") },
            { "secureassessment", ResourceService.GetLocalized("Installer/CapabilitySecureAssessment") },
            { "serialcommunication", ResourceService.GetLocalized("Installer/CapabilitySerialCommunication") },
            { "sharedusercertificates", ResourceService.GetLocalized("Installer/CapabilitySharedUserCertificates") },
            { "slapiquerylicensevalue", ResourceService.GetLocalized("Installer/CapabilitySlapiQueryLicenseValue") },
            { "smbios", ResourceService.GetLocalized("Installer/CapabilitySmbios") },
            { "smssend", ResourceService.GetLocalized("Installer/CapabilitySmsSend") },
            { "spatialperception", ResourceService.GetLocalized("Installer/CapabilitySpatialPerception") },
            { "startscreenmanagement", ResourceService.GetLocalized("Installer/CapabilityStartScreenManagement") },
            { "storelicensemanagement", ResourceService.GetLocalized("Installer/CapabilityStoreLicenseManagement") },
            { "systemmanagement", ResourceService.GetLocalized("Installer/CapabilitySystemManagement") },
            { "targetedcontent", ResourceService.GetLocalized("Installer/CapabilityTargetedContent") },
            { "teameditiondevicecredential", ResourceService.GetLocalized("Installer/CapabilityTeamEditionDeviceCredential") },
            { "teameditionexperience", ResourceService.GetLocalized("Installer/CapabilityTeamEditionExperience") },
            { "teameditionview", ResourceService.GetLocalized("Installer/CapabilityTeamEditionView") },
            { "uiaccess", ResourceService.GetLocalized("Installer/CapabilityUiAccess") },
            { "uiautomation", ResourceService.GetLocalized("Installer/CapabilityUiAutomation") },
            { "unvirtualizedresources", ResourceService.GetLocalized("Installer/CapabilityUnvirtualizedResources") },
            { "usb", ResourceService.GetLocalized("Installer/CapabilityUSB") },
            { "useraccountinformation", ResourceService.GetLocalized("Installer/CapabilityUserAccountInformation") },
            { "userdataaccountsprovider", ResourceService.GetLocalized("Installer/CapabilityUserDataAccountsProvider") },
            { "userdatatasks", ResourceService.GetLocalized("Installer/CapabilityUserDataTasks") },
            { "userdatasystem", ResourceService.GetLocalized("Installer/CapabilityUserDataSystem") },
            { "usernotificationlistener", ResourceService.GetLocalized("Installer/CapabilityUserNotificationListener") },
            { "userprincipalname", ResourceService.GetLocalized("Installer/CapabilityUserPrincipalName") },
            { "usersystemid", ResourceService.GetLocalized("Installer/CapabilityUserSystemId") },
            { "videoslibrary", ResourceService.GetLocalized("Installer/CapabilityVideosLibrary") },
            { "voipcall", ResourceService.GetLocalized("Installer/CapabilityVoipCall") },
            { "walletsystem", ResourceService.GetLocalized("Installer/CapabilityWalletSystem") },
            { "webcam", ResourceService.GetLocalized("Installer/CapabilityWebCam") },
            { "wificontrol", ResourceService.GetLocalized("Installer/CapabilityWifiControl") },
            { "xboxaccessorymanagement", ResourceService.GetLocalized("Installer/CapabilityXboxAccessoryManagement") },
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public InstallerWindow()
        {
            InitializeComponent();

            // 窗口部分初始化
            WindowTitle = RuntimeHelper.IsElevated ? TitleString + RunningAdministratorString : TitleString;
            overlappedPresenter = AppWindow.Presenter.As<OverlappedPresenter>();
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(AppWindow.Id);
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
            inputPointerSource = InputPointerSource.GetForIsland(contentIsland);

            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            contentIsland.StateChanged += OnStateChanged;
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;
            inputPointerSource.PointerReleased += OnPointerReleased;

            // 标题栏和右键菜单设置
            SetClassicMenuTheme(Content.As<FrameworkElement>().ActualTheme);

            // 为应用主窗口添加窗口过程
            installerWindowSubClassProc = new SUBCLASSPROC(InstallerWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), installerWindowSubClassProc, 0, nint.Zero);

            SetWindowTheme();
            SetSystemBackdrop();

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
                Shell32Library.DragAcceptFiles(Win32Interop.GetWindowFromWindowId(AppWindow.Id), true);
            }

            if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out nint appxFactoryPtr) is 0)
            {
                appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.None);
            }

            if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out nint appxBundleFactoryPtr) is 0)
            {
                appxBundleFactory = (IAppxBundleFactory2)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxBundleFactoryPtr, CreateObjectFlags.None);
            }

            rasterizationScale = contentIsland.RasterizationScale;
            AppWindow.Resize(new SizeInt32(Convert.ToInt32(800 * contentIsland.RasterizationScale), Convert.ToInt32(560 * contentIsland.RasterizationScale)));

            // 默认直接显示到窗口中间
            if (DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest) is DisplayArea displayArea && contentIsland is not null)
            {
                RectInt32 workArea = displayArea.WorkArea;
                AppWindow.Move(new PointInt32((workArea.Width - AppWindow.Size.Width) / 2, (workArea.Height - AppWindow.Size.Height) / 2));
            }
        }

        #region 第一部分：窗口类事件

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (contentIsland is not null && !contentIsland.IsClosed && WindowSystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue || args.WindowActivationState is not WindowActivationState.Deactivated;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }

        /// <summary>
        /// 窗口大小发生改变时的事件
        /// </summary>
        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if (TitlebarMenuFlyout.IsOpen)
            {
                TitlebarMenuFlyout.Hide();
            }

            if (overlappedPresenter is not null)
            {
                IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            }

            if (contentIsland is not null)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(800 * contentIsland.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(560 * contentIsland.RasterizationScale);
                rasterizationScale = contentIsland.RasterizationScale;
            }
        }

        #endregion 第一部分：窗口类事件

        #region 第二部分：窗口辅助类挂载的事件

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            // 窗口位置发生变化
            if (args.DidPositionChange)
            {
                if (TitlebarMenuFlyout.IsOpen)
                {
                    TitlebarMenuFlyout.Hide();
                }

                if (overlappedPresenter is not null)
                {
                    IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
                }
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            AppWindow.Changed -= OnAppWindowChanged;
            contentIsland.Environment.SettingChanged -= OnSettingChanged;
            inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
            inputPointerSource.PointerReleased -= OnPointerReleased;
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), installerWindowSubClassProc, 0);
            (Application.Current as InstallerApp).Dispose();

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_REMOVE);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
            }
        }

        /// <summary>
        /// 内容岛状态发生更改时触发的事件
        /// </summary>
        private void OnStateChanged(ContentIsland sender, ContentIslandStateChangedEventArgs args)
        {
            if (args.DidRasterizationScaleChange)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1000 * contentIsland.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(600 * contentIsland.RasterizationScale);
                rasterizationScale = contentIsland.RasterizationScale;
            }
        }

        /// <summary>
        /// 内容岛设置发生更改时触发的事件
        /// </summary>
        private void OnSettingChanged(ContentIslandEnvironment sender, ContentEnvironmentSettingChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]))
                {
                    WindowTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                SetPopupControlTheme(WindowTheme);
            });
        }

        /// <summary>
        /// 处理键盘系统按键事件
        /// </summary>
        private async void OnSystemKeyDown(InputKeyboardSource sender, KeyEventArgs args)
        {
            if (args.KeyStatus.IsMenuKeyDown && args.VirtualKey is VirtualKey.Space)
            {
                args.Handled = true;
                FlyoutShowOptions options = new()
                {
                    Position = new Point(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
            else if (args.VirtualKey is VirtualKey.F10 && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        /// <summary>
        /// 处理鼠标事件
        /// </summary>
        private async void OnPointerReleased(InputPointerSource sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.Properties.PointerUpdateKind is PointerUpdateKind.RightButtonReleased && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        #endregion 第二部分：窗口辅助类挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<MenuFlyoutItem>().Tag.As<MenuFlyout>() is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<MenuFlyoutItem>().Tag.As<MenuFlyout>() is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第三部分：窗口右键菜单事件

        #region 第四部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除依赖包
        /// </summary>
        private void OnDeleteDependencyPackageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string dependencyFullName && !string.IsNullOrEmpty(dependencyFullName))
            {
                foreach (InstallDependencyModel installDependencyItem in InstallDependencyCollection)
                {
                    if (string.Equals(installDependencyItem.DependencyFullName, dependencyFullName))
                    {
                        InstallDependencyCollection.Remove(installDependencyItem);
                        break;
                    }
                }
            }
        }

        #endregion 第四部分：XamlUICommand 命令调用时挂载的事件

        #region 第五部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
            SetClassicMenuTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 拖拽安装包
        /// </summary>
        private async void OnDragEnter(object sender, DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                if (dragItemsList.Count is 1)
                {
                    string extensionName = Path.GetExtension(dragItemsList[0].Name);

                    if (string.Equals(extensionName, ".appx", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msix", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".appxbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        args.AcceptedOperation = DataPackageOperation.Copy;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = OpenPackageString;
                    }
                    else if (string.Equals(extensionName, ".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        args.AcceptedOperation = DataPackageOperation.Copy;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = OpenInstallerFileString;
                    }
                    else
                    {
                        args.AcceptedOperation = DataPackageOperation.None;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = UnsupportedFileTypeString;
                    }
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = UnsupportedMultiFilesString;
                }

                args.Handled = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnDragEnter), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        private async void OnDrop(object sender, DragEventArgs args)
        {
            DragOperationDeferral dragOperationDeferral = args.GetDeferral();

            try
            {
                DataPackageView dataPackageView = args.DataView;
                fileName = string.Empty;

                if (dataPackageView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                    if (dragItemsList.Count > 0)
                    {
                        try
                        {
                            fileName = dragItemsList[0].Path;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnDrop), 1, e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnDrop), 1, e);
            }
            finally
            {
                dragOperationDeferral.Complete();
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                IsParseEmpty = false;
                ResetResult();

                (bool parseResult, PackageInformation packageInformation) parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
            }
        }

        /// <summary>
        /// 主页面初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            AppWindow.Resize(new SizeInt32(Convert.ToInt32(800 * contentIsland.RasterizationScale), Convert.ToInt32(560 * contentIsland.RasterizationScale)));
            IsParseEmpty = true;
            CanDragFile = true;
            fileName = string.Empty;

            // 正常启动
            if (Program.AppActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                string[] argumentsArray = Environment.GetCommandLineArgs();
                string executableFileName = Path.GetFileName(Environment.ProcessPath);

                if (argumentsArray.Length > 0 && string.Equals(Path.GetExtension(argumentsArray[0]), ".dll", StringComparison.OrdinalIgnoreCase))
                {
                    argumentsArray[0] = argumentsArray[0].Replace(".dll", ".exe");
                }

                if (argumentsArray.Length >= 2 && argumentsArray[0].Contains(executableFileName))
                {
                    fileName = argumentsArray[1];
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    IsParseEmpty = false;
                    ResetResult();

                    (bool parseResult, PackageInformation packageInformation) parseResult = await Task.Run(async () =>
                    {
                        return await ParsePackagedAppAsync(fileName);
                    });

                    await UpdateResultAsync(parseResult);
                }
            }
            // 从文件处启动
            else if (Program.AppActivationArguments.Kind is ExtendedActivationKind.File)
            {
                FileActivatedEventArgs fileActivatedEventArgs = Program.AppActivationArguments.Data.As<FileActivatedEventArgs>();

                // 只解析读取到的第一个文件
                if (fileActivatedEventArgs.Files.Count > 0)
                {
                    fileName = fileActivatedEventArgs.Files[0].Path;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        IsParseEmpty = false;
                        ResetResult();

                        (bool parseResult, PackageInformation packageInformation) parseResult = await Task.Run(async () =>
                        {
                            return await ParsePackagedAppAsync(fileName);
                        });

                        await UpdateResultAsync(parseResult);
                    }
                }
            }
            // 从共享目标处启动
            else if (Program.AppActivationArguments.Kind is ExtendedActivationKind.ShareTarget)
            {
                ShareTargetActivatedEventArgs shareTargetActivatedEventArgs = Program.AppActivationArguments.Data.As<ShareTargetActivatedEventArgs>();
                ShareOperation shareOperation = shareTargetActivatedEventArgs.ShareOperation;
                shareOperation.ReportCompleted();

                if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> sharedFilesList = await shareOperation.Data.GetStorageItemsAsync();

                    if (sharedFilesList.Count > 0)
                    {
                        fileName = sharedFilesList[0].Path;

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            IsParseEmpty = false;
                            ResetResult();

                            (bool parseResult, PackageInformation packageInformation) parseResult = await Task.Run(async () =>
                            {
                                return await ParsePackagedAppAsync(fileName);
                            });

                            await UpdateResultAsync(parseResult);
                        }
                    }
                }
            }

            // 设置标题栏主题
            SetTitleBarTheme(Content.As<FrameworkElement>().ActualTheme);
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("getstoreapp:"), new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = global::Windows.ApplicationModel.Package.Current.Id.FamilyName
                    },
                    new ValueSet()
                    {
                        { "Parameter", "AppInstallSettings" }
                    });
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 选择安装包
        /// </summary>
        private async void OnOpenPackageClicked(object sender, RoutedEventArgs args)
        {
            bool hasSelectFile = false;

            try
            {
                FileOpenPicker fileOpenPicker = new(AppWindow.Id)
                {
                    SuggestedStartLocation = PickerLocationId.Desktop
                };

                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");
                fileOpenPicker.FileTypeFilter.Add(".appinstaller");

                if (await fileOpenPicker.PickSingleFileAsync() is PickFileResult pickFileResult)
                {
                    fileName = pickFileResult.Path;
                    hasSelectFile = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnOpenPackageClicked), 1, e);
            }

            if (hasSelectFile)
            {
                IsParseEmpty = false;
                ResetResult();

                (bool parseResult, PackageInformation packageInformation) parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
            }
        }

        /// <summary>
        /// 选择其他的安装包
        /// </summary>
        private async void OnOpenOtherPackageClicked(object sender, RoutedEventArgs args)
        {
            bool hasSelectFile = false;

            try
            {
                FileOpenPicker fileOpenPicker = new(AppWindow.Id)
                {
                    SuggestedStartLocation = PickerLocationId.Desktop
                };
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");
                fileOpenPicker.FileTypeFilter.Add(".appinstaller");

                if (await fileOpenPicker.PickSingleFileAsync() is PickFileResult pickFileResult)
                {
                    fileName = pickFileResult.Path;
                    hasSelectFile = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnOpenOtherPackageClicked), 1, e);
            }

            if (hasSelectFile)
            {
                ResetResult();

                (bool parseResult, PackageInformation packageInformation) parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
            }
        }

        /// <summary>
        /// 添加依赖包
        /// </summary>
        private async void OnAddDependencyClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                FileOpenPicker fileOpenPicker = new(AppWindow.Id)
                {
                    SuggestedStartLocation = PickerLocationId.Desktop
                };
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");

                if (await fileOpenPicker.PickMultipleFilesAsync() is IReadOnlyList<PickFileResult> pickFileResultList)
                {
                    foreach (PickFileResult pickFileResult in pickFileResultList)
                    {
                        (bool parseResult, DependencyAppInformation dependencyAppInformation) parseResult = await Task.Run(async () =>
                        {
                            return await ParseDependencyAppAsync(pickFileResult.Path);
                        });

                        if (parseResult.parseResult)
                        {
                            DependencyAppInformation dependencyAppInformation = parseResult.dependencyAppInformation;

                            InstallDependencyCollection.Add(new InstallDependencyModel()
                            {
                                DependencyName = Path.GetFileName(pickFileResult.Path),
                                DependencyVersion = dependencyAppInformation.Version is Version version ? version : new Version(),
                                DependencyPublisher = string.IsNullOrEmpty(dependencyAppInformation.PublisherDisplayName) ? NotAvailableString : dependencyAppInformation.PublisherDisplayName,
                                DependencyFullName = string.IsNullOrEmpty(dependencyAppInformation.PackageFullName) ? Convert.ToString(GuidHelper.CreateNewGuid()) : dependencyAppInformation.PackageFullName,
                                DependencyPath = pickFileResult.Path
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnAddDependencyClicked), 1, e);
            }
        }

        /// <summary>
        /// 清空依赖包
        /// </summary>
        private void OnClearDependencyClicked(object sender, RoutedEventArgs args)
        {
            InstallDependencyCollection.Clear();
        }

        /// <summary>
        /// 复制错误原因
        /// </summary>
        private async void OnCopyErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            if (ViewErrorInformationFlyout.IsOpen)
            {
                ViewErrorInformationFlyout.Hide();
            }

            if (!string.IsNullOrEmpty(InstallFailedInformation))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(InstallFailedInformation);

                await ShowNotificationAsync(new CopyPasteInstallerNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<Button>().Tag.As<Flyout>() is Flyout flyout && flyout.IsOpen)
            {
                flyout.Hide();
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallAppClicked(object sender, RoutedEventArgs args)
        {
            if (File.Exists(fileName))
            {
                CanDragFile = false;
                IsInstalling = true;
                IsInstallFailed = false;
                InstallProgressValue = 0;
                InstallStateString = PrepareInstallString;

                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    string extensionName = Path.GetExtension(fileName);

                    if (string.Equals(extensionName, ".appx", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msix", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".appxbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msixbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            if (string.Equals(PackageFamilyName, global::Windows.ApplicationModel.Package.Current.Id.FamilyName, StringComparison.OrdinalIgnoreCase))
                            {
                                await Task.Run(() =>
                                {
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(AppInstallSelfString, PackageName));
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });
                            }

                            AddPackageOptions addPackageOptions = new()
                            {
                                AllowUnsigned = AppInstallService.AllowUnsignedPackageValue,
                                ForceAppShutdown = AppInstallService.ForceAppShutdownValue,
                                ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdownValue,
                                TargetVolume = PackageVolume.GetDefault()
                            };

                            foreach (InstallDependencyModel installDependencyItem in InstallDependencyCollection)
                            {
                                addPackageOptions.DependencyPackageUris.Add(new Uri(installDependencyItem.DependencyPath));
                            }

                            // 安装目标应用，并获取安装进度
                            installPackageWithProgress = packageDeploymentManager.AddPackageByUriAsync(new Uri(fileName), addPackageOptions);

                            // 更新安装进度
                            installPackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress);
                            return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, await installPackageWithProgress, null);
                        }
                        // 安装失败显示失败信息
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnInstallAppClicked), 1, e);
                            return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, null, e);
                        }
                    }
                    else
                    {
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, null);
                    }
                });

                if (result && packageDeploymentResult is not null)
                {
                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                    {
                        // 更新应用安装状态
                        CanDragFile = true;
                        InstallProgressValue = 100;
                        IsInstallWaiting = false;
                        IsInstallFailed = false;
                        InstallFailedInformation = string.Empty;
                        IsAppInstalled = true;
                        InstallStateString = AppInstallSuccessfullyString;
                        AppInstalledState = AppInstalledNotNewVersionString;

                        await Task.Run(() =>
                        {
                            // 显示安装成功通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(AppInstallSuccessfully1String, PackageName));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                    else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                    {
                        string errorCode = packageDeploymentResult.Error is not null ? "0x" + Convert.ToString(packageDeploymentResult.Error.HResult, 16).ToUpperInvariant() : NotAvailableString;
                        string errorMessage = string.IsNullOrEmpty(packageDeploymentResult.ErrorText) ? packageDeploymentResult.Error is not null ? packageDeploymentResult.Error.Message : NotAvailableString : packageDeploymentResult.ErrorText;

                        // 更新应用安装状态
                        CanDragFile = true;
                        InstallProgressValue = 100;
                        IsInstallWaiting = false;
                        IsInstallFailed = true;
                        InstallFailedInformation = errorMessage;
                        IsAppInstalled = false;
                        InstallStateString = AppInstallFailedString;
                        AppInstalledState = AppNotInstallString;

                        await Task.Run(() =>
                        {
                            // 显示安装失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(AppInstallFailed1String, PackageName));
                            appNotificationBuilder.AddText(string.Format(AppInstallFailed2String, errorCode));
                            appNotificationBuilder.AddText(string.Format(AppInstallFailed3String, errorMessage));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                }
                else
                {
                    string errorCode = exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString;
                    string errorMessage = exception is not null ? exception.Message : NotAvailableString;

                    // 更新应用安装状态
                    CanDragFile = true;
                    InstallProgressValue = 100;
                    IsInstallWaiting = false;
                    IsInstallFailed = true;
                    InstallFailedInformation = errorMessage;
                    IsAppInstalled = false;
                    AppInstalledState = AppNotInstallString;

                    await Task.Run(() =>
                    {
                        // 显示安装失败通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(AppInstallFailed1String, PackageName));
                        appNotificationBuilder.AddText(string.Format(AppInstallFailed2String, errorCode));
                        appNotificationBuilder.AddText(string.Format(AppInstallFailed3String, errorMessage));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    });
                }

                // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                await Task.Delay(500);
                IsInstalling = false;
                InstallProgressValue = 0;
            }
        }

        /// <summary>
        /// 取消安装
        /// </summary>
        private async void OnCancelInstallClicked(object sender, RoutedEventArgs args)
        {
            if (installPackageWithProgress is not null)
            {
                IsCancelInstall = false;
                await Task.Run(installPackageWithProgress.Cancel);
                IsCancelInstall = true;
            }
        }

        /// <summary>
        /// 显示启动应用浮出控件
        /// </summary>
        private void OnStartClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender.As<FrameworkElement>());
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        private void OnStartAppClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                foreach (global::Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (string.Equals(package.Id.FamilyName, PackageFamilyName))
                    {
                        IReadOnlyList<AppListEntry> appListEntryList = package.GetAppListEntries();

                        if (appListEntryList.Count > 0)
                        {
                            await appListEntryList[0].LaunchAsync();
                            break;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 打开应用安装目录
        /// </summary>
        private void OnOpenAppInstalledFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                foreach (global::Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (string.Equals(package.Id.FamilyName, PackageFamilyName))
                    {
                        try
                        {
                            await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnOpenAppInstalledFolderClicked), 1, e);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 打开应用缓存目录
        /// </summary>
        private void OnOpenAppCachedFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(PackageFamilyName))
                {
                    try
                    {
                        if (Microsoft.Windows.Storage.ApplicationData.GetForPackageFamily(PackageFamilyName) is Microsoft.Windows.Storage.ApplicationData applicationData)
                        {
                            await Launcher.LaunchFolderAsync(applicationData.LocalFolder);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(OnOpenAppCachedFolderClicked), 1, e);
                    }
                }
            });
        }

        /// <summary>
        /// 打开应用安装程序文件链接
        /// </summary>
        private void OnOpenAppInstallerSourceLinkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(AppInstallerSourceLink))
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri(AppInstallerSourceLink));
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
            });
        }

        /// <summary>
        /// 打开应用包链接
        /// </summary>
        private void OnOpenPackageSourceLinkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(PackageSourceLink))
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri(PackageSourceLink));
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
            });
        }

        #endregion 第五部分：窗口内容挂载的事件

        #region 第六部分：自定义事件

        /// <summary>
        /// 应用安装状态发生改变时触发的事件
        /// </summary>
        private void OnPackageInstallProgress(IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> result, PackageDeploymentProgress progress)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                if (progress.Status is PackageDeploymentProgressStatus.Queued)
                {
                    IsInstalling = true;
                    IsInstallWaiting = false; // TODO：windows app sdk api 问题
                    //InstallStateString = WaitInstallString;
                    InstallProgressValue = progress.Progress * 100;
                    InstallStateString = string.Format(InstallProgressString, Convert.ToInt32(progress.Progress * 100));
                }
                else if (progress.Status is PackageDeploymentProgressStatus.InProgress)
                {
                    IsInstalling = true;
                    IsInstallWaiting = false;
                    InstallProgressValue = progress.Progress * 100;
                    InstallStateString = string.Format(InstallProgressString, Convert.ToInt32(progress.Progress * 100));
                }
            });
        }

        #endregion 第六部分：自定义事件

        #region 第七部分：窗口及内容属性设置

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public void SetWindowTheme()
        {
            WindowTheme = string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]) ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark : Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        private void SetSystemBackdrop()
        {
            if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[1]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.Base);
                VisualStateManager.GoToState(InstallerPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[2]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
                VisualStateManager.GoToState(InstallerPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[3]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
                VisualStateManager.GoToState(InstallerPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[4]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
                VisualStateManager.GoToState(InstallerPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[5]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
                VisualStateManager.GoToState(InstallerPage, "BackgroundTransparent", false);
            }
            else
            {
                WindowSystemBackdrop = null;
                VisualStateManager.GoToState(InstallerPage, "BackgroundDefault", false);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private void SetClassicMenuTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            if (theme is ElementTheme.Light)
            {
                titleBar.PreferredTheme = TitleBarTheme.Light;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                titleBar.PreferredTheme = TitleBarTheme.Dark;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 设置所有弹出控件主题
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(FlyoutPresenter)), DynamicWindowsRuntimeCast(typeof(Grid))]
        private void SetPopupControlTheme(ElementTheme elementTheme)
        {
            foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
            {
                popup.RequestedTheme = elementTheme;

                try
                {
                    if (popup.Child is FlyoutPresenter flyoutPresenter)
                    {
                        flyoutPresenter.RequestedTheme = elementTheme;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }

                try
                {
                    if (popup.Child is Grid grid && grid.Name is "OuterOverflowContentRootV2")
                    {
                        grid.RequestedTheme = elementTheme;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            }
        }

        #endregion 第七部分：窗口及内容属性设置

        #region 第八部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private nint InstallerWindowSubClassProc(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, uint uIdSubclass, nint dwRefData)
        {
            switch (Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Content is not null && Content.XamlRoot is not null)
                        {
                            PointInt32 screenPoint = new(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = Environment.OSVersion.Version.Build > 22000 ? new Point(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) : new Point(localPoint.X, localPoint.Y)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU && lParam is (nint)VirtualKey.Space)
                        {
                            return 0;
                        }
                        break;
                    }
                // 提升权限时允许应用接收拖放文件消息
                case WindowMessage.WM_DROPFILES:
                    {
                        Task.Run(() =>
                        {
                            List<string> filesList = [];
                            char[] dragFileCharArray = new char[260];
                            uint filesCount = Shell32Library.DragQueryFile(wParam, 0xffffffffu, null, 0);

                            for (uint index = 0; index < filesCount; index++)
                            {
                                Array.Clear(dragFileCharArray, 0, dragFileCharArray.Length);
                                if (Shell32Library.DragQueryFile(wParam, index, dragFileCharArray, (uint)dragFileCharArray.Length) > 0)
                                {
                                    filesList.Add(new string(dragFileCharArray).Replace("\0", string.Empty));
                                }
                            }

                            Shell32Library.DragQueryPoint(wParam, out PointInt32 point);
                            Shell32Library.DragFinish(wParam);

                            if (filesList.Count > 0)
                            {
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    await DealElevatedDragDropAsync(filesList[0]);
                                });
                            }
                        });

                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第八部分：窗口过程

        #region 第九部分：显示应用通知

        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            try
            {
                if (teachingTip is not null && InstallerPage.Content.As<Grid>() is Grid grid)
                {
                    grid.Children.Add(teachingTip);

                    teachingTip.IsOpen = true;
                    await Task.Delay(duration);
                    teachingTip.IsOpen = false;

                    // 应用内通知关闭动画显示耗费 300 ms
                    await Task.Delay(300);
                    grid.Children.Remove(teachingTip);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }

        #endregion 第九部分：显示应用通知

        #region 第十部分：解析应用包信息

        /// <summary>
        /// 解析应用包
        /// </summary>
        private async Task<(bool parseResult, PackageInformation packageInformation)> ParsePackagedAppAsync(string filePath)
        {
            bool parseResult = false;
            PackageInformation packageInformation = new();

            try
            {
                if (File.Exists(filePath))
                {
                    // 获取文件的扩展文件名称
                    string extensionName = Path.GetExtension(filePath);

                    // 第一部分：解析以 appx 或 msix 格式结尾的单个应用包
                    if (string.Equals(extensionName, ".appx", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msix", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (appxFactory is not null && appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                            {
                                parseResult = true;

                                // 解析安装包所有文件
                                Dictionary<string, IAppxFile> appxFileDict = ParsePackagePayloadFiles(appxPackageReader);

                                // 解析资源包清单文件
                                ManifestInformation manifestInformation = ParsePackageManifest(appxPackageReader, false, null);

                                packageInformation.PackageFileType = PackageFileType.Package;
                                packageInformation.CapabilitiesList = manifestInformation.CapabilitiesList;
                                packageInformation.ProcessorArchitecture = manifestInformation.ProcessorArchitecture;
                                packageInformation.PackageFamilyName = manifestInformation.PackageFamilyName;
                                packageInformation.PackageFullName = manifestInformation.PackageFullName;
                                packageInformation.Version = manifestInformation.Version;
                                packageInformation.IsFramework = manifestInformation.IsFramework;
                                packageInformation.Description = manifestInformation.Description;
                                packageInformation.DisplayName = manifestInformation.DisplayName;
                                packageInformation.Logo = manifestInformation.Logo;
                                packageInformation.PublisherDisplayName = manifestInformation.PublisherDisplayName;
                                packageInformation.DependencyList = manifestInformation.DependencyList;
                                packageInformation.TargetDeviceFamilyList = manifestInformation.TargetDeviceFamilyList;
                                packageInformation.ApplicationList = manifestInformation.ApplicationList;
                                packageInformation.LanguageList = manifestInformation.LanguageList;

                                // 获取应用包图标
                                if (appxFileDict.Count > 0 && !string.IsNullOrEmpty(packageInformation.Logo))
                                {
                                    IStream imageFileStream = GetPackageLogo(packageInformation.Logo, appxFileDict);
                                    packageInformation.ImageLogo = imageFileStream;
                                }
                            }

                            randomAccessStream?.Dispose();
                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                            randomAccessStream = null;
                            fileStream = null;
                        }

                        if (packageInformation.Version is not null)
                        {
                            foreach (global::Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                            {
                                if (package.Id.FullName.Contains(packageInformation.PackageFamilyName))
                                {
                                    Version installedVersion = new(package.Id.Version.Major, package.Id.Version.Major, package.Id.Version.Build, package.Id.Version.Revision);
                                    packageInformation.AppInstalledState = packageInformation.Version > installedVersion ? AppInstalledNotNewVersionString : AppInstalledNewVersionString;
                                    packageInformation.IsAppInstalled = true;
                                    break;
                                }
                            }

                            if (string.IsNullOrEmpty(packageInformation.AppInstalledState))
                            {
                                packageInformation.AppInstalledState = AppNotInstallString;
                            }
                        }
                        else
                        {
                            packageInformation.AppInstalledState = NotAvailableString;
                        }
                    }

                    // 解析以 appxbundle 或 msixbundle 格式结尾的应用包
                    else if (string.Equals(extensionName, ".appxbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (appxBundleFactory is not null && appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0 && appxBundleReader.GetManifest(out IAppxBundleManifestReader appxBundleManifestReader) is 0)
                            {
                                PackageManifestInformation packageManifestInformation = ParsePackageBundleManifestInfo(appxBundleManifestReader);

                                if (appxFactory is not null)
                                {
                                    parseResult = true;

                                    // 解析应用
                                    if (packageManifestInformation.ApplicationDict is not null)
                                    {
                                        packageInformation.LanguageList = packageManifestInformation.LanguageList;
                                        string applicationFileName = ParsePackageBundleCompatibleFile(packageManifestInformation.ApplicationDict);
                                        string architecture = ParsePackageBundleArchitecture(packageManifestInformation.ApplicationDict);
                                        packageInformation.ProcessorArchitecture = architecture;

                                        if (!string.IsNullOrEmpty(applicationFileName))
                                        {
                                            appxBundleReader.GetPayloadPackage(applicationFileName, out IAppxFile applicationFile);

                                            if (applicationFile.GetStream(out IStream applicationFileStream) is 0 && appxFactory.CreatePackageReader2(applicationFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                            {
                                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(applicationFileStream, CreateComInterfaceFlags.None));

                                                ManifestInformation manifestInformation = ParsePackageManifest(appxPackageReader, true, packageInformation.LanguageList);

                                                packageInformation.PackageFileType = PackageFileType.Package;
                                                packageInformation.CapabilitiesList = manifestInformation.CapabilitiesList;
                                                packageInformation.PackageFamilyName = manifestInformation.PackageFamilyName;
                                                packageInformation.PackageFullName = manifestInformation.PackageFullName;
                                                packageInformation.Version = manifestInformation.Version;
                                                packageInformation.IsFramework = manifestInformation.IsFramework;
                                                packageInformation.Description = manifestInformation.Description;
                                                packageInformation.DisplayName = manifestInformation.DisplayName;
                                                packageInformation.Logo = manifestInformation.Logo;
                                                packageInformation.PublisherDisplayName = manifestInformation.PublisherDisplayName;
                                                packageInformation.DependencyList = manifestInformation.DependencyList;
                                                packageInformation.TargetDeviceFamilyList = manifestInformation.TargetDeviceFamilyList;
                                                packageInformation.ApplicationList = manifestInformation.ApplicationList;
                                            }
                                        }
                                    }

                                    // 从资源文件中查找符合的图标
                                    if (packageManifestInformation.ScaleResourceList is not null)
                                    {
                                        // 获取应用包图标
                                        if (!string.IsNullOrEmpty(packageInformation.Logo))
                                        {
                                            Dictionary<string, IAppxFile> scaleBundleFileDict = ParsePackageBundleScaleFiles(appxBundleReader, packageManifestInformation.ScaleResourceList);
                                            IStream imageFileStream = await GetPackageBundleLogoAsync(packageInformation.Logo, scaleBundleFileDict);
                                            packageInformation.ImageLogo = imageFileStream;
                                        }
                                    }
                                }
                            }

                            randomAccessStream?.Dispose();
                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                            randomAccessStream = null;
                            fileStream = null;
                        }

                        if (packageInformation.Version is not null)
                        {
                            foreach (global::Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                            {
                                if (string.Equals(package.Id.FamilyName, packageInformation.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    Version installedVersion = new(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision);
                                    packageInformation.AppInstalledState = packageInformation.Version > installedVersion ? AppInstalledNotNewVersionString : AppInstalledNewVersionString; packageInformation.IsAppInstalled = true;
                                    packageInformation.IsAppInstalled = true;
                                    break;
                                }
                            }

                            if (string.IsNullOrEmpty(packageInformation.AppInstalledState))
                            {
                                packageInformation.AppInstalledState = AppNotInstallString;
                            }
                        }
                        else
                        {
                            packageInformation.AppInstalledState = NotAvailableString;
                        }
                    }

                    // 解析以 appinstaller 格式结尾的应用安装文件
                    else if (string.Equals(extensionName, ".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        // 解析应用安装文件
                        XmlLoadSettings xmlLoadSettings = new()
                        {
                            ElementContentWhiteSpace = true
                        };

                        XmlDocument xmlDocument = await XmlDocument.LoadFromFileAsync(await StorageFile.GetFileFromPathAsync(filePath), xmlLoadSettings);

                        if (xmlDocument is not null)
                        {
                            parseResult = true;
                            packageInformation.PackageFileType = PackageFileType.AppInstaller;

                            XmlNodeList appInstallerNodeList = xmlDocument.GetElementsByTagName("AppInstaller");

                            if (appInstallerNodeList.Count > 0 && appInstallerNodeList[0].Attributes.GetNamedItem("Uri") is IXmlNode appInstallerUriNode)
                            {
                                packageInformation.AppInstallerSourceLink = appInstallerUriNode.InnerText;
                                packageInformation.IsAppInstallerSourceLinkExisted = !string.IsNullOrEmpty(appInstallerUriNode.InnerText);
                            }

                            XmlNodeList mainPackageNodeList = xmlDocument.GetElementsByTagName("MainPackage");
                            XmlNodeList mainBundleNodeList = xmlDocument.GetElementsByTagName("MainBundle");

                            // 应用安装包
                            if (mainPackageNodeList.Count > 0)
                            {
                                packageInformation.PackageType = PackageString;

                                if (mainPackageNodeList[0].Attributes.GetNamedItem("Name") is IXmlNode nameNode)
                                {
                                    packageInformation.DisplayName = nameNode.InnerText;
                                }

                                if (mainPackageNodeList[0].Attributes.GetNamedItem("Publisher") is IXmlNode publisherNode)
                                {
                                    packageInformation.PublisherDisplayName = publisherNode.InnerText;
                                }

                                if (mainPackageNodeList[0].Attributes.GetNamedItem("Version") is IXmlNode versionNode)
                                {
                                    packageInformation.Version = new Version(versionNode.InnerText);
                                }

                                if (mainPackageNodeList[0].Attributes.GetNamedItem("Uri") is IXmlNode packageUriNode)
                                {
                                    packageInformation.PackageSourceLink = packageUriNode.InnerText;
                                    packageInformation.IsPackageSourceLinkExisted = true;
                                }
                            }
                            // 应用捆绑包
                            else if (mainBundleNodeList.Count > 0)
                            {
                                packageInformation.PackageType = PackageBundleString;

                                if (mainBundleNodeList[0].Attributes.GetNamedItem("Name") is IXmlNode nameNode)
                                {
                                    packageInformation.DisplayName = nameNode.InnerText;
                                }

                                if (mainBundleNodeList[0].Attributes.GetNamedItem("Publisher") is IXmlNode publisherNode)
                                {
                                    packageInformation.PublisherDisplayName = publisherNode.InnerText;
                                }

                                if (mainBundleNodeList[0].Attributes.GetNamedItem("Version") is IXmlNode versionNode)
                                {
                                    packageInformation.Version = new Version(versionNode.InnerText);
                                }

                                if (mainBundleNodeList[0].Attributes.GetNamedItem("Uri") is IXmlNode packageUriNode)
                                {
                                    packageInformation.PackageSourceLink = packageUriNode.InnerText;
                                    packageInformation.IsPackageSourceLinkExisted = !string.IsNullOrEmpty(packageUriNode.InnerText);
                                }
                            }

                            XmlNodeList updateSettingsNodeList = xmlDocument.GetElementsByTagName("UpdateSettings");

                            if (updateSettingsNodeList.Count > 0)
                            {
                                packageInformation.IsUpdateSettingsExisted = true;

                                foreach (IXmlNode updateSettingsNode in updateSettingsNodeList[0].ChildNodes)
                                {
                                    if (updateSettingsNode.NodeName is "OnLaunch")
                                    {
                                        if (updateSettingsNode.Attributes.GetNamedItem("HoursBetweenUpdateChecks") is IXmlNode hoursBetweenUpdateChecksNode)
                                        {
                                            packageInformation.HoursBetweenUpdateChecks = Convert.ToInt32(hoursBetweenUpdateChecksNode.InnerText);
                                        }

                                        if (updateSettingsNode.Attributes.GetNamedItem("UpdateBlocksActivation") is IXmlNode updateBlocksActivationNode)
                                        {
                                            packageInformation.UpdateBlocksActivation = Convert.ToBoolean(updateBlocksActivationNode.InnerText);
                                        }

                                        if (updateSettingsNode.Attributes.GetNamedItem("ShowPrompt") is IXmlNode showPromptNode)
                                        {
                                            packageInformation.ShowPrompt = Convert.ToBoolean(showPromptNode.InnerText);
                                        }
                                    }
                                    else if (updateSettingsNode.NodeName is "ForceUpdateFromAnyVersion")
                                    {
                                        packageInformation.ForceUpdateFromAnyVersion = Convert.ToBoolean(updateSettingsNode.InnerText);
                                    }
                                    else if (updateSettingsNode.NodeName is "AutomaticBackgroundTask")
                                    {
                                        packageInformation.AutomaticBackgroundTask = true;
                                    }
                                }
                            }
                            else
                            {
                                packageInformation.IsUpdateSettingsExisted = false;
                            }

                            XmlNodeList packageNodeList = xmlDocument.GetElementsByTagName("Package");
                            packageInformation.DependencyList = [];

                            foreach (IXmlNode packageNode in packageNodeList)
                            {
                                DependencyInformation dependencyInformation = new();

                                if (packageNode.Attributes.GetNamedItem("Name") is IXmlNode nameNode)
                                {
                                    dependencyInformation.DependencyName = nameNode.InnerText;
                                }

                                if (packageNode.Attributes.GetNamedItem("Publisher") is IXmlNode publisherNode)
                                {
                                    dependencyInformation.DependencyPublisher = publisherNode.InnerText;
                                }

                                if (packageNode.Attributes.GetNamedItem("Version") is IXmlNode versionNode)
                                {
                                    dependencyInformation.DependencyVersion = new Version(versionNode.InnerText);
                                }

                                if (packageNode.Attributes.GetNamedItem("ProcessorArchitecture") is IXmlNode processorArchitectureNode)
                                {
                                    dependencyInformation.ProcessorArchitecture = processorArchitectureNode.InnerText;
                                }
                                else
                                {
                                    dependencyInformation.ProcessorArchitecture = NotAvailableString;
                                }

                                if (packageNode.Attributes.GetNamedItem("Uri") is IXmlNode uriNode)
                                {
                                    dependencyInformation.Uri = new Uri(uriNode.InnerText);
                                }

                                packageInformation.DependencyList.Add(dependencyInformation);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(ParsePackagedAppAsync), 1, e);
            }

            return ValueTuple.Create(parseResult, packageInformation);
        }

        /// <summary>
        /// 解析依赖应用包
        /// </summary>
        private async Task<(bool parseResult, DependencyAppInformation dependencyAppInformation)> ParseDependencyAppAsync(string filePath)
        {
            bool parseResult = false;
            DependencyAppInformation dependencyAppInformation = null;

            try
            {
                if (File.Exists(filePath))
                {
                    // 获取文件的扩展文件名称
                    string extensionName = Path.GetExtension(filePath);

                    // 第一部分：解析以 appx 或 msix 格式结尾的单个应用包
                    if (string.Equals(extensionName, ".appx", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msix", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (appxFactory is not null && appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                            {
                                parseResult = true;

                                // 解析资源包清单文件
                                dependencyAppInformation = ParseDependencyPackageManifest(appxPackageReader);
                            }

                            randomAccessStream?.Dispose();
                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                            randomAccessStream = null;
                            fileStream = null;
                        }
                    }

                    // 解析以 appxbundle 或 msixbundle 格式结尾的应用包
                    else if (string.Equals(extensionName, ".appxbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out nint appxBundleFactoryPtr) is 0)
                        {
                            IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                            if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                            {
                                if (appxBundleFactory is not null && appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0)
                                {
                                    parseResult = true;

                                    // 解析资源包清单文件
                                    dependencyAppInformation = ParseDependencyPackageBundleManifest(appxBundleReader);
                                }

                                randomAccessStream?.Dispose();
                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                                randomAccessStream = null;
                                fileStream = null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(ParseDependencyAppAsync), 1, e);
            }

            return ValueTuple.Create(parseResult, dependencyAppInformation);
        }

        /// <summary>
        /// 获取应用程序入口信息
        /// </summary>
        private List<ApplicationModel> ParsePackageApplication(IAppxManifestReader3 appxManifestReader, ResourceManagement resourceManagement)
        {
            List<ApplicationModel> applicationList = [];

            if (appxManifestReader.GetApplications(out IAppxManifestApplicationsEnumerator appxManifestApplicationsEnumerator) is 0)
            {
                while (appxManifestApplicationsEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxManifestApplicationsEnumerator.GetCurrent(out IAppxManifestApplication appxManifestApplication);
                    appxManifestApplication.GetStringValue("Description", out string description);
                    appxManifestApplication.GetStringValue("EntryPoint", out string entryPoint);
                    appxManifestApplication.GetStringValue("Executable", out string executable);
                    appxManifestApplication.GetStringValue("ID", out string id);
                    appxManifestApplication.GetAppUserModelId(out string appUserModelId);

                    description = GetLocalizedString(description, resourceManagement);

                    ApplicationModel application = new()
                    {
                        AppDescription = string.IsNullOrEmpty(description) ? NotAvailableString : description,
                        EntryPoint = string.IsNullOrEmpty(entryPoint) ? NotAvailableString : entryPoint,
                        Executable = string.IsNullOrEmpty(executable) ? NotAvailableString : executable,
                        AppID = string.IsNullOrEmpty(id) ? NotAvailableString : id,
                        AppUserModelId = string.IsNullOrEmpty(appUserModelId) ? NotAvailableString : appUserModelId
                    };

                    applicationList.Add(application);
                    appxManifestApplicationsEnumerator.MoveNext(out _);
                }
            }

            applicationList.Sort((item1, item2) => item1.AppID.CompareTo(item2.AppID));
            return applicationList;
        }

        /// <summary>
        /// 解析应用包的功能
        /// </summary>
        private List<string> ParsePackageCapability(IAppxManifestReader3 appxManifestReader)
        {
            List<string> capabilityList = [];

            if (appxManifestReader.GetCapabilitiesByCapabilityClass(APPX_CAPABILITY_CLASS_TYPE.APPX_CAPABILITY_CLASS_ALL, out IAppxManifestCapabilitiesEnumerator appxManifestCapabilitiesEnumerator) is 0)
            {
                while (appxManifestCapabilitiesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxManifestCapabilitiesEnumerator.GetCurrent(out string capability);
                    capabilityList.Add(capability);
                    appxManifestCapabilitiesEnumerator.MoveNext(out _);
                }
            }

            capabilityList.Sort();
            return capabilityList;
        }

        /// <summary>
        /// 解析应用包的依赖信息
        /// </summary>
        private List<DependencyInformation> ParsePackageDependencies(IAppxManifestReader3 appxManifestReader)
        {
            List<DependencyInformation> dependencyList = [];

            // 获取应用包定义的静态依赖项列表
            if (appxManifestReader.GetPackageDependencies(out IAppxManifestPackageDependenciesEnumerator dependenciesEnumerator) is 0)
            {
                while (dependenciesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    if (dependenciesEnumerator.GetCurrent(out IAppxManifestPackageDependency2 appxManifestPackageDependency) is 0)
                    {
                        appxManifestPackageDependency.GetMinVersion(out ulong dependencyMinVersion);
                        appxManifestPackageDependency.GetName(out string dependencyName);
                        appxManifestPackageDependency.GetPublisher(out string dependencyPublisher);
                        appxManifestPackageDependency.GetMaxMajorVersionTested(out ushort dependencyMaxMajorVersionTested);

                        DependencyInformation dependencyInformation = new();

                        PackageVersion dependencyMinPackageVersion = new(dependencyMinVersion);
                        dependencyInformation.DependencyMinVersion = new Version(dependencyMinPackageVersion.Major, dependencyMinPackageVersion.Minor, dependencyMinPackageVersion.Build, dependencyMinPackageVersion.Revision);
                        dependencyInformation.DependencyName = dependencyName;
                        dependencyInformation.DependencyPublisher = dependencyPublisher;

                        PackageVersion dependencyMaxPackageMajorVersionTested = new(dependencyMaxMajorVersionTested);
                        dependencyInformation.DependencyMaxMajorVersionTested = new Version(dependencyMaxPackageMajorVersionTested.Major, dependencyMaxPackageMajorVersionTested.Minor, dependencyMaxPackageMajorVersionTested.Build, dependencyMaxPackageMajorVersionTested.Revision);
                        dependencyList.Add(dependencyInformation);
                    }

                    dependenciesEnumerator.MoveNext(out _);
                }
            }

            return dependencyList;
        }

        /// <summary>
        /// 解析应用包清单
        /// </summary>
        private ManifestInformation ParsePackageManifest(IAppxPackageReader appxPackageReader, bool isBundle, List<string> bundleLanguageList)
        {
            ManifestInformation manifestInformation = new();

            try
            {
                // 分段 4：读取应用包清单
                if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
                {
                    // 获取应用包定义的功能列表
                    manifestInformation.CapabilitiesList = ParsePackageCapability(appxManifestReader);

                    // 获取应用包定义的静态依赖项列表
                    manifestInformation.DependencyList = ParsePackageDependencies(appxManifestReader);

                    // 获取应用包定义的包标识符
                    if (appxManifestReader.GetPackageId(out IAppxManifestPackageId2 packageId) is 0)
                    {
                        packageId.GetArchitecture2(out ProcessorArchitecture architecture);
                        packageId.GetPackageFamilyName(out string packageFamilyName);
                        packageId.GetPackageFullName(out string packageFullName);
                        packageId.GetVersion(out ulong version);

                        if (!isBundle)
                        {
                            manifestInformation.ProcessorArchitecture = Convert.ToString(architecture);
                        }

                        manifestInformation.PackageFamilyName = packageFamilyName;
                        manifestInformation.PackageFullName = packageFullName;

                        PackageVersion packageVersion = new(version);
                        manifestInformation.Version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
                    }

                    // 获取包的目标设备系列
                    manifestInformation.TargetDeviceFamilyList = ParsePackageTargetDeviceFamily(appxManifestReader);

                    // 查找符合显示的语言
                    if (!isBundle)
                    {
                        // 获取应用包定义的语言资源
                        List<string> languageList = ParsePackageLanguage(appxManifestReader);
                        manifestInformation.LanguageList = languageList;
                    }
                    else
                    {
                        manifestInformation.LanguageList = bundleLanguageList;
                    }

                    string currentLanguage = GetSpecifiedLanguage(manifestInformation.LanguageList);

                    // 获取包的所有文件信息
                    Dictionary<string, IAppxFile> appxFileDict = ParsePackagePayloadFiles(appxPackageReader);
                    ResourceManagement resourceManagement = null;

                    if (appxFileDict.Count > 0 && GetPackageResourceFileStream(appxFileDict) is IStream resourceFileStream)
                    {
                        ShCoreLibrary.CreateRandomAccessStreamOverStream(resourceFileStream, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out nint ppv);
                        RandomAccessStreamOverStream randomAccessStreamOverStream = RandomAccessStreamOverStream.FromAbi(ppv);
                        Stream resourceStream = randomAccessStreamOverStream.AsStream();
                        string tempFilename = Path.GetTempFileName();
                        using (FileStream fileStream = new(tempFilename, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                        randomAccessStreamOverStream.Dispose();
                        Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));

                        // 获取包的资源信息
                        ResourceManager resourceManager = new(tempFilename);
                        ResourceContext defaultResourceContext = resourceManager.CreateResourceContext();
                        ResourceContext currentResourceContext = resourceManager.CreateResourceContext();
                        defaultResourceContext.QualifierValues["Language"] = Convert.ToString(LanguageService.DefaultAppLanguage);
                        currentResourceContext.QualifierValues["Language"] = Convert.ToString(currentLanguage);
                        resourceManagement = new()
                        {
                            ResourceMap = resourceManager.MainResourceMap,
                            DefaultResourceContext = defaultResourceContext,
                            CurrentResourceContext = currentResourceContext
                        };

                        try
                        {
                            File.Delete(tempFilename);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(ParsePackageManifest), 1, e);
                        }
                    }

                    // 获取应用入口信息
                    manifestInformation.ApplicationList = ParsePackageApplication(appxManifestReader, resourceManagement);

                    // 获取应用包的属性
                    if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                    {
                        packageProperties.GetBoolValue("Framework", out bool isFramework);

                        packageProperties.GetStringValue("Description", out string description);
                        packageProperties.GetStringValue("DisplayName", out string displayName);
                        packageProperties.GetStringValue("Logo", out string logo);
                        packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);

                        description = GetLocalizedString(description, resourceManagement);
                        displayName = GetLocalizedString(displayName, resourceManagement);
                        publisherDisplayName = GetLocalizedString(publisherDisplayName, resourceManagement);

                        manifestInformation.IsFramework = isFramework;
                        manifestInformation.Description = description;
                        manifestInformation.DisplayName = displayName;
                        manifestInformation.Logo = logo;
                        manifestInformation.PublisherDisplayName = publisherDisplayName;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(ParsePackageManifest), 2, e);
            }

            return manifestInformation;
        }

        /// <summary>
        /// 解析应用包的所有文件
        /// </summary>
        private Dictionary<string, IAppxFile> ParsePackagePayloadFiles(IAppxPackageReader appxPackageReader)
        {
            Dictionary<string, IAppxFile> fileDict = [];

            if (appxPackageReader.GetPayloadFiles(out IAppxFilesEnumerator appxFilesEnumerator) is 0)
            {
                while (appxFilesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxFilesEnumerator.GetCurrent(out IAppxFile appxFile);
                    appxFile.GetName(out string packageFileName);
                    fileDict.TryAdd(packageFileName, appxFile);

                    appxFilesEnumerator.MoveNext(out _);
                }
            }

            return fileDict;
        }

        /// <summary>
        /// 解析应用包定义的语言
        /// </summary>
        private List<string> ParsePackageLanguage(IAppxManifestReader3 appxManifestReader)
        {
            List<string> languageList = [];

            if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
            {
                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language) && !languageList.Contains(language))
                    {
                        languageList.Add(language);
                    }

                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                }
            }

            languageList.Sort();
            return languageList;
        }

        /// <summary>
        /// 获取程序包面向的设备系列信息
        /// </summary>
        private List<TargetDeviceFamilyModel> ParsePackageTargetDeviceFamily(IAppxManifestReader3 appxManifestReader)
        {
            List<TargetDeviceFamilyModel> targetDeviceFamilyList = [];

            if (appxManifestReader.GetTargetDeviceFamilies(out IAppxManifestTargetDeviceFamiliesEnumerator appxManifestTargetDeviceFamiliesEnumerator) is 0)
            {
                while (appxManifestTargetDeviceFamiliesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxManifestTargetDeviceFamiliesEnumerator.GetCurrent(out IAppxManifestTargetDeviceFamily appxManifestTargetDeviceFamily);
                    TargetDeviceFamilyModel targetDeviceFamily = new();
                    appxManifestTargetDeviceFamily.GetName(out string targetDeviceName);
                    appxManifestTargetDeviceFamily.GetMinVersion(out ulong minVersion);
                    appxManifestTargetDeviceFamily.GetMaxVersionTested(out ulong maxVersionTested);

                    targetDeviceFamily.TargetDeviceName = targetDeviceName;
                    PackageVersion minPackageVersion = new(minVersion);
                    targetDeviceFamily.MinVersion = new Version(minPackageVersion.Major, minPackageVersion.Minor, minPackageVersion.Build, minPackageVersion.Revision);
                    PackageVersion maxPackageVersionTested = new(maxVersionTested);
                    targetDeviceFamily.MaxVersionTested = new Version(maxPackageVersionTested.Major, maxPackageVersionTested.Minor, maxPackageVersionTested.Build, maxPackageVersionTested.Revision);

                    targetDeviceFamilyList.Add(targetDeviceFamily);
                    appxManifestTargetDeviceFamiliesEnumerator.MoveNext(out _);
                }
            }

            targetDeviceFamilyList.Sort((item1, item2) => item1.TargetDeviceName.CompareTo(item2.TargetDeviceName));
            return targetDeviceFamilyList;
        }

        /// <summary>
        /// 解析依赖包应用信息
        /// </summary>
        private DependencyAppInformation ParseDependencyPackageManifest(IAppxPackageReader appxPackageReader)
        {
            DependencyAppInformation dependencyAppInformation = new();

            try
            {
                // 分段 4：读取应用包清单
                if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0 && appxManifestReader.GetPackageId(out IAppxManifestPackageId2 appxManifestPackageId) is 0)
                {
                    appxManifestPackageId.GetPackageFullName(out string packageFullName);
                    appxManifestPackageId.GetVersion(out ulong version);
                    appxManifestPackageId.GetPublisher(out string publisher);

                    dependencyAppInformation.PackageFullName = packageFullName;
                    dependencyAppInformation.PublisherDisplayName = publisher;

                    PackageVersion packageVersion = new(version);
                    dependencyAppInformation.Version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(ParseDependencyPackageManifest), 1, e);
            }

            return dependencyAppInformation;
        }

        /// <summary>
        /// 解析依赖捆绑包应用信息
        /// </summary>
        private DependencyAppInformation ParseDependencyPackageBundleManifest(IAppxBundleReader appxBundleReader)
        {
            DependencyAppInformation dependencyAppInformation = new();

            try
            {
                // 分段 4：读取应用包清单
                if (appxBundleReader.GetManifest(out IAppxBundleManifestReader appxBundleManifestReader) is 0 && appxBundleManifestReader.GetPackageId(out IAppxManifestPackageId2 appxManifestPackageId) is 0)
                {
                    appxManifestPackageId.GetPackageFullName(out string packageFullName);
                    appxManifestPackageId.GetVersion(out ulong version);
                    appxManifestPackageId.GetPublisher(out string publisher);

                    dependencyAppInformation.PackageFullName = packageFullName;
                    dependencyAppInformation.PublisherDisplayName = publisher;

                    PackageVersion packageVersion = new(version);
                    dependencyAppInformation.Version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(ParseDependencyPackageBundleManifest), 1, e);
            }

            return dependencyAppInformation;
        }

        /// <summary>
        /// 解析应用包支持的处理器架构
        /// </summary>
        private string ParsePackageBundleArchitecture(Dictionary<ProcessorArchitecture, string> applicationDict)
        {
            string architecture = string.Empty;

            if (applicationDict.ContainsKey(ProcessorArchitecture.X86))
            {
                architecture = Convert.ToString(ProcessorArchitecture.X86);
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.X64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += Convert.ToString(ProcessorArchitecture.X64);
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Arm))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += Convert.ToString(ProcessorArchitecture.Arm);
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Arm64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += Convert.ToString(ProcessorArchitecture.Arm64);
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Neutral))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += Convert.ToString(ProcessorArchitecture.Neutral);
            }

            if (string.IsNullOrEmpty(architecture))
            {
                architecture = NotAvailableString;
            }

            return string.Format(BundleHeaderString, architecture);
        }

        /// <summary>
        /// 解析应用捆绑包适合主机系统的文件
        /// </summary>
        private string ParsePackageBundleCompatibleFile(Dictionary<ProcessorArchitecture, string> applicationDict)
        {
            Architecture osArchitecture = RuntimeInformation.ProcessArchitecture;
            string appxFile = null;
            if (applicationDict.Count is 0)
            {
                return null;
            }

            if (osArchitecture is Architecture.X86)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.X86, out appxFile);
            }
            else if (osArchitecture is Architecture.X64)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.X64, out appxFile);
            }
            else if (osArchitecture is Architecture.Arm64)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.Arm64, out appxFile);
            }
            else if (osArchitecture is Architecture.Arm)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.Arm, out appxFile);
            }
            else
            {
                foreach (KeyValuePair<ProcessorArchitecture, string> bundleFileItem in applicationDict)
                {
                    appxFile = bundleFileItem.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(appxFile))
            {
                foreach (KeyValuePair<ProcessorArchitecture, string> applicationItem in applicationDict)
                {
                    appxFile = applicationItem.Value;
                    break;
                }
            }
            return appxFile;
        }

        /// <summary>
        /// 解析捆绑包每个包信息
        /// </summary>
        private PackageManifestInformation ParsePackageBundleManifestInfo(IAppxBundleManifestReader appxBundleManifestReader)
        {
            PackageManifestInformation packageManifestInformation = new();
            Dictionary<ProcessorArchitecture, string> applicationDict = [];
            List<string> languageList = [];
            List<string> scaleResourceList = [];

            if (appxBundleManifestReader.GetPackageInfoItems(out IAppxBundleManifestPackageInfoEnumerator appxBundleManifestPackageInfoEnumerator) is 0)
            {
                while (appxBundleManifestPackageInfoEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxBundleManifestPackageInfoEnumerator.GetCurrent(out IAppxBundleManifestPackageInfo appxBundleManifestPackageInfo);

                    appxBundleManifestPackageInfo.GetFileName(out string fileName);
                    appxBundleManifestPackageInfo.GetPackageType(out APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE packageType);

                    // 获取应用捆绑包中的所有资源包（暂不支持解析分割 resources.pri 的解析）
                    if (packageType is APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE.APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE_APPLICATION)
                    {
                        appxBundleManifestPackageInfo.GetPackageId(out IAppxManifestPackageId appxManifestPackageId);
                        appxManifestPackageId.GetArchitecture(out ProcessorArchitecture architecture);
                        applicationDict.TryAdd(architecture, fileName);
                        scaleResourceList.Add(fileName);

                        List<string> languageBundleList = GetPackageBundleLanguage(appxBundleManifestPackageInfo);

                        foreach (string languageBundleItem in languageBundleList)
                        {
                            if (!languageList.Contains(languageBundleItem))
                            {
                                languageList.Add(languageBundleItem);
                            }
                        }
                    }
                    else
                    {
                        bool isPackageBundleScale = GetIsPackageBundleScale(appxBundleManifestPackageInfo);

                        if (isPackageBundleScale)
                        {
                            scaleResourceList.Add(fileName);
                        }
                    }

                    appxBundleManifestPackageInfoEnumerator.MoveNext(out _);
                }
            }

            packageManifestInformation.ApplicationDict = applicationDict;
            packageManifestInformation.LanguageList = languageList;
            packageManifestInformation.ScaleResourceList = scaleResourceList;
            return packageManifestInformation;
        }

        /// <summary>
        /// 解析应用捆绑包带缩放资源的文件
        /// </summary>
        private Dictionary<string, IAppxFile> ParsePackageBundleScaleFiles(IAppxBundleReader appxBundleReader, List<string> scaleResourceList)
        {
            Dictionary<string, IAppxFile> bundleFileDict = [];

            // 读取捆绑包的二进制文件
            if (appxBundleReader.GetPayloadPackages(out IAppxFilesEnumerator appxFilesEnumerator) is 0)
            {
                while (appxFilesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxFilesEnumerator.GetCurrent(out IAppxFile appxFile);
                    appxFile.GetName(out string packageFileName);

                    foreach (string scaleResourceItem in scaleResourceList)
                    {
                        if (string.Equals(packageFileName, scaleResourceItem, StringComparison.OrdinalIgnoreCase))
                        {
                            bundleFileDict.TryAdd(packageFileName, appxFile);
                            break;
                        }
                    }

                    appxFilesEnumerator.MoveNext(out _);
                }
            }

            return bundleFileDict;
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        private string GetLocalizedString(string resource, ResourceManagement resourceManagement)
        {
            if (!string.IsNullOrEmpty(resource) && resourceManagement is not null)
            {
                if (resource.StartsWith(msresource))
                {
                    string splitResource = resource[msresource.Length..];

                    string resourceKey;
                    if (splitResource.Contains('\\'))
                    {
                        resourceKey = splitResource;
                    }
                    else if (resource.Contains('/'))
                    {
                        resourceKey = splitResource.Replace('/', '\\');
                    }
                    else
                    {
                        resourceKey = string.Format(@"Resources\{0}", splitResource);
                    }

                    try
                    {
                        if (resourceManagement.ResourceMap.TryGetValue(resourceKey, resourceManagement.CurrentResourceContext) is ResourceCandidate currentCandidate && currentCandidate.Kind is ResourceCandidateKind.String)
                        {
                            return currentCandidate.ValueAsString;
                        }
                        else if (resourceManagement.ResourceMap.TryGetValue(resourceKey, resourceManagement.DefaultResourceContext) is ResourceCandidate defaultCandidate && defaultCandidate.Kind is ResourceCandidateKind.String)
                        {
                            return defaultCandidate.ValueAsString;
                        }
                        else
                        {
                            return resourceKey;
                        }
                    }
                    catch (Exception exception)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(GetLocalizedString), 1, exception);
                        return resourceKey;
                    }
                }
            }

            return string.IsNullOrEmpty(resource) ? string.Empty : resource;
        }

        /// <summary>
        /// 获取应用包图标
        /// </summary>
        private IStream GetPackageLogo(string logo, Dictionary<string, IAppxFile> appxFileDict)
        {
            List<KeyValuePair<string, IAppxFile>> logoList = [];
            string logoExtensionName = Path.GetExtension(logo);
            string logoFileName = logo[..^logoExtensionName.Length];

            Regex logoRegex = new(string.Format("""{0}(.scale-\d{{3}}){{0,1}}{1}""", logoFileName.Replace(@"\", @"\\"), logoExtensionName), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // 读取应用包图标
            foreach (KeyValuePair<string, IAppxFile> appxFileItem in appxFileDict)
            {
                if (appxFileItem.Value.GetName(out string packageFileName) is 0 && logoRegex.IsMatch(packageFileName))
                {
                    logoList.Add(KeyValuePair.Create(packageFileName, appxFileItem.Value));
                }
            }

            return GetSpecifiedLogoStream(logo, logoList);
        }

        /// <summary>
        /// 获取应用捆绑包图标
        /// </summary>
        private async Task<IStream> GetPackageBundleLogoAsync(string logo, Dictionary<string, IAppxFile> scaleBundleFileDict)
        {
            List<KeyValuePair<string, IAppxFile>> logoList = [];
            Dictionary<string, IAppxFile> logoDict = [];
            string logoExtensionName = Path.GetExtension(logo);
            string logoFileName = logo[..^logoExtensionName.Length];

            Regex logoRegex = new(string.Format("""{0}(.scale-\d{{3}}){{0,1}}{1}""", logoFileName.Replace(@"\", @"\\"), logoExtensionName), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Lock logoLock = new();
            List<Task> logoTaskList = [];

            // 读取应用包图标
            foreach (KeyValuePair<string, IAppxFile> scaleBundleItem in scaleBundleFileDict)
            {
                logoTaskList.Add(Task.Run(() =>
                {
                    scaleBundleItem.Value.GetStream(out IStream bundleFileStream);

                    if (appxFactory is not null && appxFactory.CreatePackageReader2(bundleFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                    {
                        // 解析安装包所有文件
                        Dictionary<string, IAppxFile> appxFileDict = ParsePackagePayloadFiles(appxPackageReader);

                        // 读取应用包图标
                        foreach (KeyValuePair<string, IAppxFile> appxFileItem in appxFileDict)
                        {
                            if (appxFileItem.Value.GetName(out string packageFileName) is 0 && logoRegex.IsMatch(packageFileName))
                            {
                                logoLock.Enter();
                                try
                                {
                                    logoDict.TryAdd(packageFileName, appxFileItem.Value);
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    logoLock.Exit();
                                }
                            }
                        }
                    }
                }));
            }

            await Task.WhenAll(logoTaskList);

            foreach (KeyValuePair<string, IAppxFile> logoItem in logoDict)
            {
                logoList.Add(KeyValuePair.Create(logoItem.Key, logoItem.Value));
            }

            return GetSpecifiedLogoStream(logo, logoList);
        }

        /// <summary>
        /// 获取应用包资源文件
        /// </summary>
        private IStream GetPackageResourceFileStream(Dictionary<string, IAppxFile> fileDict)
        {
            IStream resourceFileStream = null;

            foreach (KeyValuePair<string, IAppxFile> fileItem in fileDict)
            {
                if (string.Equals(fileItem.Key, "resources.pri", StringComparison.OrdinalIgnoreCase))
                {
                    fileItem.Value.GetStream(out resourceFileStream);
                    break;
                }
            }

            return resourceFileStream;
        }

        /// <summary>
        /// 获取特定语言的资源
        /// </summary>
        private string GetSpecifiedLanguage(List<string> languageList)
        {
            if (languageList is null || languageList.Count is 0)
            {
                return null;
            }

            CultureInfo appCultureInfo = CultureInfo.GetCultureInfo(LanguageService.AppLanguage);
            CultureInfo systemCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(LanguageService.DefaultAppLanguage);

            // 查找符合当前应用显示界面的语言资源信息
            while (!string.IsNullOrEmpty(appCultureInfo.Name))
            {
                foreach (string language in languageList)
                {
                    if (string.Equals(language, appCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return language;
                    }
                }

                appCultureInfo = appCultureInfo.Parent;
            }

            // 查找符合当前系统显示界面的语言资源信息
            while (!string.IsNullOrEmpty(systemCultureInfo.Name))
            {
                foreach (string language in languageList)
                {
                    if (string.Equals(language, systemCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return language;
                    }
                }

                systemCultureInfo = systemCultureInfo.Parent;
            }

            // 查找默认语言的资源信息
            while (!string.IsNullOrEmpty(defaultCultureInfo.Name))
            {
                foreach (string language in languageList)
                {
                    if (string.Equals(language, defaultCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return language;
                    }
                }

                defaultCultureInfo = systemCultureInfo.Parent;
            }

            // 不符合，如果存在资源信息，则返回第一个
            return languageList[0];
        }

        /// <summary>
        /// 获取符合的图标流
        /// </summary>
        private IStream GetSpecifiedLogoStream(string logo, List<KeyValuePair<string, IAppxFile>> logoList)
        {
            IStream imageFileStream = null;
            logoList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));

            if (logoList.Count > 0)
            {
                int deviceDpi = (int)(rasterizationScale * 100);

                for (int logoIndex = 0; logoIndex < logoList.Count; logoIndex++)
                {
                    KeyValuePair<string, IAppxFile> logoItem = logoList[logoIndex];
                    MatchCollection scaleMatchCollection = ScaleRegex.Matches(logoItem.Key);

                    for (int index = 0; index < scaleMatchCollection.Count; index++)
                    {
                        Match matchItem = scaleMatchCollection[index];
                        GroupCollection scaleDataListGroups = matchItem.Groups;

                        // 选择符合设备缩放大小的图标文件
                        if (scaleDataListGroups.Count is 2)
                        {
                            int scale = Convert.ToInt32(scaleDataListGroups[1].Value);

                            if (scale >= deviceDpi)
                            {
                                logoItem.Value.GetStream(out imageFileStream);
                                break;
                            }
                        }
                    }

                    if (imageFileStream is not null)
                    {
                        break;
                    }
                    else
                    {
                        // 若设备的缩放大小超过应用包提供的缩放大小，则使用最后一个
                        if (Equals(logoIndex, logoList.Count - 1))
                        {
                            logoItem.Value.GetStream(out imageFileStream);
                        }
                    }
                }

                // 没有合适的，则使用文件名对应的图标
                if (imageFileStream is null)
                {
                    foreach (KeyValuePair<string, IAppxFile> logoItem in logoList)
                    {
                        if (string.Equals(logoItem.Key, logo, StringComparison.OrdinalIgnoreCase))
                        {
                            logoItem.Value.GetStream(out imageFileStream);
                            break;
                        }
                    }
                }

                // 没有选中，则选择第一个
                if (imageFileStream is null)
                {
                    if (logoList.Count > 0)
                    {
                        logoList[0].Value.GetStream(out imageFileStream);
                    }
                }
            }

            return imageFileStream;
        }

        /// <summary>
        /// 获取应用捆绑包定义的语言
        /// </summary>
        private List<string> GetPackageBundleLanguage(IAppxBundleManifestPackageInfo appxBundleManifestPackageInfo)
        {
            List<string> languageResourceList = [];

            if (appxBundleManifestPackageInfo.GetResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
            {
                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                    {
                        languageResourceList.Add(language);
                    }

                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                }
            }

            languageResourceList.Sort();
            return languageResourceList;
        }

        /// <summary>
        /// 获取应用捆绑包中定义资源缩放
        /// </summary>
        private bool GetIsPackageBundleScale(IAppxBundleManifestPackageInfo appxBundleManifestPackageInfo)
        {
            bool isPackageBundleScale = false;

            if (appxBundleManifestPackageInfo.GetResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
            {
                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetScale(out uint scale) is 0 && scale is not 0)
                    {
                        isPackageBundleScale = true;
                        break;
                    }

                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                }
            }

            return isPackageBundleScale;
        }

        #endregion 第十部分：解析应用包信息

        #region 第十一部分：更新应用包信息

        /// <summary>
        /// 恢复默认内容
        /// </summary>
        private void ResetResult()
        {
            IsLoadCompleted = false;
            CanDragFile = false;
            IsParseSuccessfully = false;
            PackageFileType = PackageFileType.None;
            PackageIconImage = null;
            PackageName = string.Empty;
            PublisherDisplayName = string.Empty;
            Version = new Version();
            PackageDescription = string.Empty;
            PackageFamilyName = string.Empty;
            PackageFullName = string.Empty;
            SupportedArchitecture = string.Empty;
            IsFramework = string.Empty;
            AppInstalledState = string.Empty;
            AppInstallerSourceLink = string.Empty;
            IsAppInstallerSourceLinkExisted = false;
            PackageSourceLink = string.Empty;
            IsPackageSourceLinkExisted = false;
            PackageType = string.Empty;
            HoursBetweenUpdateChecks = string.Empty;
            UpdateBlocksActivation = string.Empty;
            ShowPrompt = string.Empty;
            ForceUpdateFromAnyVersion = string.Empty;
            AutomaticBackgroundTask = string.Empty;
            IsAppInstalled = false;
            IsUpdateSettingsExisted = false;
            IsInstalling = false;
            InstallProgressValue = 0;
            IsInstallWaiting = false;
            IsInstallFailed = false;
            IsCancelInstall = true;
            InstallStateString = string.Empty;
            InstallFailedInformation = string.Empty;
            TargetDeviceFamilyCollection.Clear();
            DependencyCollection.Clear();
            CapabilitiesCollection.Clear();
            ApplicationCollection.Clear();
            LanguageCollection.Clear();
            InstallDependencyCollection.Clear();
        }

        /// <summary>
        /// 更新结果
        /// </summary>
        private async Task UpdateResultAsync((bool parseResult, PackageInformation packageInformation) resultDict)
        {
            IsParseSuccessfully = resultDict.parseResult;

            if (IsParseSuccessfully)
            {
                PackageInformation packageInformation = resultDict.packageInformation;

                PackageFileType = packageInformation.PackageFileType;
                PackageName = string.IsNullOrEmpty(packageInformation.DisplayName) ? NotAvailableString : packageInformation.DisplayName;
                PublisherDisplayName = string.IsNullOrEmpty(packageInformation.PublisherDisplayName) ? NotAvailableString : packageInformation.PublisherDisplayName;
                Version = packageInformation.Version is not null ? packageInformation.Version : new Version();
                PackageDescription = string.IsNullOrEmpty(packageInformation.Description) ? NoneString : packageInformation.Description;
                PackageFamilyName = string.IsNullOrEmpty(packageInformation.PackageFamilyName) ? NotAvailableString : packageInformation.PackageFamilyName;
                PackageFullName = string.IsNullOrEmpty(packageInformation.PackageFullName) ? NotAvailableString : packageInformation.PackageFullName;
                SupportedArchitecture = string.IsNullOrEmpty(packageInformation.ProcessorArchitecture) ? NotAvailableString : packageInformation.ProcessorArchitecture;
                IsFramework = packageInformation.IsFramework.HasValue ? packageInformation.IsFramework.Value ? YesString : NoString : NotAvailableString;
                AppInstalledState = string.IsNullOrEmpty(packageInformation.AppInstalledState) ? NotAvailableString : packageInformation.AppInstalledState;
                AppInstallerSourceLink = string.IsNullOrEmpty(packageInformation.AppInstallerSourceLink) ? NotAvailableString : packageInformation.AppInstallerSourceLink;
                IsAppInstallerSourceLinkExisted = packageInformation.IsAppInstallerSourceLinkExisted;
                PackageSourceLink = string.IsNullOrEmpty(packageInformation.PackageSourceLink) ? NotAvailableString : packageInformation.PackageSourceLink;
                IsPackageSourceLinkExisted = packageInformation.IsPackageSourceLinkExisted;
                PackageType = packageInformation.PackageType;
                HoursBetweenUpdateChecks = string.Format(HoursString, packageInformation.HoursBetweenUpdateChecks);
                UpdateBlocksActivation = packageInformation.UpdateBlocksActivation ? YesString : NoString;
                ShowPrompt = packageInformation.ShowPrompt ? YesString : NoString;
                ForceUpdateFromAnyVersion = packageInformation.ForceUpdateFromAnyVersion ? YesString : NoString;
                AutomaticBackgroundTask = packageInformation.AutomaticBackgroundTask ? YesString : NoString;
                IsAppInstalled = packageInformation.IsAppInstalled;
                IsUpdateSettingsExisted = packageInformation.IsUpdateSettingsExisted;

                if (packageInformation.TargetDeviceFamilyList is not null)
                {
                    foreach (TargetDeviceFamilyModel targetDeviceFamilyItem in packageInformation.TargetDeviceFamilyList)
                    {
                        TargetDeviceFamilyCollection.Add(targetDeviceFamilyItem);
                    }
                }

                if (packageInformation.DependencyList is not null)
                {
                    foreach (DependencyInformation dependencyItem in packageInformation.DependencyList)
                    {
                        DependencyCollection.Add(new DependencyModel()
                        {
                            DependencyName = dependencyItem.DependencyName,
                            DependencyPublisher = dependencyItem.DependencyPublisher,
                            DependencyMinVersion = dependencyItem.DependencyMinVersion,
                            DependencyVersion = dependencyItem.DependencyVersion,
                            ProcessorArchitecture = dependencyItem.ProcessorArchitecture,
                            Uri = dependencyItem.Uri
                        });
                    }
                }

                if (packageInformation.CapabilitiesList is not null)
                {
                    foreach (string capability in packageInformation.CapabilitiesList)
                    {
                        if (CapabilityDict.TryGetValue(capability.ToLowerInvariant(), out string capabilityValue))
                        {
                            CapabilitiesCollection.Add(new CapabilityModel()
                            {
                                CapabilityLocalizedName = capabilityValue,
                                CapabilityName = capability
                            });
                        }
                        else
                        {
                            CapabilitiesCollection.Add(new CapabilityModel()
                            {
                                CapabilityLocalizedName = capability,
                                CapabilityName = capability
                            });
                        }
                    }
                }

                if (packageInformation.ApplicationList is not null)
                {
                    foreach (ApplicationModel applicationItem in packageInformation.ApplicationList)
                    {
                        ApplicationCollection.Add(applicationItem);
                    }
                }

                if (packageInformation.LanguageList is not null)
                {
                    foreach (string language in packageInformation.LanguageList)
                    {
                        LanguageCollection.Add(new CultureInfo(language));
                    }
                }

                try
                {
                    if (packageInformation.ImageLogo is not null)
                    {
                        ShCoreLibrary.CreateRandomAccessStreamOverStream(packageInformation.ImageLogo, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out nint ppv);
                        RandomAccessStreamOverStream randomAccessStreamOverStream = RandomAccessStreamOverStream.FromAbi(ppv);
                        BitmapImage bitmapImage = new();
                        await bitmapImage.SetSourceAsync(randomAccessStreamOverStream);
                        PackageIconImage = bitmapImage;
                        randomAccessStreamOverStream?.Dispose();
                        Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(packageInformation.ImageLogo, CreateComInterfaceFlags.None));
                        randomAccessStreamOverStream = null;
                    }
                    else
                    {
                        PackageIconImage = null;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerWindow), nameof(UpdateResultAsync), 1, e);
                }
            }

            CanDragFile = true;
            IsLoadCompleted = true;
        }

        #endregion 第十一部分：更新应用包信息

        /// <summary>
        /// 处理提权模式下的文件拖拽
        /// </summary>
        public async Task DealElevatedDragDropAsync(string filePath)
        {
            string extensionName = Path.GetExtension(filePath);

            if (CanDragFile && string.Equals(extensionName, ".appx", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msix", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".appxbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".msixbundle", StringComparison.OrdinalIgnoreCase) || string.Equals(extensionName, ".appinstaller", StringComparison.OrdinalIgnoreCase))
            {
                fileName = filePath;

                if (!string.IsNullOrEmpty(fileName))
                {
                    IsParseEmpty = false;
                    ResetResult();

                    ValueTuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
                    {
                        return await ParsePackagedAppAsync(fileName);
                    });

                    await UpdateResultAsync(parseResult);
                }
            }
        }
    }
}
