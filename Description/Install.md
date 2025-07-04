### <p align="center">学习过程中参考或使用的代码</p>
### <p align="center">Code referenced or used during the learning process</p>

------

#### ① 使用应用安装程序安装应用
1. 下载最新的文件`GetStoreAppPackage_x.x.xxx.0_x86_x64_arm64.msixbundle`（以 msixbundle 扩展名结尾）
2. [开启开发人员模式](https://learn.microsoft.com/zh-cn/windows/application-management/sideload-apps-in-windows)，如果您想开发UWP应用，您可以开启[开发人员模式](https://docs.microsoft.com/zh-cn/windows/uwp/get-started/enable-your-device-for-development)，对于大多数不需要做 UWP 开发的用户来说，开发人员模式是没有必要的
3. 安装`Dependencies`文件夹下的适用于您的设备的所有依赖包
4. 安装`*.cer`证书到`本地计算机`→`受信任的根证书颁发机构`。项操作需要用到管理员权限，如果您安装证书时没有用到该权限，则可能是因为您将证书安装到了错误的位置或者您使用的是超级管理员账户
5. 打开`GetStoreAppPackage_x.x.xxx.0_x86_x64_arm64.msixbundle`文件，单击安装

#### ① Use the application installer to install the application
1. Download the latest file `GetStoreAppPackage_x.x.xxx.0_x86_x64_arm64.msixbundle` (ending with the msixbundle extension).
2. [Open Developer mode](https://learn.microsoft.com/zh-cn/windows/application-management/sideload-apps-in-windows), if you want to develop UWP application, You can open [developer mode](https://docs.microsoft.com/zh-cn/windows/uwp/get-started/enable-your-device-for-development), For most users who do not need to do UWP development, the developer mode is not necessary
3. Install all `Dependencies` that apply to your device in the Dependencies folder
4. Install `*.cer` certificates to `local computer`→`Trusted root Certification Authority`. If you do not use this permission when you install the certificate, it may be because you installed the certificate in the wrong location or you are using a super administrator account
5. Open the `GetStoreAppPackage_x.x.xxx.0_x86_x64_arm64.msixbundle` file and click Install

=================================

#### ② 使用应用安装脚本安装应用
1. 下载并解压最新的压缩包`GetStoreAppPackage_x.x.xxx.0.zip`（以 zip 扩展名结尾）
2. 进入目录，右击`Install.ps1`，选择“使用PowerShell运行”
3. 应用安装脚本将会引导您完成此过程的剩余部分

- 安装过程中可能遇到的问题
在使用Powershell安装应用时，您可能会遇到无法运行脚本的错误，导致应用无法正常安装。这是因为计算机上启动 Windows PowerShell 时，执行策略很可能是 Restricted（默认设置），Restricted 执行策略不允许任何脚本运行。
AllSigned 和 RemoteSigned 执行策略可防止 Windows PowerShell 运行没有数字签名的脚本。
可以使用以下命令在不更改执行策略时运行脚本（需要管理员权限）
`powershell -ExecutionPolicy ByPass -File "Install.ps1"`

#### ② Use the application installation script to install applications
1. Download and extract the latest package `GetStoreAppPackage_x.x.xxx.0.zip` (end with zip extension)
2. Go to the directory, right click `Install.ps1` and select 'Run with PowerShell'.
3. Applying the installation script will guide you through the rest of the process

- Problems that may be encountered during installation
When installing an application using Powershell, you may encounter an error that prevents the script from running, causing the application to not install properly. This is because when Windows PowerShell is started on your computer, the execution policy is most likely Restricted (the default), which does not allow any scripts to run.
AllSigned and RemoteSigned execution policies prevent Windows PowerShell from running scripts that are not digitally signed.
You can run the script without changing the execution policy using the following command (administrator permissions required)
`powershell -ExecutionPolicy ByPass -File "Install.ps1"`

=================================

#### ③ 安装参考链接
https://github.com/Coolapk-UWP/Coolapk-UWP

#### ③ Install reference link
https://github.com/Coolapk-UWP/Coolapk-UWP