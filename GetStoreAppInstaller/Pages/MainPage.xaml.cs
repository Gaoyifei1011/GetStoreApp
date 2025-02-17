using GetStoreAppInstaller.Extensions.DataType.Classes;
using GetStoreAppInstaller.Extensions.DataType.Enums;
using GetStoreAppInstaller.Extensions.DataType.Methods;
using GetStoreAppInstaller.Extensions.PriExtract;
using GetStoreAppInstaller.Helpers.Controls.Extensions;
using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.Services.Controls.Settings;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.UI.TeachingTips;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.SHCore;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.Management.Core;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using WinRT;
using WinRT.Interop;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppInstaller.Pages
{
    /// <summary>
    /// 应用主页面
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private readonly string unknown = string.Format("[{0}]", ResourceService.GetLocalized("Installer/unknown"));
        private readonly string yes = ResourceService.GetLocalized("Installer/Yes");
        private readonly string no = ResourceService.GetLocalized("Installer/No");
        private readonly Guid CLSID_ApplicationActivationManager = new("45BA127D-10A8-46EA-8AB7-56EA9078943C");

        private readonly Guid CLSID_AppxFactory = new("5842A140-FF9F-4166-8F5C-62F5B7B0C781");
        private readonly Guid CLSID_AppxBundleFactory = new("378E0446-5384-43B7-8877-E7DBDD883446");
        private readonly PackageManager packageManager = new();
        private readonly AppActivationArguments appActivationArguments;

        private readonly IApplicationActivationManager applicationActivationManager;
        private readonly IAppxFactory3 appxFactory;
        private readonly IAppxBundleFactory2 appxBundleFactory;

        [GeneratedRegex("""scale-(\d{3})""", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex ScaleRegex { get; }

        private string fileName = string.Empty;
        private IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installPackageWithProgress;

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

        private BitmapImage _packageIconImage;

        public BitmapImage PackageIconImage
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
                if (!Equals(_packageName, value))
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
                if (!Equals(_publisherDisplayName, value))
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
                if (!Equals(_version, value))
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
                if (!Equals(_packageDescription, value))
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
                if (!Equals(_packageFamilyName, value))
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
                if (!Equals(_packageFullName, value))
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
                if (!Equals(_supportedArchitecture, value))
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
                if (!Equals(_isFramework, value))
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
                if (!Equals(_appInstalledState, value))
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
                if (!Equals(_appInstallerSourceLink, value))
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
                if (!Equals(_packageSourceLink, value))
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
                if (!Equals(_packageType, value))
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
                if (!Equals(_updateBlocksActivation, value))
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
                if (!Equals(_showPrompt, value))
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
                if (!Equals(_forceUpdateFromAnyVersion, value))
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
                if (!Equals(_automaticBackgroundTask, value))
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
                if (!Equals(_installStateString, value))
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
                if (!Equals(_installFailedInformation, value))
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

        private ObservableCollection<Language> LanguageCollection { get; } = [];

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

        public MainPage(AppActivationArguments activationArguments)
        {
            InitializeComponent();
            appActivationArguments = activationArguments;

            WindowTheme = ThemeService.AppTheme.Equals(ThemeService.ThemeList[0])
                ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark
                : Enum.TryParse(ThemeService.AppTheme.Key, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;

            Program.SetTitleBarTheme(ActualTheme);
            Program.SetClassicMenuTheme(ActualTheme);

            if (ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush)))
            {
                EnableBackdropMaterial = true;
                VisualStateManager.GoToState(MainPageRoot, "MicaBackdrop", false);
            }
            else
            {
                EnableBackdropMaterial = false;
                VisualStateManager.GoToState(MainPageRoot, "DesktopAcrylicBackdrop", false);
            }

            if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
            {
                appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);
            }

            if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out IntPtr appxBundleFactoryPtr) is 0)
            {
                appxBundleFactory = (IAppxBundleFactory2)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxBundleFactoryPtr, CreateObjectFlags.Unwrap);
            }

            if (Ole32Library.CoCreateInstance(CLSID_ApplicationActivationManager, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IApplicationActivationManager).GUID, out IntPtr applicationActivationManagerPtr) is 0)
            {
                applicationActivationManager = (IApplicationActivationManager)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(applicationActivationManagerPtr, CreateObjectFlags.Unwrap);
            }
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 拖拽安装包
        /// </summary>
        protected override async void OnDragEnter(DragEventArgs args)
        {
            base.OnDragEnter(args);
            DragOperationDeferral deferral = args.GetDeferral();

            try
            {
                IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                if (dragItemsList.Count is 1)
                {
                    string extensionName = Path.GetExtension(dragItemsList[0].Name);

                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        args.AcceptedOperation = DataPackageOperation.Copy;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/OpenPackage");
                    }
                    else if (extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        args.AcceptedOperation = DataPackageOperation.Copy;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/OpenInstallerFile");
                    }
                    else
                    {
                        args.AcceptedOperation = DataPackageOperation.None;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/UnsupportedFileType");
                    }
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/UnsupportedMultiFiles");
                }

                args.Handled = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Update drag enter information failed", e);
            }
            finally
            {
                deferral.Complete();
            }
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral deferral = args.GetDeferral();
            DataPackageView view = args.DataView;
            fileName = string.Empty;

            if (view.Contains(StandardDataFormats.StorageItems))
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
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Read file {0} information failed", dragItemsList[0].Path), e);
                    }
                }
            }

            deferral.Complete();

            if (!string.IsNullOrEmpty(fileName))
            {
                IsParseEmpty = false;
                ResetResult();

                Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第二部分：窗口右键菜单事件

        #region 第三部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除依赖包
        /// </summary>
        private void OnDeleteDependencyPackageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string dependencyFullName && !string.IsNullOrEmpty(dependencyFullName))
            {
                foreach (InstallDependencyModel installDependencyItem in InstallDependencyCollection)
                {
                    if (installDependencyItem.DependencyFullName.Equals(dependencyFullName))
                    {
                        InstallDependencyCollection.Remove(installDependencyItem);
                        break;
                    }
                }
            }
        }

        #endregion 第三部分：XamlUICommand 命令调用时挂载的事件

        #region 第四部分：应用安装器主页面——挂载的事件

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            Program.SetTitleBarTheme(sender.ActualTheme);
            Program.SetClassicMenuTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 主页面初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            IsParseEmpty = true;
            CanDragFile = true;
            fileName = string.Empty;

            // 正常启动
            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                if (appActivationArguments.Data is WinRT.IInspectable inspectable)
                {
                    string executableFileName = Path.GetFileName(Environment.ProcessPath);
                    string[] argumentsArray = Environment.GetCommandLineArgs();

                    if (argumentsArray.Length >= 2 && argumentsArray[0].Contains(executableFileName))
                    {
                        fileName = argumentsArray[1];
                    }
                    else if (argumentsArray.Length >= 1)
                    {
                        fileName = argumentsArray[0];
                    }
                }
                else if (appActivationArguments.Data is LaunchActivatedEventArgs launchActivatedEventArgs)
                {
                    if (!string.IsNullOrEmpty(launchActivatedEventArgs.Arguments))
                    {
                        string executableFileName = Path.GetFileName(Environment.ProcessPath);
                        string[] argumentsArray = launchActivatedEventArgs.Arguments.Split(' ');

                        if (launchActivatedEventArgs.Arguments.Contains(executableFileName))
                        {
                            if (argumentsArray.Length >= 2)
                            {
                                fileName = argumentsArray[1];
                            }
                        }
                        else
                        {
                            if (argumentsArray.Length >= 1)
                            {
                                fileName = argumentsArray[0];
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    IsParseEmpty = false;
                    ResetResult();

                    Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
                    {
                        return await ParsePackagedAppAsync(fileName);
                    });

                    await UpdateResultAsync(parseResult);
                }
            }
            // 从文件处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.File)
            {
                FileActivatedEventArgs fileActivatedEventArgs = appActivationArguments.Data as FileActivatedEventArgs;

                // 只解析读取到的第一个文件
                if (fileActivatedEventArgs.Files.Count > 0)
                {
                    fileName = fileActivatedEventArgs.Files[0].Path;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        IsParseEmpty = false;
                        ResetResult();

                        Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
                        {
                            return await ParsePackagedAppAsync(fileName);
                        });

                        await UpdateResultAsync(parseResult);
                    }
                }
            }
            // 从共享目标处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.ShareTarget)
            {
                ShareTargetActivatedEventArgs shareTargetActivatedEventArgs = appActivationArguments.Data as ShareTargetActivatedEventArgs;
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

                            Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
                            {
                                return await ParsePackagedAppAsync(fileName);
                            });

                            await UpdateResultAsync(parseResult);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            if (applicationActivationManager is not null)
            {
                Task.Run(() =>
                {
                    foreach (Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                    {
                        if (package.Id.FullName.Contains("Gaoyifei1011.GetStoreApp"))
                        {
                            IReadOnlyList<AppListEntry> appListEntryList = package.GetAppListEntries();
                            foreach (AppListEntry appListEntry in appListEntryList)
                            {
                                if (appListEntry.AppUserModelId.Equals("Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreApp"))
                                {
                                    applicationActivationManager.ActivateApplication(appListEntry.AppUserModelId, "Settings", ACTIVATEOPTIONS.AO_NONE, out _);
                                    break;
                                }
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 选择安装包
        /// </summary>
        private async void OnOpenPackageClicked(object sender, RoutedEventArgs args)
        {
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;
            bool hasSelectFile = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");
                fileOpenPicker.FileTypeFilter.Add(".appinstaller");

                if (await fileOpenPicker.PickSingleFileAsync() is StorageFile storageFile)
                {
                    fileName = storageFile.Path;
                    hasSelectFile = true;
                }
                result = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Open fileOpenPicker failed", e);
            }

            // 使用 IFileDialog
            if (!result)
            {
                try
                {
                    OpenFileDialog openFileDialog = new(Program.MainAppWindow.Id)
                    {
                        Description = ResourceService.GetLocalized("Installer/SelectPackage"),
                        UseCustomFilterTypes = true
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add("*.appx");
                    openFileDialog.FileTypeFilter.Add("*.msix");
                    openFileDialog.FileTypeFilter.Add("*.appxbundle");
                    openFileDialog.FileTypeFilter.Add("*.msixbundle");
                    openFileDialog.FileTypeFilter.Add("*.appinstaller");

                    if (openFileDialog.ShowDialog())
                    {
                        fileName = openFileDialog.SelectedFile;
                        hasSelectFile = true;
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
            }

            if (result && hasSelectFile)
            {
                IsParseEmpty = false;
                ResetResult();

                Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
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
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;
            bool hasSelectFile = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");
                fileOpenPicker.FileTypeFilter.Add(".appinstaller");

                if (await fileOpenPicker.PickSingleFileAsync() is StorageFile storageFile)
                {
                    fileName = storageFile.Path;
                    hasSelectFile = true;
                }
                result = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Open fileOpenPicker failed", e);
            }

            // 使用 IFileDialog
            if (!result)
            {
                try
                {
                    OpenFileDialog openFileDialog = new(Program.MainAppWindow.Id)
                    {
                        Description = ResourceService.GetLocalized("Installer/SelectPackage"),
                        UseCustomFilterTypes = true
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add("*.appx");
                    openFileDialog.FileTypeFilter.Add("*.msix");
                    openFileDialog.FileTypeFilter.Add("*.appxbundle");
                    openFileDialog.FileTypeFilter.Add("*.msixbundle");
                    openFileDialog.FileTypeFilter.Add("*.appinstaller");

                    if (openFileDialog.ShowDialog())
                    {
                        fileName = openFileDialog.SelectedFile;
                        hasSelectFile = true;
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
            }

            if (result && hasSelectFile)
            {
                ResetResult();

                Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
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
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");

                IReadOnlyList<StorageFile> filesList = await fileOpenPicker.PickMultipleFilesAsync();

                if (filesList is not null)
                {
                    foreach (StorageFile file in filesList)
                    {
                        Tuple<bool, DependencyAppInformation> parseResult = await Task.Run(async () =>
                        {
                            return await ParseDependencyAppAsync(file.Path);
                        });

                        if (parseResult.Item1)
                        {
                            DependencyAppInformation dependencyAppInformation = parseResult.Item2;

                            InstallDependencyCollection.Add(new InstallDependencyModel()
                            {
                                DependencyName = file.Name,
                                DependencyVersion = dependencyAppInformation.Version is not null ? dependencyAppInformation.Version : new Version(),
                                DependencyPublisher = string.IsNullOrEmpty(dependencyAppInformation.PublisherDisplayName) ? unknown : dependencyAppInformation.PublisherDisplayName,
                                DependencyFullName = string.IsNullOrEmpty(dependencyAppInformation.PackageFullName) ? Guid.NewGuid().ToString() : dependencyAppInformation.PackageFullName,
                                DependencyPath = file.Path
                            });
                        }
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Open fileOpenPicker failed", e);
            }

            // 使用 IFileDialog
            if (!result)
            {
                try
                {
                    OpenFileDialog openFileDialog = new(Program.MainAppWindow.Id)
                    {
                        Description = ResourceService.GetLocalized("Installer/SelectDependencyPackage"),
                        AllowMultiSelect = true,
                        UseCustomFilterTypes = true
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add("*.appx");
                    openFileDialog.FileTypeFilter.Add("*.msix");
                    openFileDialog.FileTypeFilter.Add("*.appxbundle");
                    openFileDialog.FileTypeFilter.Add("*.msixbundle");

                    if (openFileDialog.ShowDialog())
                    {
                        foreach (string fileItem in openFileDialog.SelectedFileList)
                        {
                            InstallDependencyCollection.Add(new InstallDependencyModel()
                            {
                                DependencyName = Path.GetFileName(fileItem),
                                DependencyVersion = new Version(),
                                DependencyPublisher = string.Empty,
                                DependencyFullName = Guid.NewGuid().ToString(),
                                DependencyPath = fileItem
                            });
                        }
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
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

                await TeachingTipHelper.ShowAsync(new InstallerDataCopyTip(copyResult));
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if ((sender as Button).Tag is string tag)
            {
                if (tag.Equals("AddDependencyFlyout", StringComparison.OrdinalIgnoreCase) && AddDependencyFlyout.IsOpen)
                {
                    AddDependencyFlyout.Hide();
                }
                else if (tag.Equals("ViewErrorInformationFlyout", StringComparison.OrdinalIgnoreCase) && ViewErrorInformationFlyout.IsOpen)
                {
                    ViewErrorInformationFlyout.Hide();
                }
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
                InstallStateString = ResourceService.GetLocalized("Installer/PrepareInstall");

                await Task.Run(() =>
                {
                    string extensionName = Path.GetExtension(fileName);

                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            // todo : 添加更新自身提示
                            if (PackageFamilyName.Equals(Windows.ApplicationModel.Package.Current.Id.FamilyName, StringComparison.OrdinalIgnoreCase))
                            {
                                AppNotificationBuilder appNotificationBuilder = new();
                                appNotificationBuilder.AddArgument("action", "OpenApp");
                                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallSelf"), PackageName));
                                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            }

                            AddPackageOptions addPackageOptions = new()
                            {
                                AllowUnsigned = AppInstallService.AllowUnsignedPackageValue,
                                ForceAppShutdown = AppInstallService.ForceAppShutdownValue,
                                ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdownValue
                            };

                            foreach (InstallDependencyModel installDependencyItem in InstallDependencyCollection)
                            {
                                addPackageOptions.DependencyPackageUris.Add(new Uri(installDependencyItem.DependencyPath));
                            }

                            // 安装目标应用，并获取安装进度
                            installPackageWithProgress = packageManager.AddPackageByUriAsync(new Uri(fileName), addPackageOptions);

                            // 更新安装进度
                            installPackageWithProgress.Progress = (result, progress) => OnInstallPackageProgressing(result, progress);

                            // 应用安装过程已结束
                            installPackageWithProgress.Completed = (result, status) => OnInstallPackageCompleted(result, status);
                        }
                        // 安装失败显示失败信息
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                        }
                    }
                    else if (extensionName.Equals(".appinstaller"))
                    {
                        try
                        {
                            // 安装目标应用，并获取安装进度
                            IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installPackageWithProgress = packageManager.AddPackageByAppInstallerFileAsync(new Uri(fileName), AppInstallService.ForceTargetAppShutdownValue ? AddPackageByAppInstallerOptions.ForceTargetAppShutdown : AddPackageByAppInstallerOptions.None, null);

                            // 更新安装进度
                            installPackageWithProgress.Progress = (result, progress) => OnInstallPackageProgressing(result, progress);

                            // 应用安装过程已结束
                            installPackageWithProgress.Completed = (result, status) => OnInstallPackageCompleted(result, status);
                        }
                        // 安装失败显示失败信息
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                        }
                    }
                });
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
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        private void OnStartAppClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                foreach (Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (package.Id.FamilyName.Equals(PackageFamilyName))
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
                foreach (Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (package.Id.FamilyName.Equals(PackageFamilyName))
                    {
                        try
                        {
                            await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, string.Format("{0} app installed folder open failed", package.DisplayName), e);
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
                        if (ApplicationDataManager.CreateForPackageFamily(PackageFamilyName) is ApplicationData applicationData)
                        {
                            await Launcher.LaunchFolderAsync(applicationData.LocalFolder);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "Open app cache folder failed.", e);
                    }
                }
            });
        }

        /// <summary>
        /// 打开应用安装程序文件链接
        /// </summary>
        private void OnOpenAppInstallerSourceLinkClicked(object sender, RoutedEventArgs args)
        {
            string appInstallerSourceLink = (sender as HyperlinkButton).Tag as string;

            Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(appInstallerSourceLink))
                {
                    await Launcher.LaunchUriAsync(new Uri(appInstallerSourceLink));
                }
            });
        }

        /// <summary>
        /// 打开应用包链接
        /// </summary>
        private void OnOpenPackageSourceLinkClicked(object sender, RoutedEventArgs args)
        {
            string packageSourceLink = (sender as HyperlinkButton).Tag as string;

            Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(packageSourceLink))
                {
                    await Launcher.LaunchUriAsync(new Uri(packageSourceLink));
                }
            });
        }

        /// <summary>
        /// 了解目标设备系列
        /// </summary>
        private async void OnLearnTargetDeviceFamilyClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://learn.microsoft.com/uwp/extension-sdks/device-families-overview"));
        }

        /// <summary>
        /// 了解更新设置
        /// </summary>
        private async void OnLearnUpdateSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://learn.microsoft.com/windows/msix/app-installer/how-to-create-appinstaller-file"));
        }

        /// <summary>
        /// 了解应用包依赖
        /// </summary>
        private async void OnLearnPackageDependencyClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://learn.microsoft.com/uwp/schemas/appxpackage/uapmanifestschema/element-packagedependency"));
        }

        /// <summary>
        /// 了解应用包功能
        /// </summary>
        private async void OnLearnPackageCapabilityClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://learn.microsoft.com/windows/uwp/packaging/app-capability-declarations"));
        }

        #endregion 第四部分：应用安装器主页面——挂载的事件

        #region 第五部分：自定义事件

        /// <summary>
        /// 应用安装状态发生改变时触发的事件
        /// </summary>
        private async void OnInstallPackageProgressing(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress progress)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (progress.state is DeploymentProgressState.Queued)
                {
                    IsInstalling = true;
                    IsInstallWaiting = true;
                    InstallStateString = ResourceService.GetLocalized("Installer/WaitInstall");
                }
                else if (progress.state is DeploymentProgressState.Processing)
                {
                    IsInstalling = true;
                    IsInstallWaiting = false;
                    InstallProgressValue = progress.percentage;
                    InstallStateString = string.Format(ResourceService.GetLocalized("Installer/InstallProgress"), progress.percentage);
                }
            });
        }

        /// <summary>
        /// 应用安装完成时触发的事件
        /// </summary>
        private async void OnInstallPackageCompleted(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, AsyncStatus status)
        {
            // 安装完成
            if (status is AsyncStatus.Completed)
            {
                // 显示安装成功通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallSuccessfully"), PackageName));
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanDragFile = true;
                    IsInstalling = false;
                    InstallProgressValue = 0;
                    IsInstallWaiting = false;
                    IsInstallFailed = false;
                    InstallFailedInformation = string.Empty;
                    IsAppInstalled = true;
                    AppInstalledState = ResourceService.GetLocalized("Installer/AppInstalledNotNewVersion");
                });
            }
            // 安装错误
            else if (status is AsyncStatus.Error)
            {
                string errorMessage = result.ErrorCode.Message;

                // 显示安装失败通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallFailed"), PackageName));
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallFailedReason"), errorMessage));
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanDragFile = true;
                    IsInstalling = false;
                    InstallProgressValue = 0;
                    IsInstallWaiting = false;
                    IsInstallFailed = true;
                    InstallFailedInformation = errorMessage;
                });
            }
            else if (status is AsyncStatus.Canceled)
            {
                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanDragFile = true;
                    IsInstalling = false;
                    InstallProgressValue = 0;
                    IsInstallWaiting = false;
                    IsInstallFailed = false;
                    InstallFailedInformation = string.Empty;
                });
            }

            result.Close();
        }

        #endregion 第五部分：自定义事件

        #region 第六部分：解析应用包信息

        /// <summary>
        /// 解析应用包
        /// </summary>
        private async Task<Tuple<bool, PackageInformation>> ParsePackagedAppAsync(string filePath)
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
                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (appxFactory is not null && appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                            {
                                parseResult = true;

                                // 解析安装包所有文件
                                Dictionary<string, IAppxFile> appxFileDict = ParsePackagePayloadFiles(appxPackageReader);

                                // 获取并解析本地资源文件
                                IStream resourceFileStream = GetPackageResourceFileStream(appxFileDict);
                                Dictionary<string, Dictionary<string, string>> resourceDict = null;

                                if (resourceFileStream is not null)
                                {
                                    resourceDict = ParsePackageResources(resourceFileStream);
                                    Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));
                                    resourceFileStream = null;
                                }

                                // 从资源文件中查找符合的语言
                                Dictionary<string, string> specifiedLanguageResourceDict = GetSpecifiedLanguageResource(resourceDict);

                                // 解析资源包清单文件
                                ManifestInformation manifestInformation = ParsePackageManifest(appxPackageReader, false, specifiedLanguageResourceDict);

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
                                if (!string.IsNullOrEmpty(packageInformation.Logo))
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
                            foreach (Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                            {
                                if (package.Id.FullName.Contains(packageInformation.PackageFamilyName))
                                {
                                    Version installedVersion = new(package.Id.Version.Major, package.Id.Version.Major, package.Id.Version.Build, package.Id.Version.Revision);
                                    packageInformation.AppInstalledState = packageInformation.Version > installedVersion ? ResourceService.GetLocalized("Installer/AppInstalledNotNewVersion") : ResourceService.GetLocalized("Installer/AppInstalledNewVersion");
                                    packageInformation.IsAppInstalled = true;
                                    break;
                                }
                            }

                            if (string.IsNullOrEmpty(packageInformation.AppInstalledState))
                            {
                                packageInformation.AppInstalledState = ResourceService.GetLocalized("Installer/AppNotInstall");
                            }
                        }
                        else
                        {
                            packageInformation.AppInstalledState = unknown;
                        }
                    }

                    // 解析以 appxbundle 或 msixbundle 格式结尾的应用包
                    else if (extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (appxBundleFactory is not null && appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0 && appxBundleReader.GetManifest(out IAppxBundleManifestReader appxBundleManifestReader) is 0)
                            {
                                PackageManifestInformation packageManifestInformation = ParsePackageBundleManifestInfo(appxBundleManifestReader);
                                Dictionary<string, Dictionary<string, string>> resourceDict = [];

                                if (appxFactory is not null)
                                {
                                    parseResult = true;

                                    // 从资源文件中查找符合的语言
                                    if (packageManifestInformation.LanguageResourceDict is not null)
                                    {
                                        List<string> languageList = [];

                                        foreach (KeyValuePair<string, string> languageResourceItem in packageManifestInformation.LanguageResourceDict)
                                        {
                                            // 获取安装包支持的语言
                                            languageList.Add(languageResourceItem.Key);
                                        }

                                        // 获取特定语言的资源文件
                                        List<KeyValuePair<string, string>> specifiedLanguageResourceList = GetPackageBundleSpecifiedLanguageResource(packageManifestInformation.LanguageResourceDict);

                                        foreach (KeyValuePair<string, string> specifiedLanguageResourceItem in specifiedLanguageResourceList)
                                        {
                                            // 解析应用捆绑包对应符合的资源包
                                            appxBundleReader.GetPayloadPackage(specifiedLanguageResourceItem.Value, out IAppxFile specifiedLanguageResourceFile);

                                            if (specifiedLanguageResourceFile.GetStream(out IStream specifiedLanguageResourceFileStream) is 0 && appxFactory.CreatePackageReader2(specifiedLanguageResourceFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                            {
                                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(specifiedLanguageResourceFileStream, CreateComInterfaceFlags.None));

                                                // 解析资源包所有文件
                                                Dictionary<string, IAppxFile> fileDict = ParsePackagePayloadFiles(appxPackageReader);

                                                // 获取并解析本地资源文件
                                                IStream resourceFileStream = GetPackageResourceFileStream(fileDict);

                                                if (resourceFileStream is not null)
                                                {
                                                    Dictionary<string, Dictionary<string, string>> parseResourceDict = ParsePackageResources(resourceFileStream);
                                                    Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));

                                                    foreach (KeyValuePair<string, Dictionary<string, string>> parseResourceItem in parseResourceDict)
                                                    {
                                                        if (parseResourceItem.Key.Equals(specifiedLanguageResourceItem.Key, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            resourceDict.TryAdd(specifiedLanguageResourceItem.Key, parseResourceItem.Value);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        languageList.Sort();
                                        packageInformation.LanguageList = languageList;
                                    }

                                    // 从资源文件中查找符合的语言
                                    Dictionary<string, string> specifiedLanguageResourceDict = GetSpecifiedLanguageResource(resourceDict);

                                    // 解析应用
                                    if (packageManifestInformation.ApplicationDict is not null)
                                    {
                                        string applicationFileName = ParsePacakgeBundleCompatibleFile(packageManifestInformation.ApplicationDict);
                                        string architecture = ParsePackageBundleArchitecture(packageManifestInformation.ApplicationDict);
                                        packageInformation.ProcessorArchitecture = architecture;

                                        appxBundleReader.GetPayloadPackage(applicationFileName, out IAppxFile applicationFile);

                                        if (applicationFile.GetStream(out IStream applicationFileStream) is 0 && appxFactory.CreatePackageReader2(applicationFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                        {
                                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(applicationFileStream, CreateComInterfaceFlags.None));

                                            Dictionary<string, IAppxFile> appxFileDict = ParsePackagePayloadFiles(appxPackageReader);
                                            ManifestInformation manifestInformation = ParsePackageManifest(appxPackageReader, true, specifiedLanguageResourceDict);

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
                                            packageInformation.LanguageList = manifestInformation.LanguageList;
                                        }
                                    }

                                    // 从资源文件中查找符合的图标
                                    if (packageManifestInformation.ScaleResourceList is not null)
                                    {
                                        // 获取应用包图标
                                        if (!string.IsNullOrEmpty(packageInformation.Logo))
                                        {
                                            Dictionary<string, IAppxFile> scaleBundleFileDict = ParsePacakgeBundleScaleFiles(appxBundleReader, packageManifestInformation.ScaleResourceList);
                                            IStream imageFileStream = GetPackageBundleLogo(packageInformation.Logo, scaleBundleFileDict);
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
                            foreach (Windows.ApplicationModel.Package package in packageManager.FindPackagesForUser(string.Empty))
                            {
                                if (package.Id.FamilyName.Equals(packageInformation.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    Version installedVersion = new(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision);
                                    packageInformation.AppInstalledState = packageInformation.Version > installedVersion ? ResourceService.GetLocalized("Installer/AppInstalledNotNewVersion") : ResourceService.GetLocalized("Installer/AppInstalledNewVersion"); packageInformation.IsAppInstalled = true;
                                    packageInformation.IsAppInstalled = true;
                                    break;
                                }
                            }

                            if (string.IsNullOrEmpty(packageInformation.AppInstalledState))
                            {
                                packageInformation.AppInstalledState = ResourceService.GetLocalized("Installer/AppNotInstall");
                            }
                        }
                        else
                        {
                            packageInformation.AppInstalledState = unknown;
                        }
                    }

                    // 解析以 appinstaller 格式结尾的应用安装文件
                    else if (extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
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
                                packageInformation.IsAppInstallerSourceLinkExisted = true;
                            }

                            XmlNodeList mainPackageNodeList = xmlDocument.GetElementsByTagName("MainPackage");
                            XmlNodeList mainBundleNodeList = xmlDocument.GetElementsByTagName("MainBundle");

                            // 应用安装包
                            if (mainPackageNodeList.Count > 0)
                            {
                                packageInformation.PackageType = ResourceService.GetLocalized("Installer/Package");

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
                                packageInformation.PackageType = ResourceService.GetLocalized("Installer/PackageBundle");

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
                                    packageInformation.IsPackageSourceLinkExisted = true;
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
                                    dependencyInformation.ProcessorArchitecture = unknown;
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
                LogService.WriteLog(LoggingLevel.Error, string.Format("Parse package {0} failed", filePath), e);
            }

            return Tuple.Create(parseResult, packageInformation);
        }

        /// <summary>
        /// 解析依赖应用包
        /// </summary>
        private async Task<Tuple<bool, DependencyAppInformation>> ParseDependencyAppAsync(string filePath)
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
                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase))
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
                    else if (extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out IntPtr appxBundleFactoryPtr) is 0)
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
                LogService.WriteLog(LoggingLevel.Error, string.Format("Parse package {0} failed", filePath), e);
            }

            return Tuple.Create(parseResult, dependencyAppInformation);
        }

        /// <summary>
        /// 获取应用程序入口信息
        /// </summary>
        private List<ApplicationModel> ParsePackageApplication(IAppxManifestReader3 appxManifestReader, Dictionary<string, string> specifiedLanguageResourceDict)
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

                    description = GetLocalizedString(description, specifiedLanguageResourceDict);

                    ApplicationModel applicationItem = new()
                    {
                        AppDescription = string.IsNullOrEmpty(description) ? unknown : description,
                        EntryPoint = string.IsNullOrEmpty(entryPoint) ? unknown : entryPoint,
                        Executable = string.IsNullOrEmpty(executable) ? unknown : executable,
                        AppID = string.IsNullOrEmpty(id) ? unknown : id,
                        AppUserModelId = string.IsNullOrEmpty(appUserModelId) ? unknown : appUserModelId
                    };

                    applicationList.Add(applicationItem);
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
        private ManifestInformation ParsePackageManifest(IAppxPackageReader appxPackageReader, bool isBundle, Dictionary<string, string> specifiedLanguageResourceDict)
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
                            manifestInformation.ProcessorArchitecture = architecture.ToString();
                        }

                        manifestInformation.PackageFamilyName = packageFamilyName;
                        manifestInformation.PackageFullName = packageFullName;

                        PackageVersion packageVersion = new(version);
                        manifestInformation.Version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
                    }

                    // 获取包的目标设备系列
                    manifestInformation.TargetDeviceFamilyList = ParsePackageTargetDeviceFamily(appxManifestReader);

                    // 获取应用入口信息
                    manifestInformation.ApplicationList = ParsePackageApplication(appxManifestReader, specifiedLanguageResourceDict);

                    // 获取应用包的属性
                    if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                    {
                        packageProperties.GetBoolValue("Framework", out bool isFramework);

                        packageProperties.GetStringValue("Description", out string description);
                        packageProperties.GetStringValue("DisplayName", out string displayName);
                        packageProperties.GetStringValue("Logo", out string logo);
                        packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);

                        description = GetLocalizedString(description, specifiedLanguageResourceDict);
                        displayName = GetLocalizedString(displayName, specifiedLanguageResourceDict);
                        publisherDisplayName = GetLocalizedString(publisherDisplayName, specifiedLanguageResourceDict);

                        manifestInformation.IsFramework = isFramework;
                        manifestInformation.Description = description;
                        manifestInformation.DisplayName = displayName;
                        manifestInformation.Logo = logo;
                        manifestInformation.PublisherDisplayName = publisherDisplayName;
                    }

                    if (!isBundle)
                    {
                        // 获取应用包定义的限定资源
                        List<string> languageList = ParsePackageLanguage(appxManifestReader);
                        manifestInformation.LanguageList = languageList;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Parse package manifest failed", e);
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
                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
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
        /// 解析应用包的所有资源
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> ParsePackageResources(IStream resourceFileStream)
        {
            Dictionary<string, Dictionary<string, string>> resourceDict = [];

            try
            {
                // 分段 3 ：解析资源文件
                if (resourceFileStream is not null)
                {
                    ShCoreLibrary.CreateRandomAccessStreamOverStream(resourceFileStream, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out IntPtr ppv);
                    RandomAccessStreamOverStream randomAccessStreamOverStream = RandomAccessStreamOverStream.FromAbi(ppv);
                    Stream resourceStream = randomAccessStreamOverStream.AsStream();

                    if (resourceStream is not null)
                    {
                        // 读取并检查文件类型
                        BinaryReader priBinaryReader = new(resourceStream, Encoding.ASCII, true);
                        long fileStartOffset = priBinaryReader.BaseStream.Position;
                        string priType = new(priBinaryReader.ReadChars(8));

                        if (priType is "mrm_pri0" || priType is "mrm_pri1" || priType is "mrm_pri2" || priType is "mrm_prif")
                        {
                            priBinaryReader.ExpectUInt16(0);
                            priBinaryReader.ExpectUInt16(1);
                            uint totalFileSize = priBinaryReader.ReadUInt32();
                            uint tocOffset = priBinaryReader.ReadUInt32();
                            uint sectionStartOffset = priBinaryReader.ReadUInt32();
                            uint numSections = priBinaryReader.ReadUInt16();
                            priBinaryReader.ExpectUInt16(0xFFFF);
                            priBinaryReader.ExpectUInt32(0);
                            priBinaryReader.BaseStream.Seek(fileStartOffset + totalFileSize - 16, SeekOrigin.Begin);
                            priBinaryReader.ExpectUInt32(0xDEFFFADE);
                            priBinaryReader.ExpectUInt32(totalFileSize);
                            priBinaryReader.ExpectString(priType);
                            priBinaryReader.BaseStream.Seek(tocOffset, SeekOrigin.Begin);

                            // 读取内容列表
                            List<TocEntry> tocList = new((int)numSections);

                            for (int index = 0; index < numSections; index++)
                            {
                                tocList.Add(new TocEntry()
                                {
                                    SectionIdentifier = new(priBinaryReader.ReadChars(16)),
                                    Flags = priBinaryReader.ReadUInt16(),
                                    SectionFlags = priBinaryReader.ReadUInt16(),
                                    SectionQualifier = priBinaryReader.ReadUInt32(),
                                    SectionOffset = priBinaryReader.ReadUInt32(),
                                    SectionLength = priBinaryReader.ReadUInt32(),
                                });
                            }

                            // 读取分段列表
                            object[] sectionArray = new object[numSections];

                            for (int index = 0; index < sectionArray.Length; index++)
                            {
                                if (sectionArray[index] is null)
                                {
                                    priBinaryReader.BaseStream.Seek(sectionStartOffset + tocList[index].SectionOffset, SeekOrigin.Begin);

                                    switch (tocList[index].SectionIdentifier)
                                    {
                                        case "[mrm_pridescex]\0":
                                            {
                                                PriDescriptorSection section = new("[mrm_pridescex]\0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_hschema]  \0":
                                            {
                                                HierarchicalSchemaSection section = new("[mrm_hschema]  \0", priBinaryReader, false);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_hschemaex] ":
                                            {
                                                HierarchicalSchemaSection section = new("[mrm_hschemaex] ", priBinaryReader, true);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_decn_info]\0":
                                            {
                                                DecisionInfoSection section = new("[mrm_decn_info]\0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_res_map__]\0":
                                            {
                                                ResourceMapSection section = new("[mrm_res_map__]\0", priBinaryReader, false, ref sectionArray);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_res_map2_]\0":
                                            {
                                                ResourceMapSection section = new("[mrm_res_map2_]\0", priBinaryReader, true, ref sectionArray);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_dataitem] \0":
                                            {
                                                DataItemSection section = new("[mrm_dataitem] \0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_rev_map]  \0":
                                            {
                                                ReverseMapSection section = new("[mrm_rev_map]  \0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[def_file_list]\0":
                                            {
                                                ReferencedFileSection section = new("[def_file_list]\0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        default:
                                            {
                                                UnknownSection section = new(null, priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                    }
                                }
                            }

                            // 根据分段列表获取相应的内容
                            List<PriDescriptorSection> priDescriptorSectionList = [];

                            foreach (object section in sectionArray)
                            {
                                if (section is PriDescriptorSection priDescriptorSection)
                                {
                                    priDescriptorSectionList.Add(priDescriptorSection);
                                }
                            }

                            foreach (PriDescriptorSection priDescriptorSection in priDescriptorSectionList)
                            {
                                foreach (int resourceMapIndex in priDescriptorSection.ResourceMapSectionsList)
                                {
                                    if (sectionArray[resourceMapIndex] is ResourceMapSection resourceMapSection)
                                    {
                                        if (resourceMapSection.HierarchicalSchemaReference is not null)
                                        {
                                            continue;
                                        }

                                        DecisionInfoSection decisionInfoSection = sectionArray[resourceMapSection.DecisionInfoSectionIndex] as DecisionInfoSection;

                                        foreach (CandidateSet candidateSet in resourceMapSection.CandidateSetsDict.Values)
                                        {
                                            if (sectionArray[candidateSet.ResourceMapSectionAndIndex.Item1] is HierarchicalSchemaSection hierarchicalSchemaSection)
                                            {
                                                ResourceMapScopeAndItem resourceMapScopeAndItem = hierarchicalSchemaSection.ItemsList[candidateSet.ResourceMapSectionAndIndex.Item2];

                                                string key = string.Empty;

                                                if (resourceMapScopeAndItem.Name is not null && resourceMapScopeAndItem.Parent is not null)
                                                {
                                                    key = Path.Combine(resourceMapScopeAndItem.Parent.Name, resourceMapScopeAndItem.Name);
                                                }
                                                else if (resourceMapScopeAndItem.Name is not null)
                                                {
                                                    key = resourceMapScopeAndItem.Name;
                                                }

                                                if (string.IsNullOrEmpty(key))
                                                {
                                                    continue;
                                                }

                                                foreach (Candidate candidate in candidateSet.CandidatesList)
                                                {
                                                    string value = string.Empty;

                                                    if (candidate.SourceFileIndex is null)
                                                    {
                                                        ByteSpan byteSpan = null;

                                                        if (candidate.DataItemSectionAndIndex is not null)
                                                        {
                                                            DataItemSection dataItemSection = sectionArray[candidate.DataItemSectionAndIndex.Item1] as DataItemSection;
                                                            byteSpan = dataItemSection is not null ? dataItemSection.DataItemsList[candidate.DataItemSectionAndIndex.Item2] : candidate.Data;
                                                        }

                                                        if (byteSpan is not null)
                                                        {
                                                            resourceStream.Seek(byteSpan.Offset, SeekOrigin.Begin);
                                                            using BinaryReader binaryReader = new(resourceStream, Encoding.Default, true);
                                                            byte[] data = binaryReader.ReadBytes((int)byteSpan.Length);
                                                            binaryReader.Dispose();

                                                            switch (candidate.Type)
                                                            {
                                                                // ASCII 格式字符串内容
                                                                case ResourceValueType.AsciiString:
                                                                    {
                                                                        string content = Encoding.ASCII.GetString(data).TrimEnd('\0');
                                                                        string language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : "Neutral";

                                                                        if (resourceDict.TryGetValue(language, out Dictionary<string, string> stringContentDict))
                                                                        {
                                                                            stringContentDict.TryAdd(key, content);
                                                                        }
                                                                        else
                                                                        {
                                                                            Dictionary<string, string> stringDict = [];
                                                                            stringDict.TryAdd(key, content);
                                                                            resourceDict.TryAdd(language, stringDict);
                                                                        }

                                                                        break;
                                                                    }
                                                                // UTF8 格式字符串内容
                                                                case ResourceValueType.Utf8String:
                                                                    {
                                                                        string content = Encoding.UTF8.GetString(data).TrimEnd('\0');
                                                                        string language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : "Neutral";

                                                                        if (resourceDict.TryGetValue(language, out Dictionary<string, string> stringContentDict))
                                                                        {
                                                                            stringContentDict.TryAdd(key, content);
                                                                        }
                                                                        else
                                                                        {
                                                                            Dictionary<string, string> stringDict = [];
                                                                            stringDict.TryAdd(key, content);
                                                                            resourceDict.TryAdd(language, stringDict);
                                                                        }
                                                                        break;
                                                                    }
                                                                // Unicode 格式字符串内容
                                                                case ResourceValueType.UnicodeString:
                                                                    {
                                                                        string content = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                                                        string language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : "Neutral";

                                                                        if (resourceDict.TryGetValue(language, out Dictionary<string, string> stringContentDict))
                                                                        {
                                                                            stringContentDict.TryAdd(key, content);
                                                                        }
                                                                        else
                                                                        {
                                                                            Dictionary<string, string> stringDict = [];
                                                                            stringDict.TryAdd(key, content);
                                                                            resourceDict.TryAdd(language, stringDict);
                                                                        }
                                                                        break;
                                                                    }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        priBinaryReader.Dispose();
                    }

                    randomAccessStreamOverStream?.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Parse package resources failed", e);
            }

            return resourceDict;
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
                    TargetDeviceFamilyModel targetDeviceFamilyItem = new();
                    appxManifestTargetDeviceFamily.GetName(out string targetDeviceName);
                    appxManifestTargetDeviceFamily.GetMinVersion(out ulong minVersion);
                    appxManifestTargetDeviceFamily.GetMaxVersionTested(out ulong maxVersionTested);

                    targetDeviceFamilyItem.TargetDeviceName = targetDeviceName;
                    PackageVersion minPackageVersion = new(minVersion);
                    targetDeviceFamilyItem.MinVersion = new Version(minPackageVersion.Major, minPackageVersion.Minor, minPackageVersion.Build, minPackageVersion.Revision);
                    PackageVersion maxPackageVersionTested = new(maxVersionTested);
                    targetDeviceFamilyItem.MaxVersionTested = new Version(maxPackageVersionTested.Major, maxPackageVersionTested.Minor, maxPackageVersionTested.Build, maxPackageVersionTested.Revision);

                    targetDeviceFamilyList.Add(targetDeviceFamilyItem);
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
                LogService.WriteLog(LoggingLevel.Error, "Parse package manifest failed", e);
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
                LogService.WriteLog(LoggingLevel.Error, "Parse package manifest failed", e);
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
                architecture = ProcessorArchitecture.X86.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.X64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.X64.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Arm))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.Arm.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Arm64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.Arm64.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Neutral))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.Neutral.ToString();
            }

            if (string.IsNullOrEmpty(architecture))
            {
                architecture = unknown;
            }

            return string.Format(ResourceService.GetLocalized("Installer/BundleHeader"), architecture);
        }

        /// <summary>
        /// 解析应用捆绑包适合主机系统的文件
        /// </summary>
        private string ParsePacakgeBundleCompatibleFile(Dictionary<ProcessorArchitecture, string> applicationDict)
        {
            Architecture osArchitecture = RuntimeInformation.ProcessArchitecture;
            string appxFile = null;

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

            return appxFile;
        }

        /// <summary>
        /// 解析捆绑包每个包信息
        /// </summary>
        private PackageManifestInformation ParsePackageBundleManifestInfo(IAppxBundleManifestReader appxBundleManifestReader)
        {
            PackageManifestInformation packageManifestInformation = new();
            Dictionary<ProcessorArchitecture, string> applicationDict = [];
            Dictionary<string, string> languageResourceDict = [];
            List<string> scaleResourceList = [];

            if (appxBundleManifestReader.GetPackageInfoItems(out IAppxBundleManifestPackageInfoEnumerator appxBundleManifestPackageInfoEnumerator) is 0)
            {
                while (appxBundleManifestPackageInfoEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && hasCurrent)
                {
                    appxBundleManifestPackageInfoEnumerator.GetCurrent(out IAppxBundleManifestPackageInfo appxBundleManifestPackageInfo);

                    appxBundleManifestPackageInfo.GetFileName(out string fileName);
                    appxBundleManifestPackageInfo.GetPackageType(out APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE packageType);

                    // 获取应用捆绑包中的所有资源包
                    if (packageType is APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE.APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE_APPLICATION)
                    {
                        appxBundleManifestPackageInfo.GetPackageId(out IAppxManifestPackageId appxManifestPackageId);
                        appxManifestPackageId.GetArchitecture(out ProcessorArchitecture architecture);
                        applicationDict.TryAdd(architecture, fileName);
                        scaleResourceList.Add(fileName);
                    }
                    else
                    {
                        bool isPackageBundleScale = GetIsPackageBundleScale(appxBundleManifestPackageInfo);

                        if (isPackageBundleScale)
                        {
                            scaleResourceList.Add(fileName);
                        }
                    }

                    List<string> languageResourceList = GetPackageBundleLanguage(appxBundleManifestPackageInfo);

                    foreach (string languageResourceItem in languageResourceList)
                    {
                        languageResourceDict.TryAdd(languageResourceItem, fileName);
                    }

                    appxBundleManifestPackageInfoEnumerator.MoveNext(out _);
                }
            }

            packageManifestInformation.ApplicationDict = applicationDict;
            packageManifestInformation.LanguageResourceDict = languageResourceDict;
            packageManifestInformation.ScaleResourceList = scaleResourceList;
            return packageManifestInformation;
        }

        /// <summary>
        /// 解析应用捆绑包带缩放资源的文件
        /// </summary>
        private Dictionary<string, IAppxFile> ParsePacakgeBundleScaleFiles(IAppxBundleReader appxBundleReader, List<string> scaleResourceList)
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
                        if (packageFileName.Equals(scaleResourceItem, StringComparison.OrdinalIgnoreCase))
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
        private string GetLocalizedString(string resource, Dictionary<string, string> resourceDict)
        {
            if (!string.IsNullOrEmpty(resource) && resourceDict is not null)
            {
                string msResource = "ms-resource:";
                if (resource.StartsWith("ms-resource:"))
                {
                    string splitedResource = resource[msResource.Length..];

                    string resourceKey;
                    if (splitedResource.Contains('\\'))
                    {
                        resourceKey = splitedResource;
                    }
                    else if (resource.Contains('/'))
                    {
                        resourceKey = splitedResource.Replace('/', '\\');
                    }
                    else
                    {
                        resourceKey = string.Format(@"Resources\{0}", splitedResource);
                    }

                    foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                    {
                        if (resourceItem.Key.Equals(resourceKey, StringComparison.OrdinalIgnoreCase))
                        {
                            resource = resourceItem.Value;
                            break;
                        }
                    }
                }
            }
            else
            {
                resource = string.Empty;
            }

            return resource;
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
        private IStream GetPackageBundleLogo(string logo, Dictionary<string, IAppxFile> scaleBundleFileDict)
        {
            List<KeyValuePair<string, IAppxFile>> logoList = [];
            Dictionary<string, IAppxFile> logoDict = [];
            string logoExtensionName = Path.GetExtension(logo);
            string logoFileName = logo[..^logoExtensionName.Length];

            Regex logoRegex = new(string.Format("""{0}(.scale-\d{{3}}){{0,1}}{1}""", logoFileName.Replace(@"\", @"\\"), logoExtensionName), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // 读取应用包图标
            foreach (KeyValuePair<string, IAppxFile> scaleBundleItem in scaleBundleFileDict)
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
                            logoDict.TryAdd(packageFileName, appxFileItem.Value);
                        }
                    }
                }
            }

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
                if (fileItem.Key.Equals("resources.pri", StringComparison.OrdinalIgnoreCase))
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
        private Dictionary<string, string> GetSpecifiedLanguageResource(Dictionary<string, Dictionary<string, string>> resourceDict)
        {
            if (resourceDict is null || resourceDict.Count is 0)
            {
                return null;
            }

            CultureInfo appCultureInfo = CultureInfo.GetCultureInfo(LanguageService.AppLanguage);
            CultureInfo systemCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(LanguageService.DefaultAppLanguage);

            // 查找符合当前应用显示界面的语言资源信息
            while (!string.IsNullOrEmpty(appCultureInfo.Name))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(appCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resourceItem.Value;
                    }
                }

                appCultureInfo = appCultureInfo.Parent;
            }

            // 查找符合当前系统显示界面的语言资源信息
            while (!string.IsNullOrEmpty(systemCultureInfo.Name))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(systemCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resourceItem.Value;
                    }
                }

                systemCultureInfo = systemCultureInfo.Parent;
            }

            // 查找默认语言的资源信息
            while (!string.IsNullOrEmpty(defaultCultureInfo.Name))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(defaultCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resourceItem.Value;
                    }
                }

                defaultCultureInfo = systemCultureInfo.Parent;
            }

            // 不符合，如果存在未指定的语言，返回未指定的语言资源信息
            foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
            {
                if (resourceItem.Key.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
                {
                    return resourceItem.Value;
                }
            }

            // 不符合，如果存在资源信息，则返回第一个
            foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
            {
                return resourceItem.Value;
            }

            return null;
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
                int deviceDpi = (int)(Program.DisplayInformation.RawPixelsPerViewPixel * 100);

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
                        if (logoIndex.Equals(logoList.Count - 1))
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
                        if (logoItem.Key.Equals(logo, StringComparison.OrdinalIgnoreCase))
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
        /// 获取特定语言的资源文件
        /// </summary>
        private List<KeyValuePair<string, string>> GetPackageBundleSpecifiedLanguageResource(Dictionary<string, string> resourceDict)
        {
            List<KeyValuePair<string, string>> specifiedLanguageResourceList = [];

            if (resourceDict is null || resourceDict.Count is 0)
            {
                return null;
            }

            CultureInfo appCultureInfo = CultureInfo.GetCultureInfo(LanguageService.AppLanguage);
            CultureInfo systemCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(LanguageService.DefaultAppLanguage);

            // 查找符合当前应用显示界面的语言资源信息
            while (!string.IsNullOrEmpty(appCultureInfo.Name))
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(appCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        specifiedLanguageResourceList.Add(resourceItem);
                        break;
                    }
                }

                appCultureInfo = appCultureInfo.Parent;
            }

            // 查找符合当前系统显示界面的语言资源信息
            while (!string.IsNullOrEmpty(systemCultureInfo.Name))
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(systemCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        specifiedLanguageResourceList.Add(resourceItem);
                        break;
                    }
                }

                systemCultureInfo = systemCultureInfo.Parent;
            }

            // 查找默认语言的资源信息
            while (!string.IsNullOrEmpty(defaultCultureInfo.Name))
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(defaultCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        specifiedLanguageResourceList.Add(resourceItem);
                        break;
                    }
                }

                defaultCultureInfo = systemCultureInfo.Parent;
            }

            // 不符合，如果存在未指定的语言，返回未指定的语言资源信息
            foreach (KeyValuePair<string, string> resourceItem in resourceDict)
            {
                if (resourceItem.Key.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
                {
                    specifiedLanguageResourceList.Add(resourceItem);
                    break;
                }
            }

            // 不符合，如果存在资源信息，则返回第一个
            if (specifiedLanguageResourceList.Count is 0)
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    specifiedLanguageResourceList.Add(resourceItem);
                    break;
                }
            }

            return specifiedLanguageResourceList;
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

        #endregion 第六部分：解析应用包信息

        #region 第七部分：更新应用包信息

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
        private async Task UpdateResultAsync(Tuple<bool, PackageInformation> resultDict)
        {
            IsParseSuccessfully = resultDict.Item1;

            if (IsParseSuccessfully)
            {
                PackageInformation packageInformation = resultDict.Item2;

                PackageFileType = packageInformation.PackageFileType;
                PackageName = string.IsNullOrEmpty(packageInformation.DisplayName) ? unknown : packageInformation.DisplayName;
                PublisherDisplayName = string.IsNullOrEmpty(packageInformation.PublisherDisplayName) ? unknown : packageInformation.PublisherDisplayName;
                Version = packageInformation.Version is not null ? packageInformation.Version : new Version();
                PackageDescription = string.IsNullOrEmpty(packageInformation.Description) ? ResourceService.GetLocalized("Installer/None") : packageInformation.Description;
                PackageFamilyName = string.IsNullOrEmpty(packageInformation.PackageFamilyName) ? unknown : packageInformation.PackageFamilyName;
                PackageFullName = string.IsNullOrEmpty(packageInformation.PackageFullName) ? unknown : packageInformation.PackageFullName;
                SupportedArchitecture = string.IsNullOrEmpty(packageInformation.ProcessorArchitecture) ? unknown : packageInformation.ProcessorArchitecture;
                IsFramework = packageInformation.IsFramework.HasValue ? packageInformation.IsFramework.Value ? yes : no : unknown;
                AppInstalledState = string.IsNullOrEmpty(packageInformation.AppInstalledState) ? string.Empty : packageInformation.AppInstalledState;
                AppInstallerSourceLink = string.IsNullOrEmpty(packageInformation.AppInstallerSourceLink) ? string.Empty : packageInformation.AppInstallerSourceLink;
                IsAppInstallerSourceLinkExisted = packageInformation.IsAppInstallerSourceLinkExisted;
                PackageSourceLink = string.IsNullOrEmpty(packageInformation.PackageSourceLink) ? string.Empty : packageInformation.PackageSourceLink;
                IsPackageSourceLinkExisted = packageInformation.IsPackageSourceLinkExisted;
                PackageType = packageInformation.PackageType;
                HoursBetweenUpdateChecks = string.Format(ResourceService.GetLocalized("Installer/Hours"), packageInformation.HoursBetweenUpdateChecks);
                UpdateBlocksActivation = packageInformation.UpdateBlocksActivation ? yes : no;
                ShowPrompt = packageInformation.ShowPrompt ? yes : no;
                ForceUpdateFromAnyVersion = packageInformation.ForceUpdateFromAnyVersion ? yes : no;
                AutomaticBackgroundTask = packageInformation.AutomaticBackgroundTask ? yes : no;
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
                        if (CapabilityDict.TryGetValue(capability.ToLower(), out string capabilityValue))
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
                        LanguageCollection.Add(new Language(language));
                    }
                }

                try
                {
                    if (packageInformation.ImageLogo is not null)
                    {
                        ShCoreLibrary.CreateRandomAccessStreamOverStream(packageInformation.ImageLogo, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out IntPtr ppv);
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
                    LogService.WriteLog(LoggingLevel.Error, "Display package app icon failed", e);
                }
            }

            CanDragFile = true;
            IsLoadCompleted = true;
        }

        #endregion 第七部分：更新应用包信息

        /// <summary>
        /// 处理提权模式下的文件拖拽
        /// </summary>
        public async Task DealElevatedDragDropAsync(string filePath)
        {
            string extensionName = Path.GetExtension(filePath);

            if (CanDragFile && extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
            {
                fileName = filePath;

                if (!string.IsNullOrEmpty(fileName))
                {
                    IsParseEmpty = false;
                    ResetResult();

                    Tuple<bool, PackageInformation> parseResult = await Task.Run(async () =>
                    {
                        return await ParsePackagedAppAsync(fileName);
                    });

                    await UpdateResultAsync(parseResult);
                }
            }
        }
    }
}
