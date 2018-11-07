# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>ASP.NET 4.6 用 Microsoft Graph Connect のサンプル

## <a name="table-of-contents"></a>目次

* [前提条件](#prerequisites)
* [アプリケーションの登録](#register-the-application)
* [サンプルのビルドと実行](#build-and-run-the-sample)
* [ノートのコード](#code-of-note)
* [質問とコメント](#questions-and-comments)
* [投稿](#contributing)
* [その他のリソース](#additional-resources)

このサンプルでは、Microsoft Graph API を使って ASP.NET 4.6 MVC Web アプリを Microsoft の職場または学校 (Azure Active Directory) アカウントまたは個人用 (Microsoft) アカウントに接続して、ユーザーのプロフィール画像の取得、OneDrive への画像のアップロード、電子メール (画像が添付され、共有リンクがテキストに含まれる) の送信を行う方法を示します。 [Microsoft Graph .NET クライアント ライブラリ](https://github.com/microsoftgraph/msgraph-sdk-dotnet)を使用して、Microsoft Graph が返すデータを操作します。 

また、サンプルでは認証に [Microsoft 認証ライブラリ (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) を使用します。MSAL SDK には、[Azure AD v2 0 エンドポイント](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview)を操作するための機能が用意されており、開発者は職場または学校 (Azure Active Directory) アカウント、および個人用 (Microsoft) アカウントの両方に対する認証を処理する 1 つのコード フローを記述することができます。

## <a name="important-note-about-the-msal-preview"></a>MSAL プレビューに関する重要な注意事項

このライブラリは、運用環境での使用に適しています。 このライブラリに対しては、現在の運用ライブラリと同じ運用レベルのサポートを提供します。 プレビュー中にこのライブラリの API、内部キャッシュの形式、および他のメカニズムを変更する場合があります。これは、バグの修正や機能強化の際に実行する必要があります。 これは、アプリケーションに影響を与える場合があります。 例えば、キャッシュ形式を変更すると、再度サインインが要求されるなどの影響をユーザーに与えます。 API を変更すると、コードの更新が要求される場合があります。 一般提供リリースが実施されると、プレビュー バージョンを使って作成されたアプリケーションは動作しなくなるため、6 か月以内に一般提供バージョンに更新することが求められます。

## <a name="prerequisites"></a>前提条件

このサンプルを実行するには次のものが必要です:  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * [Microsoft アカウント](https://www.outlook.com)または [Office 365 for Business アカウント](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account)のいずれか。Office 365 アプリのビルドを開始するために必要なリソースを含む、[Office 365 Developer サブスクリプション](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account)にサインアップできます。

## <a name="register-the-application"></a>アプリケーションの登録

1. 個人用アカウント、あるいは職場または学校アカウントのいずれかを使用して、[アプリ登録ポータル](https://apps.dev.microsoft.com/)にサインインします。

2. **[アプリの追加]** を選択します。

3. アプリの名前を入力して、**[アプリケーションの作成]** を選択します。 
    
   登録ページが表示され、アプリのプロパティが一覧表示されます。

4. アプリケーション ID をコピーします。これは、アプリの一意識別子です。 

5. **[アプリケーション シークレット]** で、**[新しいパスワードを生成する]** を選びます。**[新しいパスワードを生成する]** ダイアログからパスワードをコピーします。

   次のセクションで、アプリケーション ID とパスワードを使用してサンプル アプリを構成します。 

6. **[プラットフォーム]** で、**[プラットフォームの追加]** を選択します。

7. **[Web]** を選択します。

8. **[暗黙的フローを許可する]** のチェック ボックスが選択されていることを確認して、リダイレクト URI として *http://localhost:55065/* を入力します。 

   **[暗黙的フローを許可する]** オプションにより、ハイブリッド フローが有効になります。認証時に、アクセス トークンを取得するためにアプリが使用できるサインイン情報 (id_token) と成果物 (この場合は、認証コード) の両方をアプリで受信できるようになります。

9. **[保存]** を選択します。

## <a name="build-and-run-the-sample"></a>サンプルの構築と実行

1. ASP.NET 4.6 用 Microsoft Graph Connect のサンプルをダウンロードするか、クローンを作成します。

2. Visual Studio でサンプル ソリューションを開きます。

3. ルート ディレクトリの Web.config ファイルで、**ida:AppId** と **ida:AppSecret** のプレースホルダ―の値をアプリの登録時にコピーしたアプリケーションの ID とパスワードと置き換えます。

4. F5 キーを押して、サンプルを構築して実行します。これにより、NuGet パッケージの依存関係が復元され、アプリが開きます。

   >パッケージのインストール中にエラーが発生した場合は、ソリューションを保存したローカル パスが長すぎたり深すぎたりしていないかご確認ください。ドライブのルート近くにソリューションを移動すると問題が解決します。

5. 個人用あるいは職場または学校アカウントでサインインし、要求されたアクセス許可を付与します。

6. **[メール アドレスの取得]** ボタンを選択します。操作が完了すると、サインインしているユーザーのメール アドレスがページに表示されます。

7. 必要に応じて、受信者一覧とメールの件名を編集し、**[メールの送信]** ボタンを選択します。メールが送信されると、ボタンの下に成功メッセージが表示されます。

8. 次の手順：[ASP.NET 4.6 用 Microsoft Graph スニペットのサンプル](https://github.com/microsoftgraph/aspnet-snippets-sample)を参照して、Microsoft Graph の一般的な操作の例を確認します。

## <a name="code-of-note"></a>ノートのコード

> 注:ASP.NET MVC アプリで Microsoft Graph API を呼び出すためのコードを理解するには、「[ASP.NET 4.6 MVC アプリで Microsoft Graph を開始する](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc)」をご覧ください。

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs)。現在のユーザーを認証して、サンプルのトークン キャッシュを初期化します。

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs).ユーザーのトークン情報を保存します。これを独自のカスタム トークン キャッシュと置き換えることができます。詳細については、「[マルチテナント アプリケーションのアクセス トークンのキャッシュ](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/)」を参照してください。

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs)。ローカルの IAuthProvider インターフェイスを実装して、MSAL **AcquireTokenSilentAsync** メソッドを使用してアクセス トークンを取得します。これを独自の認証プロバイダーと置き換えることができます。 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs)。Microsoft Graph との対話に使用される [Microsoft Graph .NET クライアント ライブラリ](https://github.com/microsoftgraph/msgraph-sdk-dotnet)の **GraphServiceClient** を初期化します。

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs)。呼び出しを構築して Microsoft Graph サービスに送信し、その応答を処理するために **GraphServiceClient** を使用するメソッドが含まれています。
   - **GetMyEmailAddress** アクションは、**メール** プロパティまたは **userPrincipalName** プロパティから現在のユーザーのメール アドレスを取得します。
   - **SendMail** アクションは、現在のユーザーに代わってメールを送信します。

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml).サンプルの UI が含まれています。 

## <a name="questions-and-comments"></a>質問とコメント

このサンプルに関するフィードバックをお寄せください。質問や提案につきましては、このリポジトリの「[問題](https://github.com/microsoftgraph/aspnet-connect-sample/issues)」セクションで送信できます。

お客様からのフィードバックを重視しています。[スタック オーバーフロー](http://stackoverflow.com/questions/tagged/microsoftgraph)でご連絡いただけます。ご質問には [MicrosoftGraph] のタグを付けてください。

## <a name="contributing"></a>投稿 ##

このサンプルに投稿する場合は、[CONTRIBUTING.md](CONTRIBUTING.md) を参照してください。

このプロジェクトでは、[Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/) が採用されています。詳細については、「[規範に関する FAQ](https://opensource.microsoft.com/codeofconduct/faq/)」を参照してください。または、その他の質問やコメントがあれば、[opencode@microsoft.com](mailto:opencode@microsoft.com) までにお問い合わせください。

## <a name="additional-resources"></a>追加リソース

- [その他の Microsoft Graph Connect サンプル](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph の概要](http://graph.microsoft.io)
- [Office 開発者向けコード サンプル](http://dev.office.com/code-samples)
- [Office デベロッパー センター](http://dev.office.com/)

## <a name="copyright"></a>著作権
Copyright (c) 2016 Microsoft. All rights reserved.



