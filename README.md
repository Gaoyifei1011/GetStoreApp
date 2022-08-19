# <p align="center">欢迎访问 获取商店应用</p>

### 语言选择（Language selection）

> * [简体中文](https://github.com/Gaoyifei1011/GetStoreApp/blob/master/Description/README_ZH-CN.md)&emsp;
> * [English](https://github.com/Gaoyifei1011/GetStoreApp/blob/master/Description/README_EN-US.md)&emsp;

------

### 应用简介

微软商店提供了对已上架商店应用的分发，下载和更新通道。但是在最新的微软商店中，微软要求用户下载商店的应用需要使用在线账户。这对一些从不使用微软账户且应用必须依赖商店下载的用户带来了困扰。因此我开发了这一款获取商店应用的APP，该应用使用了 store.rg-adguard.net 提供的获取接口，绕开了微软商店官方提供的应用下载渠道。用户可以离线下载所需的应用安装包，进行独立部署。

------

### 该应用的基础功能

> * 绕开微软商店下载并离线部署 Microsoft Store 应用
> * 访问已经成功获取的历史链接和添加的下载任务
> * 访问网页版（接口出现问题时），并使用应用内置的下载工具下载

注意：该应用不能绕过微软商店的付费渠道，如果您要获取的应用是付费应用，请从微软商店购买后下载。

------

### 应用截图

#### <p align="center">应用成功获取界面</p>
![image](https://user-images.githubusercontent.com/49179966/185371877-e5f2da19-e273-43eb-b845-08eb3fe2ab3f.png)
#### <p align="center">历史记录</p>
![image](https://user-images.githubusercontent.com/49179966/185371452-ff1e6c83-0e60-40e4-97c2-e5ca78c03b51.png)
#### <p align="center">网页界面</p>
![image](https://user-images.githubusercontent.com/49179966/185371942-8f82b5c7-84cb-4810-b77d-a0d8c2f74d26.png)
#### <p align="center">应用说明</p>
![image](https://user-images.githubusercontent.com/49179966/185371766-424e3349-1758-45a8-a6ce-ffa8f238d73c.png)

------

### 项目开发进展

| 项目内容                        | 开发进展                                          |
| --------------------------------| --------------------------------------------------|
| 主页面功能                      | 已完成，下载接口正在实现中，实现完成后即可对接    |
| 历史记录（记录使用过的链接）    | 已完成                                            |
| 通过生成的链接下载文件          | 开发中，已完成50%                                 |
| 下载完成后离线部署应用          | 尚未开发                                          |
| 控制台应用程序（快速下载并部署）| 计划中                                            |
| 访问网页版对接下载接口          | 计划中                                            |
| 程序性能优化                    | 尚未开发                                          |

目前该应用处于开发阶段，有部分功能尚未实现，目前仅提供基础的功能。此外我是c#的初学者，且本人时间较为紧张，只能利用自己的闲余时间开发，开发进度较为缓慢，请谅解。

------

### 项目引用（按英文首字母排序）

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

### 下载与安装注意事项

> * 该程序使用的是Windows 应用 SDK构建的，建议您的系统版本为Windows 11（代号 21H2 / 内部版本号 22000）或更高版本，最低版本为Windows 10（代号1709 / 内部版本号17763）或更高版本。
> * 如果您的系统是Windows 10，应用功能存在一些限制：
    暂不支持设置云母/亚克力背景色
    应用部分图标使用的是Segoe Fluent  Icons图标，这一类型图标并没有内置到Windows图标，所以初次打开应用时会存在图标异常的问题。需要您亲自下载相应的[图标文件](https://docs.microsoft.com/zh-cn/windows/apps/design/downloads/#fonts)，点击右键菜单安装该字体图标文件，重启应用图标才能正常显示。
> * [Release](https://github.com/Gaoyifei1011/GetStoreApp/releases)页面的二进制安装文件已经打包成压缩包。请解压压缩包并使用Powershell管理员模式（必要情况下）运行install.ps1文件方可实现快速安装。
> * 自行下载项目源代码并编译。（请仔细阅读下面的项目编译步骤）

------

### 项目编译步骤和应用本地化

#### <p align="center">必须安装的工具</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * .NET桌面开发（Visual Studio Installer中安装，.NET SDK 版本 6.0）
> * 单打包项目工具（[Single-project MSIX Packaging Tools for VS2022](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/single-project-msix?tabs=csharp)）

#### <p align="center">编译步骤</p>

> * 克隆项目并下载代码到本地
> * 使用Visual Studio 2022打开GetStoreApp.sln文件，如果解决方案提示部分工具没有安装，请完成安装步骤后再次打开该解决方案。
> * 右键项目解决方案，生成该解决方案后点击部署解决方案。
> * 部署完成后打开“开始”菜单即可运行应用。

#### <p align="center">应用本地化</p>
##### 项目在最初仅提供简体中文和英文两种语言格式，如果您想将应用翻译到您熟悉的语言或纠正已完成翻译的内容中存在的错误，请参考下面的步骤

> * 在Description文件夹中寻找Readme模板文件，例如英文版的是README_EN-US.md文件，将其重命名为README_(对应的语言).md文件。
> * 打开重命名后的文件，翻译所有的语句后并保存。翻译完成后请您认真检查一下。
> * 打开项目主页面的README.md，在最上方的“语言选择”中添加您对应的语言。例如“英文”，注意该文字附带超链接。
> * README_(对应的语言).md文件中添加的语言截图替换为您熟悉的语言的应用截图。
> * 完成上面所述的编译步骤，确保所有步骤能够顺利运行。
> * 打开项目的Strings文件夹，并创建您使用的语言，比如（English(United States)）文件夹名称为en-us，具体可以参考表示语言(文化)代码与国家地区对照表）。
> * 打开子文件夹下的resw文件，对每一个名称进行翻译。
> * 编译运行代码并测试您的语言，应用在初次打开的时候如果没有您使用的语言默认显示English(United States)，需要在设置中动态调整。
> * 完成上述步骤后创建PR，然后将修改的内容提交到本项目，等待合并即可。

