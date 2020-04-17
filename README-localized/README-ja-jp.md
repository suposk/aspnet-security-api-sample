---
page_type: sample
products:
- ms-graph
languages:
- aspx
- javascript
description: "このサンプルでは、Microsoft Graph SDK を使用して、ASP .Net Web アプリを Security API に接続する方法を示しています。"
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph 
  services:
  - Security
  createdDate: 4/6/2018 9:53:00 AM
---
# ASP.NET 4.6 用 Microsoft Graph Security API のサンプル (REST)

## 目次

* [概要](#introduction)
* [サンプル プロジェクト V1.0](#sample-project-v1.0)
* [サンプル プロジェクト V2.0](#sample-project-v2.0)
* [サンプル プロジェクト V3.0](#sample-project-v3.0)
* [質問とコメント](#questions-and-comments)
* [投稿](#contributing)
* [その他のリソース](#additional-resources)

## 概要

Microsoft Graph Security API の使用を開始するための 3 つのサンプル プロジェクトがあります。

## サンプル プロジェクト V1.0

サンプル プロジェクト V 1.0 は、Microsoft Graph Security API を使用して以下を示す非常に基本的な Web ユーザーインターフェイス (別名 UI) を作成します。

	• クエリを構築して送信してアラートを取得する
	• アラートのライフサイクル フィールド (たとえば、状態、フィードバック、コメント.) を更新する
	• アラート通知 (フィルター処理されたクエリに基づく) およりアラート通知用のサンプル リスナーにサブスクライブする。
このプロジェクトは、Microsoft Graph によって生成された SDK を使用して、Microsoft Graph API を操作します。
	  
	> メモ: REST クエリを使用して、Graph Security API サービスを操作することもできます

## サンプル プロジェクト V2.0

サンプル プロジェクトV2.0 は、V1.0 UI の高度なバージョンを作成します。これには、Microsoft Graph Security API を使用して以下を示すアラート (たとえば、リスクのあるユーザー、つまり最も重大度の高いアラートのあるユーザー) の統計ビューを表示する 'ダッシュボード リボン' が含まれます。

	• 顧客組織のセキュア スコアおよびセキュア スコア制御プロファイルを取得する
	    > V1.0 の機能 (下記参照) を含みます
	• クエリを構築して送信してアラートを取得する
	• アラートのライフサイクル フィールド (たとえば、状態、フィードバック、コメント.) を更新する
	• アラート通知 (フィルター処理されたクエリに基づく) およりアラート通知用のサンプル リスナーにサブスクライブする。

高度な UI を使用することにより、ほぼすべてのプロパティをクリックして、そのプロパティ値に対するフィルター処理されたクエリを生成し、エクスペリエンスの直感的な `ポイント アンド クリック` 調査を行るようになします。

	> メモ:V2.0 は、Microsoft Graph によって生成された SDK を使用して、アラートおよびサブスクリプション用の Microsoft Graph Security API を呼び出し、
	 セキュア スコアおよびセキュア スコア制御プロファイル用の REST 呼び出しを使用します (これらは引き続きベータ版です)。


## サンプル プロジェクト V3.0

サンプル プロジェクト V3.0 は、V2.0 UI のより高度なバージョンを作成します。これは、Angular サーバーを使用して、分析による調査目的のためのツールとして直接使用できます。 Microsoft Graph Security API を使用して以下を示します。

	• セキュリティ アクション (例:ブロック IP) を作成し、それらを取得する 
	• セキュア スコア、セキュリティ アクション、サブスクリプション用の UI に高度なフィルター処理を行うための機能を提供する
	    > V2.0 の機能 (下記参照) を含みます
	• クエリを構築して送信してアラートを取得する
	• アラートのライフサイクル フィールド (たとえば、状態、フィードバック、コメント.) を更新する
	• 顧客組織のセキュア スコアおよびセキュア スコア制御プロファイルを取得する
	• アラート通知 (フィルター処理されたクエリに基づく) およりアラート通知用のサンプル リスナーにサブスクライブする。


## 質問とコメント

このサンプルに関するフィードバックをお寄せください!
質問や提案につきましては、このリポジトリの「[問題](https://github.com/microsoftgraph/aspnet-connect-rest-sample/issues)」セクションで送信してください。

お客様からのフィードバックを重視しています。[スタック オーバーフロー](https://stackoverflow.com/questions/tagged/microsoftgraph)でご連絡ください。
ご質問には [MicrosoftGraph] のタグを付けてください。

## 投稿 ##

このサンプルに投稿する場合は、[CONTRIBUTING.md](CONTRIBUTING.md) を参照してください。

このプロジェクトでは、[Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/) が採用されています。
詳細については、「[Code of Conduct の FAQ](https://opensource.microsoft.com/codeofconduct/faq/)」を参照してください。また、その他の質問やコメントがあれば、[opencode@microsoft.com](mailto:opencode@microsoft.com) までお問い合わせください。

## その他のリソース

- [その他の Microsoft Graph Connect のサンプル](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph の概要](https://graph.microsoft.io)

## 著作権
Copyright (c) 2018 Microsoft.All rights reserved.



