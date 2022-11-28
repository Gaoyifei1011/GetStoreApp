# <p align="center">欢迎访问 获取商店应用</p>

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
![image](https://user-images.githubusercontent.com/49179966/190880785-3df61d85-2134-41c1-bb94-6cd9a91cf1e5.png)
#### <p align="center">历史记录</p>
![image](https://user-images.githubusercontent.com/49179966/190880908-fe88b95f-28e9-4081-8ad0-95fbf8b36295.png)
#### <p align="center">下载界面</p>
![image](https://user-images.githubusercontent.com/49179966/190880969-07aba750-0c0a-474b-ab73-69176c25145e.png)
![image](https://user-images.githubusercontent.com/49179966/190880987-71946b4e-6267-4315-b0e7-a68349ff1f56.png)
#### <p align="center">网页界面</p>
![image](https://user-images.githubusercontent.com/49179966/190880935-c4efdcbf-f8ef-44e3-906b-27357be25795.png)
#### <p align="center">应用说明</p>
![image](https://user-images.githubusercontent.com/49179966/190880798-eb849860-589b-4f24-9112-09514cfd1964.png)

------

### 项目开发进展

| 项目内容                        | 开发进展                                          |
| --------------------------------| --------------------------------------------------|
| 主页面功能                      | 已完成                                            |
| 历史记录（记录使用过的链接）    | 已完成                                            |
| 通过生成的链接下载文件          | 已完成（处于测试阶段）                            |
| 下载完成后离线部署应用          | 已完成                                            |
| 访问网页版对接下载接口          | 已完成                                            |
| 控制台应用程序（快速下载并部署）| 计划中（预计0.8.0预览版本实现）                   |
| 程序性能优化                    | 计划中（预计1.0.0正式版本实现）                   |

> * 目前该应用处于开发阶段，有部分功能尚未实现，目前仅提供基础的功能。此外我是c#的初学者，且本人时间较为紧张，只能利用自己的闲余时间开发，开发进度较为缓慢，请谅解。
> * 下载功能处于测试阶段，在使用过程中可能存在不稳定现象，如果在使用过程中出现异常，请使用浏览器下载。

------

### 项目引用（按英文首字母排序）

> * [Aira2](https://aria2.github.io)&emsp;
> * [HtmlAgilityPack](http://html-agility-pack.net)&emsp;
> * [Microsoft.Data.Sqlite.Core](https://docs.microsoft.com/dotnet/standard/data/sqlite)&emsp;
> * [Microsoft.Windows.CsWinRT](https://github.com/microsoft/cswinrt)&emsp;
> * [Microsoft.Windows.SDK.Contracts](https://aka.ms/WinSDKProjectURL)&emsp;
> * [Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)&emsp;
> * [SQLitePCLRaw.bundle_winsqlite3](https://github.com/ericsink/SQLitePCL.raw)&emsp;

------

### 下载与安装注意事项

> * 该程序使用的是Windows 应用 SDK构建的，建议您的系统版本为Windows 11（代号 21H2 / 内部版本号 22000）或更高版本，最低版本为Windows 10（代号1803 / 内部版本号18362）或更高版本。
> * 如果您的系统是Windows 10，应用功能存在一些限制：
    暂不支持设置云母/云母Alt/亚克力背景色
> * [Release](https://github.com/Gaoyifei1011/GetStoreApp/releases)页面的二进制安装文件已经打包成压缩包。请解压压缩包并使用Powershell管理员模式（必要情况下）运行install.ps1文件方可实现快速安装。
> * 自行下载项目源代码并编译。（请仔细阅读下面的项目编译步骤）

------

### 项目编译步骤和应用本地化

#### <p align="center">必须安装的工具</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * .NET桌面开发（Visual Studio Installer中安装，.NET SDK 版本 6.0）
> * [Microsoft Edge WebView2 运行时](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/) （推荐安装）

#### <p align="center">编译步骤</p>

> * 克隆项目并下载代码到本地
> * 使用Visual Studio 2022打开GetStoreApp.sln文件，如果解决方案提示部分工具没有安装，请完成安装工具步骤后再次打开该解决方案。
> * 还原项目的Nuget包。
> * 还原完成后，右键项目解决方案，生成该解决方案后点击部署解决方案。
> * 部署完成后打开“开始”菜单即可运行应用。

#### <p align="center">应用本地化</p>
##### 项目在最初仅提供简体中文和英文两种语言格式，如果您想将应用翻译到您熟悉的语言或纠正已完成翻译的内容中存在的错误，请参考下面的步骤。

> * 在Description文件夹中寻找Readme模板文件，例如英文版的是README_EN-US.md文件，将其重命名为README_(对应的语言).md文件。
> * 打开重命名后的文件，翻译所有的语句后并保存。翻译完成后请您认真检查一下。
> * 打开项目主页面的README.md，在最上方的“语言选择”中添加您对应的语言。例如“英文”，注意该文字附带超链接。
> * README_(对应的语言).md文件中添加的语言截图替换为您熟悉的语言的应用截图。
> * 完成上面所述的翻译步骤，确保所有步骤能够顺利运行。
> * 打开GetStoreAppPackage打包项目，找到Package.appxmanifest文件，右键该文件，点击查看代码，找到Resources标签，根据模板添加相对应的语言，例如“<Resource Language="EN-US"/>”。
> * 打开项目的Strings文件夹，并创建您使用的语言，比如（English(United States)）文件夹名称为en-us，具体可以参考表示语言(文化)代码与国家地区对照表）。
> * 打开子文件夹下的resw文件，对每一个名称进行翻译。
> * 编译运行代码并测试您的语言，应用在初次打开的时候如果没有您使用的语言默认显示English(United States)，需要在设置中动态调整。
> * 完成上述步骤后创建PR，然后将修改的内容提交到本项目，等待合并即可。

------

## 项目 Star 数量统计趋势图
[![Stargazers over time](https://starchart.cc/Gaoyifei1011/GetStoreApp.svg)](https://starchart.cc/Gaoyifei1011/GetStoreApp)
