# GetStoreApp

### 通过访问Microsoft Store服务器下载离线的应用商店安装包。<br>Download offline app store installation packages by visiting the Microsoft Store server.
### 该项目使用的是[Store](https://store.rg-adguard.net/)提供的API来生成的链接。<br>The project uses the API provided by the [Store] (https://store.rg-adguard.net/) to generate links.

| 项目进度                     | 开发进展 |
| --------------------------- | ----------- |
| 主页面功能                   | 已完成90%的功能（可以通过浏览器下载）     |
| 历史记录（记录使用过的链接）   | 已完成        |
| 通过生成的链接下载文件         | 0%，计划中        |
| 应用说明                     | 已完成       |
| 控制台应用程序                    | 0%，计划中      |

| Project progress                    | Development progress |
| --------------------------- | ----------- |
| Main page functionality                    | 90% of the features completed (can be downloaded via browser)     |
| History (records used links)  | Finished       |
| Download the file from the generated link         | 0% is planned        |
| Application Notes                   | Finished       |
| Console applications                       | 0% is planned      |

项目暂时停止开发，由于原API网页需要通过Cloudflare网页验证(添加了反爬虫验证)，需要解决这个问题后抽时间继续开发。

### 该项目的功能
#### 该项目主要包括以下功能：通过[Store](https://store.rg-adguard.net/)提供的API来生成带有文件链接
#### 记录已经成功生成内容的链接和选择的选项，方便下次使用
#### 在应用内下载已经下载完成的安装包，并可以自行安装（使用App Installer或Powershell）

### Features of the project
#### The project mainly includes the following features: generating with file links through the API provided by the [Store] (https://store.rg-adguard.net/).
#### Record the link to the content that has been successfully generated and the options selected for easy use next time
#### Download the downloaded install package in-app and install it yourself (using App Installer or Powershell)

### 使用参考的库或项目
#### [Html Agility Pack](https://github.com/zzzprojects/html-agility-pack)
#### [microsoft-ui-xaml](https://github.com/microsoft/microsoft-ui-xaml)
#### [MVVM Toolkit](https://docs.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/introduction) 

### Use a reference library or project
#### [Html Agility Pack](https://github.com/zzzprojects/html-agility-pack)
#### [microsoft-ui-xaml](https://github.com/microsoft/microsoft-ui-xaml)
#### [MVVM Toolkit](https://docs.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/introduction)

### 应用运行图（中文（简体））
#### 应用初次打开界面
![image](https://user-images.githubusercontent.com/49179966/173263943-27d72513-4b0e-4cb6-a933-85e029e36cad.png)
#### 应用成功获取界面
![image](https://user-images.githubusercontent.com/49179966/173263928-e6b3dfd0-13f8-4893-91f5-aaf4a045e19f.png)

### Application Operation Diagram (English(United States))
#### The app opens the interface for the first time
![image](https://user-images.githubusercontent.com/49179966/173263973-b921e7cd-7374-412d-a0fd-3bd08e8553f8.png)
#### The app successfully gets the interface
![image](https://user-images.githubusercontent.com/49179966/173263993-b3301a71-b349-4f99-af66-e651e9e8cad1.png)

### 为项目作出贡献
#### 多语言翻译
##### 该应用已经支持多语言功能，您只需要以下几步就能完成。
##### 第一步：Fork该项目并下载代码到本地
##### 第二步：打开Visual Studio 2022
##### 第三步：在GetStoreApp项目中打开Strings文件夹，并创建您使用的语言，比如（English(United States)）文件夹名称为en-us，具体可以参考表示语言(文化)代码与国家地区对照表
##### 第四步：进行翻译
##### 第五步：编译运行代码并测试您的语言，应用在初次打开的时候如果没有您使用的语言默认显示English(United States)
##### 第六步：创建PR，然后将修改的内容提交到本项目，等待合并即可


### Contribute to the project
#### Multilingual translation
##### The app already supports multilingual features, and you only need the following few steps to complete.
##### Step 1: Fork the project and download the code locally
##### Step 2: Open Visual Studio 2022
##### Step 3: Open the Strings folder in the GetStoreApp project and create the language you are using, such as (English (United States)) folder name en-us, you can refer to the table of language (culture) codes and countries
##### Step 4: Translate
##### Step 5: Compile and run the code and test your language, english (United States) is displayed by default if the application is not in the language you are using when it is first opened
##### Step 6: Create a PR, then submit the modified content to this project and wait for the merge

##### 目前该应用处于开发阶段，有大量功能尚未实现，目前仅提供基础的功能，由于我是c#的初学者，且本人时间较为紧张，只能利用自己的闲余时间开发，开发进度较为缓慢，请谅解<br>At present, the application is in the development stage, there are a large number of functions have not yet been implemented, currently only provide basic functions, because I am a beginner in C#, and I am more time-critical, can only use their spare time to develop, the development progress is relatively slow, please understand
