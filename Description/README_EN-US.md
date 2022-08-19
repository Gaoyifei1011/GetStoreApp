# <p align="center">Welcome to GetStoreApp</p>

### Application brief introduction

The Microsoft Store provides a distribution, download, and update channel for apps that are already on the shelves. But in the latest Microsoft Store, Microsoft requires users to use an online account to download apps from the store. This has plagued some users who never use a Microsoft account and whose apps must rely on store downloads. The app uses the acquisition interface provided by store.rg-adguard.net, bypassing the official App Download Channel provided by the Microsoft Store. Users can download the required app installation packages offline for standalone deployment.

------

### The basic functionality of the app

> * Bypass Microsoft Store downloads and deploy Microsoft Store apps offline
> * Access the historical links that have been successfully obtained and the download tasks that were added
> * Access the web version (when there is a problem with the interface) and download it using the app's built-in download tool

Note: The app can't bypass the Microsoft Store's billing channels, and if the app you're getting is a paid app, download it after purchasing it from the Microsoft Store.

------

### Screenshot of the app

#### <p align="center">The app successfully gets the interface</p>
![image](https://user-images.githubusercontent.com/49179966/185371877-e5f2da19-e273-43eb-b845-08eb3fe2ab3f.png)
#### <p align="center">History Records</p>
![image](https://user-images.githubusercontent.com/49179966/185371452-ff1e6c83-0e60-40e4-97c2-e5ca78c03b51.png)
#### <p align="center">Web interface</p>
![image](https://user-images.githubusercontent.com/49179966/185371942-8f82b5c7-84cb-4810-b77d-a0d8c2f74d26.png)
#### <p align="center">Application Notes</p>
![image](https://user-images.githubusercontent.com/49179966/185371766-424e3349-1758-45a8-a6ce-ffa8f238d73c.png)

------

### Project development progress

| Project progress                                         | Development progress                                                                                               |
| ---------------------------------------------------------| -------------------------------------------------------------------------------------------------------------------|
| Main page functionality                                  | Completed, the download interface is being implemented, and it can be docked after the implementation is completed |
| History (records used links)                             | Completed                                                                                                          |
| Download the file from the generated link                | In development, 50% complete                                                                                       |
| Deploy the app offline after the download is complete    | Developed not yet                                                                                                  |
| Console applications (quickly download and deploy)       | Planning                                                                                                           |
| Access the web version of the docking download interface | Planning                                                                                                           |
| Program performance optimization                         | Developed not yet                                                                                                  |

At present, the application is in the development stage, some functions have not yet been implemented, and only basic functions are currently provided. In addition, I am a beginner in C#, and I am more time-constrained, I can only use my spare time to develop, the development progress is relatively slow, please understand.

------

### Project References (Sort by alphabetical order)

> * [Aira2](https://aria2.github.io)&emsp;
> * [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/WindowsCommunityToolkit)&emsp;
> * [CommunityToolkit.WinUI.UI.Controls.DataGrid](https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/datagrid)&emsp;
> * [CommunityToolkit.WinUI.Notifications](https://www.nuget.org/packages/CommunityToolkit.WinUI.Notifications)&emsp;
> * [HtmlAgilityPack](http://html-agility-pack.net)&emsp;
> * [Microsoft.Data.Sqlite](https://docs.microsoft.com/dotnet/standard/data/sqlite)&emsp;
> * [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)&emsp;
> * [Microsoft.Windows.CsWin32](https://github.com/Microsoft/CsWin32)&emsp;
> * [Microsoft.Windows.SDK.BuildTools](https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools)&emsp;
> * [Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)&emsp;
> * [Microsoft.Xaml.Behaviors.WinUI.Managed](https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.WinUI.Managed)&emsp;
> * [NETStandard.Library](https://www.nuget.org/packages/NETStandard.Library)&emsp;
> * [Newtonsoft.Json](https://www.newtonsoft.com/json)&emsp;
> * [PInvoke.SHCore](https://github.com/dotnet/pinvoke)&emsp;
> * [System.Management](https://www.nuget.org/packages/System.Management)&emsp;
> * [Template Studio](https://github.com/microsoft/TemplateStudio)&emsp;
> * [WinUIEx](https://dotmorten.github.io/WinUIEx)&emsp;

------

### Download and installation considerations

> * The program is built using the Windows Apps SDK, and it is recommended that your system version be Windows 11 (codename 21H2 / build 22000) or later, and the minimum version is Windows 10 (codename 1709 / build 17763) or later.
> * If your system is Windows 10, there are some limitations to app functionality:
    Setting the mica/acrylic background color is not supported at this time
    The application part of the icon uses the Segoe Fluent Icons icon, this type of icon is not built into the Windows icon, so there will be icon anomalies when opening the application for the first time. You need to download the corresponding [icon file](https://docs.microsoft.com/zh-cn/windows/apps/design/downloads/#fonts) yourself, click the right-click menu to install the font icon file, and restart the application icon to display normally.
> * [Release](https://github.com/Gaoyifei1011/GetStoreApp/releases) The binary installation file for the page has been packaged into a compressed package. Unzip the package and run the install.ps1 file in Powershell admin mode (if necessary) for a quick installation.
> * Download and compile the project source code yourself. (Please read the project compilation steps below carefully)

------

### Project compilation steps

#### <p align="center">Tools that must be installed</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * . NET Desktop Development (Installed in Visual Studio Installer, .NET SDK Version 6.0)
> * Single-package project tool（[Single-project MSIX Packaging Tools for VS2022](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/single-project-msix?tabs=csharp)）

#### <p align="center">Compilation steps</p>

> * Fork the project and download the code locally
> * Open the GetStoreApp.sln file using Visual Studio 2022, and if the solution prompts some of the tools are not installed, open the solution again after completing the installation steps.
> * Right-click the project solution, build the solution, and click Deploy Solution.
> * Open the Start menu after the deployment is complete to run the app.

#### <p align="center">App localization</p>
##### The project was initially available in both Chinese Simplified and English formats, and if you want to translate your app into a language you are familiar with or correct errors in content that has been translated, please refer to the steps below.

> * Look for readme template files in the DeScription folder, for example, the English version is a README_EN-US.md file, rename it to README_(corresponding language).md file.
> * Open the renamed file, translate all the statements and save them. Please check it carefully after the translation is completed.
> * Open the README.md of the project's home page and add your language in the language selection at the top. For example, "English", note that the text is accompanied by a hyperlink.
> * README_ (corresponding language).The language screenshot added in the md file is replaced with the app screenshot in the language you are familiar with.
> * Complete the compilation steps described above to ensure that all steps run smoothly.
> * Open the Strings folder of the project and create the language you are using, for example ( English (United States) folder name is en-us , you can refer to the Table of Indicating Language (Culture) Codes and Countries and Regions)
> * Open the resw file under the subfolder and translate each name.
> * Compile and run the code and test your language, when the application is first opened if there is no language you use to display English (United States) by default, it needs to be dynamically adjusted in the settings.
> * Create a PR after completing the above steps, then submit the modified content to this project and wait for the merge.

