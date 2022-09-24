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
![image](https://user-images.githubusercontent.com/49179966/190880888-ecba0107-3d5a-4b16-a3ec-e47eb7f9f166.png)
#### <p align="center">History Records</p>
![image](https://user-images.githubusercontent.com/49179966/190880874-bdbd173d-333e-4409-af1c-e3fe9d596bb5.png)
#### <p align="center">Download Page</p>
![image](https://user-images.githubusercontent.com/49179966/190881022-38955a18-fa97-4ba2-ad43-a57c2cc9d383.png)
![image](https://user-images.githubusercontent.com/49179966/190881042-78c3facc-4d10-48e8-b631-7c59427cea43.png)
#### <p align="center">Web Page</p>
![image](https://user-images.githubusercontent.com/49179966/190880867-2a96f02c-9073-4179-8689-32dd1aeef507.png)
#### <p align="center">Application Descriptions</p>
![image](https://user-images.githubusercontent.com/49179966/190881054-4d6c5d5b-2bbc-4d51-a98e-5adcc3d5a5eb.png)

------

### Project development progress

| Project progress                                         | Development progress                                                                                               |
| ---------------------------------------------------------| -------------------------------------------------------------------------------------------------------------------|
| Main page functionality                                  | Completed                                                                                                          |
| History (records used links)                             | Completed                                                                                                          |
| Download the file from the generated link                | Completed (in beta)                                                                                                |
| Deploy the app offline after the download is complete    | Completed                                                                                                          |
| Console applications (quickly download and deploy)       | Planning (Expected 0.6.0 preview implementation)                                                                   |
| Access the web version of the docking download interface | Planning (Expected 0.7.0 preview implementation)                                                                   |
| Program performance optimization                         | Planning (Expected to be implemented in version 1.0.0)                                                             |

> * At present, the application is in the development stage, some functions have not yet been implemented, and only basic functions are currently provided. In addition, I am a beginner in C#, and I am more time-constrained, I can only use my spare time to develop, the development progress is relatively slow, please understand.
> * The download function is in the testing stage, there may be instability during use, if there is an abnormality during use, please use a browser to download.
> * When you open the app for the first time, a Windows Security Center alert window will pop up, asking you to allow getstoreapparia2.exe to communicate on private and public networks for quick app download.

------

### Project References (Sort by alphabetical order)

> * [Aira2](https://aria2.github.io)&emsp;
> * [Aria2.NET](https://github.com/rogerfar/Aria2.NET)&emsp;
> * [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/WindowsCommunityToolkit)&emsp;
> * [CommunityToolkit.WinUI.Notifications](https://www.nuget.org/packages/CommunityToolkit.WinUI.Notifications)&emsp;
> * [CommunityToolkit.WinUI.UI.Behaviors](https://github.com/CommunityToolkit/WindowsCommunityToolkit)&emsp;
> * [CommunityToolkit.WinUI.UI.Controls](https://github.com/CommunityToolkit/WindowsCommunityToolkit)&emsp;
> * [HtmlAgilityPack](http://html-agility-pack.net)&emsp;
> * [Microsoft.Data.Sqlite](https://docs.microsoft.com/dotnet/standard/data/sqlite)&emsp;
> * [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)&emsp;
> * [Microsoft.Windows.CsWin32](https://github.com/Microsoft/CsWin32)&emsp;
> * [Microsoft.Windows.SDK.BuildTools](https://www.nuget.org/packages/Microsoft.Windows.SDK.BuildTools)&emsp;
> * [Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)&emsp;
> * [Microsoft.Xaml.Behaviors.WinUI.Managed](https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.WinUI.Managed)&emsp;
> * [PInvoke.SHCore](https://github.com/dotnet/pinvoke)&emsp;
> * [System.Management](https://www.nuget.org/packages/System.Management)&emsp;
> * [Template Studio](https://github.com/microsoft/TemplateStudio)&emsp;
> * [WinUIEx](https://dotmorten.github.io/WinUIEx)&emsp;

------

### Download and installation considerations

> * The program is built using the Windows Apps SDK, and it is recommended that your system version be Windows 11 (codename 21H2 / build 22000) or later, and the minimum version is Windows 10 (codename 1803 / build 18362) or later.
> * If your system is Windows 10, there are some limitations to app functionality:
    Setting the mica/acrylic background color is not supported at this time
    The application part of the icon uses the Segoe Fluent Icons icon, this type of icon is not built into the Windows icon, so there will be icon anomalies when opening the application for the first time. You need to download the corresponding [icon file](https://docs.microsoft.com/zh-cn/windows/apps/design/downloads/#fonts) yourself, click the right-click menu to install the font icon file, and restart the application icon to display normally.
> * [Release](https://github.com/Gaoyifei1011/GetStoreApp/releases) The binary installation file for the page has been packaged into a compressed package. Unzip the package and run the install.ps1 file in Powershell admin mode (if necessary) for a quick installation.
> * Download and compile the project source code yourself. (Please read the project compilation steps below carefully)

------

### Project compilation steps and app localization

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
> * Complete the translation steps described above to ensure that all steps run smoothly.
> * Open the GetStoreAppPackage packaging project, find the Package.appxmanifest file, right-click the file, click View Code, find the Sources tab, and add the corresponding language according to the template, such as "<Resource Language="EN-US"/>".
> * Open the Strings folder of the project and create the language you are using, for example ( English (United States) folder name is en-us , you can refer to the Table of Indicating Language (Culture) Codes and Countries and Regions)
> * Open the resw file under the subfolder and translate each name.
> * Compile and run the code and test your language, when the application is first opened if there is no language you use to display English (United States) by default, it needs to be dynamically adjusted in the settings.
> * Create a PR after completing the above steps, then submit the modified content to this project and wait for the merge.

