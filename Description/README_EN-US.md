<div align=center>
<img src="https://user-images.githubusercontent.com/49179966/219057775-f8d6abb5-c9c3-46c6-829e-05d164937b76.png" width="140" height="140"/>
</div>

# <p align="center">Welcome to GetStoreApp</p>

### Application brief introduction

The Microsoft Store provides a distribution, download, and update channel for apps that are already on the shelves. But in the latest Microsoft Store, Microsoft requires users to use an online account to download apps from the store. This has plagued some users who never use a Microsoft account and whose apps must rely on store downloads. The app uses the acquisition interface provided by store.rg-adguard.net, bypassing the official App Download Channel provided by the Microsoft Store. Users can download the required app installation packages offline for standalone deployment.

------

### The basic functionality of the app

> * Bypass Microsoft Store downloads and deploy Microsoft Store apps offline
> * Access the historical links that have been successfully obtained and the download tasks that were added
> * Access the web version (when there is a problem with the interface) and download it using the app's built-in download tool
> * Install, uninstall, and upgrade the WinGet package

Note: The app can't bypass the Microsoft Store's billing channels, and if the app you're getting is a paid app, download it after purchasing it from the Microsoft Store.

------

### Screenshot of the app

#### <p align="center">The app successfully gets the interface</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/75d5bc77-2680-4692-9f2e-fa1126c44ea5" />
</div>

#### <p align="center">WinGet package management interface</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/9788e38e-4614-4f4f-b080-ed9c72c57995" />
</div>

<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/5938b36c-d4db-4784-b020-02251422875c" />
</div>

<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/59a56868-7762-4db8-b8b9-31cad199c97f" />
</div>

#### <p align="center">History Record</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/2a00ef92-ca15-4679-a962-49a5396aee96" />
</div>

#### <p align="center">Download Management</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/2177ee35-80a8-49fc-9c99-b82fb92fe470" />
</div>

<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/7764c08f-e1b8-4bdd-ab25-e8f9cee94f50" />
</div>

#### <p align="center">Access Webpage</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/f3d50960-0afe-43ff-bc1a-bc257633f001" />
</div>

#### <p align="center">Application Descriptions</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/GetStoreApp/assets/49179966/0743b5a6-30a4-4f71-9b4e-a38c3bc58061" />
</div>

------

### Project development progress

| Project progress                                         | Development progress                                                                                               |
| ---------------------------------------------------------| -------------------------------------------------------------------------------------------------------------------|
| Microsoft Store link to get                              | Completed                                                                                                          |
| WinGet package to get                                    | Completed                                                                                                          |
| History (records used links)                             | Completed                                                                                                          |
| Download the file from the generated link                | Completed                                                                                                          |
| Deploy the app offline after the download is complete    | Completed                                                                                                          |
| Access the web version of the docking download interface | Completed                                                                                                          |
| Console applications (quickly download)                  | Completed                                                                                                          |
| Program performance optimization                         | Completed                                                                                                          |
| Interface modernization                                  | Completed                                                                                                          |

All the content of the program has been developed

------

### Project References (Sort by alphabetical order)

> * [Microsoft.Windows.CsWinRT](https://github.com/microsoft/cswinrt)&emsp;
> * [Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)&emsp;
> * [Microsoft.WindowsPackageManager.ComInterop](https://github.com/microsoft/winget-cli)&emsp;
> * [Mile.Aria2](https://github.com/ProjectMile/Mile.Aria2)&emsp;

[Code referenced or used during the learning process](https://github.com/Gaoyifei1011/GetStoreApp/blob/main/Description/StudyReferenceCode.md)&emsp;

------

### Download and installation considerations

> * The program is built using the Windows Apps SDK, and it is recommended that your system version be Windows 11 (codename 21H2 / build 22000) or later, and the minimum version is Windows 10 (codename 1903 / build 18362) or later.
> * If your system is Windows 10, there are some limitations to app functionality:
    Setting the mica/mica alt background color is not supported
> * [Release](https://github.com/Gaoyifei1011/GetStoreApp/releases) The binary installation file for the page has been packaged into a compressed package. Unzip the package and run the install.ps1 file in Powershell admin mode (if necessary) for a quick installation.
> * Download and compile the project source code yourself. (Please read the project compilation steps below carefully)

------

### Project compilation steps and app localization

#### <p align="center">Tools that must be installed</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * . NET Desktop Development (Installed in Visual Studio Installer, .NET SDK Version 7.0)
> * [WinGet package manager](https://www.microsoft.com/store/productId/9NBLGGH4NNS1) (install recommendedly)
> * [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/) (install recommendedly)

#### <p align="center">Compilation steps</p>

> * Fork the project and download the code locally
> * Use Visual Studio 2022 to open the GetStoreApp.sln file. If the solution prompts that some tools are not installed, complete the installation steps and open the solution again.
> * Restore the Nuget package for the project.
> * After the restore is complete, right-click the project solution, generate the solution and click Deploy Solution.
> * Open the Start menu after the deployment is complete to run the app.

#### <p align="center">App localization</p>
##### The project was initially available in both Chinese Simplified and English formats, and if you want to translate your app into a language you are familiar with or correct errors in content that has been translated, please refer to the steps below.

> * Look for readme template files in the DeScription folder, for example, the English version is a README_EN-US.md file, rename it to README_(corresponding language).md file.
> * Open the renamed file, translate all the statements and save them. Please check it carefully after the translation is completed.
> * Open the README.md of the project's home page and add your language in the language selection at the top. For example, "English", note that the text is accompanied by a hyperlink.
> * README_ (corresponding language).The language screenshot added in the md file is replaced with the app screenshot in the language you are familiar with.
> * Complete the translation steps described above to ensure that all steps run smoothly.
> * Open the GetStoreAppPackage packaging project, find the Package.appxmanifest file, right-click the file, click View Code, find the Sources tab, and add the corresponding language according to the template, such as "<Resource Language="EN-US"/>".
> * Open the Strings folder of the GetStoreApp project and create the language you are using, for example ( English (United States) folder name is en-us , you can refer to the Table of Indicating Language (Culture) Codes and Countries and Regions)
> * Open the resw file under the subfolder and translate each name.
> * Compile and run the code and test your language, when the application is first opened if there is no language you use to display English (United States) by default, it needs to be dynamically adjusted in the settings.
> * Create a PR after completing the above steps, then submit the modified content to this project and wait for the merge.

------

### Thanks (Sort by alphabetical order)

> * [AndromedaMelody](https://github.com/AndromedaMelody)&emsp;
> * [cnbluefire](https://github.com/cnbluefire)&emsp;
> * [飞翔](https://fionlen.azurewebsites.net)&emsp;
> * [MouriNaruto](https://github.com/MouriNaruto)&emsp;
> * [TaylorShi](https://github.com/TaylorShi)&emsp;
> * [wherewhere](https://github.com/wherewhere)&emsp;

------

### Other content

> * This is the first small project I personally practiced when learning C#, because the advanced content about C# is not very deep, so there are many deficiencies in code content and quality, I hope to cover more.
> * The project will last 12 months and 10 days from 20 May 2022 to 30 May 2023.
> * This project is an open source project licensed under the MIT license, and you may modify, distribute, or merge copies with new copies. If you use the project, please do not use it for illegal purposes, and the developer will not be held responsible.

------

### Trend chart of project Star quantity statistics
[![Stargazers over time](https://starchart.cc/Gaoyifei1011/GetStoreApp.svg)](https://starchart.cc/Gaoyifei1011/GetStoreApp)
