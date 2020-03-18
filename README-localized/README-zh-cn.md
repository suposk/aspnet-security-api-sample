---
page_type: sample
products:
- ms-graph
languages:
- aspx
- javascript
description: "此示例演示如何使用 Microsoft Graph SDK 将 ASP .Net Web 应用连接到安全性 API。"
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph 
  services:
  - Security
  createdDate: 4/6/2018 9:53:00 AM
---
# 针对 ASP.NET 4.6 (REST) 的 Microsoft Graph 安全性 API

## 目录

* [简介](#introduction)
* [示例项目 V1.0](#sample-project-v1.0)
* [示例项目 V2.0](#sample-project-v2.0)
* [示例项目 V3.0](#sample-project-v3.0)
* [问题和意见](#questions-and-comments)
* [参与](#contributing)
* [其他资源](#additional-resources)

## 简介

本文中共有三个示例项目，可帮助你开始使用 Microsoft Graph 安全性 API。

## 示例项目 V1.0

示例项目 V1.0 将创建一个非常基本的 Web 用户界面（也称为 UI），它将演示如何使用 Microsoft Graph 安全性 API 执行以下操作：

	• 生成和提交查询以检索警报
	• 更新警报的生命周期字段（例如状态、反馈和评论等）
	• 订阅警报通知（基于筛选的查询）以及警报通知的示例侦听器。
此项目使用 Microsoft Graph 生成的 SDK 与 Microsoft Graph API 交互。
	  
	> 注意：你还可以直接使用 REST 查询与 Graph 安全性 API 服务交互。

## 示例项目 V2.0

示例项目 V2.0 将创建 V1.0 UI 的高级版本，它包括显示警报的统计视图的“仪表板功能区”（例如有风险的用户，即具有最严重警报的用户），将演示如何使用 Microsoft Graph 安全性 API 执行以下操作：

	• 检索客户组织的安全功能分数和安全功能控制配置文件
	    > 再加上 V1.0 的功能（如下所示）
	• 生成和提交查询以检索警报
	• 更新警报的生命周期字段（例如状态、反馈和评论等）
	• 订阅警报通知（基于筛选的查询）以及警报通知的示例侦听器。

在该高级 UI 中，可单击几乎任何属性来生成针对该属性值的筛选查询，从而实现直观的“指向并单击”调查体验。

	> 注意：V 2.0 使用由 Microsoft Graph 生成的 SDK 来调用 Microsoft Graph 安全性 API，以获得警报和订阅，
	 并使用 REST 调用来获取安全功能分数和安全功能控制配置文件（因为这些还在测试阶段）。


## 示例项目 V3.0

示例项目 V3.0 将创建 V2.0 UI 的更高级版本，它使用角度服务器并可直接用作分析人员的调查工具，将演示如何使用 Microsoft Graph 安全性 API 执行以下操作：

	• 创建安全操作（例如：阻止 IP）并检索这些操作 
	• 提供在 UI 上针对警报、安全功能分数、安全操作和订阅进行高级筛选的功能
	    > 再加上 V2.0 的功能（如下所示）
	• 生成和提交查询以检索警报
	• 更新警报的生命周期字段（例如状态、反馈和评论等）
	• 检索客户组织的安全功能分数和安全功能控制配置文件
	• 订阅警报通知（基于筛选的查询）以及警报通知的示例侦听器。


## 问题和意见

我们乐意倾听你对此示例的反馈！
请在该存储库中的[问题](https://github.com/microsoftgraph/aspnet-connect-rest-sample/issues)部分将问题和建议发送给我们。

你的反馈对我们意义重大。请在[堆栈溢出](https://stackoverflow.com/questions/tagged/microsoftgraph)上与我们联系。
使用 [MicrosoftGraph] 标记出你的问题。

## 参与 ##

如果想要参与本示例，请参阅 [CONTRIBUTING.md](CONTRIBUTING.md)。

此项目已采用 [Microsoft 开放源代码行为准则](https://opensource.microsoft.com/codeofconduct/)。
有关详细信息，请参阅[行为准则常见问题解答](https://opensource.microsoft.com/codeofconduct/faq/)。如有其他任何问题或意见，也可联系 [opencode@microsoft.com](mailto:opencode@microsoft.com)。

## 其他资源

- [其他 Microsoft Graph Connect 示例](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph 概述](https://graph.microsoft.io)

## 版权信息
版权所有 (c) 2018 Microsoft。保留所有权利。



