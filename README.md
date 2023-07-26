<div style="text-align: center;">

![LOGO](imgs/dog.webp "理工汪")
# 理工汪
开源的教务在线接口/课程表查看工具（哈尔滨理工大学）

[![LICENSE](https://img.shields.io/badge/license-MIT-informational)](LICENSE)

</div>

## 开发动机
> 校园网好烂……  
没网查教室好难……  
小程序体验好糟糕……  
上网课好累……在家都没人叫我……  

## 功能特色
* 课表展示
* 考试查询
* 本地缓存
* 上课提醒（仅 Windows 端）

## 技术特色
* MVVM 架构，界面逻辑分离
* 在 WPF 桌面开发中应用依赖注入
* XAML 自定义控件，单一化职责
* 使用 .NET MAUI 前沿跨平台框架
* 入门学习项目，不编了……

## 界面展示
![screenshot](imgs/screenshots.webp "界面展示")

## 项目结构
* `Hrbust`：封装的哈尔滨理工大学教务在线 API 库，接口均为模拟网页请求得到，使用 `HtmlAgilityPack` 包来完成 HTML 解析操作。
* `HrbustDoggy.Cli`：一个简单的命令行客户端，用来测试上述 API 库的功能，也可满足简单使用。
* `HrbustDoggy.Maui`：使用 .NET MAUI 框架开发的跨平台客户端。
* `HrbustDoggy.Wpf`：使用 WPF 框架开发的 Windows 桌面客户端。

## 构建说明
* 生成此项目前，请确保安装 .NET 7.0（或更新版本）SDK。  
执行 `dotnet workload restore` 以安装解决方案所需的工作负载。
* 此项目使用 Visual Studio 2022 开发，理论上 Visual Studio Code 和 .NET CLI 也能胜任开发工作。  
* 删除了 HrbustDoggy.Maui 项目中的 MacOS 和 iOS 平台目标。理论上该项目也能生成 MacOS 和 iOS 相应可执行文件，有条件可自行测试。

## 注释
整体来说是个入门项目，没有技术含量，但在细节方面下了不少功夫，有需要的同学可作为参考项目学习。  
> 另外亲身实践证明浪费时间学习 MAUI 是个错误的决定。  
目前（开发时）的 MAUI 仍然非常多坑，而且一旦掉进坑里很可能翻遍文档和论坛网站都找不到解决方案。  
虽然勉强能用，但确实不值得投入精力学习。  
微软再次令人失望，不如把目光转向更受欢迎（得多）的 Flutter 或者 React Native。谨以此为戒。