# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>ASP.NET 4.6 的 Microsoft Graph Connect 示例

## <a name="table-of-contents"></a>目录

* [先决条件](#prerequisites)
* [注册应用程序](#register-the-application)
* [生成和运行示例](#build-and-run-the-sample)
* [注释代码](#code-of-note)
* [问题和意见](#questions-and-comments)
* [参与](#contributing)
* [其他资源](#additional-resources)

此示例展示了如何使用 Microsoft Graph API 将 ASP.NET 4.6 MVC Web 应用连接到 Microsoft 工作或学校帐户 (Azure Active Directory) 或个人 (Microsoft) 帐户，从而检索用户的个人资料照片，将此照片上传到 OneDrive，并发送将此照片作为附件且文本中包含共享链接的电子邮件。 它使用 [Microsoft Graph .NET 客户端库](https://github.com/microsoftgraph/msgraph-sdk-dotnet)来处理 Microsoft Graph 返回的数据。 

此外，此示例使用 [Microsoft 身份验证库 (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) 进行身份验证。MSAL SDK 提供可使用 [Azure AD v2.0 终结点](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview)的功能，借助该终结点，开发人员可以编写单个代码流来处理对工作或学校 (Azure Active Directory) 帐户或个人 (Microsoft) 帐户的身份验证。

## <a name="important-note-about-the-msal-preview"></a>有关 MSAL 预览版的重要说明

此库适用于生产环境。 我们为此库提供的生产级支持与为当前生产库提供的支持相同。 在预览期间，我们可能会更改 API、内部缓存格式和此库的其他机制，必须接受这些更改以及 bug 修复或功能改进。 这可能会影响应用。 例如，缓存格式更改可能会对用户造成影响，如要求用户重新登录。 API 更改可能会要求更新代码。 在我们提供通用版后，必须在 6 个月内更新到通用版，因为使用预览版库编写的应用可能不再可用。

## <a name="prerequisites"></a>先决条件

此示例需要以下各项：  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * [Microsoft 帐户](https://www.outlook.com)或 [Office 365 商业版帐户](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account)。可以注册 [Office 365 开发人员订阅](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account)，其中包含开始构建 Office 365 应用所需的资源。

## <a name="register-the-application"></a>注册应用

1. 使用个人或工作或学校帐户登录到 [应用注册门户](https://apps.dev.microsoft.com/)。

2. 选择“**添加应用**”。

3. 输入应用的名称，并选择“**创建应用程序**”。 
    
   将显示注册页，其中列出应用的属性。

4. 复制应用程序 ID。这是应用的唯一标识符。 

5. 在“**应用程序密码**”下，选择“**生成新密码**”。从“**生成的新密码**”对话框复制密码。

   使用应用程序 ID 和密码在下一部分中配置示例应用。 

6. 在“**平台**”下，选择“**添加平台**”。

7. 选择“**Web**”。

8. 请务必选中“允许隐式流”****复选框，然后输入“http://localhost:55065/”**作为重定向 URI。 

   “**允许隐式流**”选项启用混合流。在身份验证过程中，这可使应用同时接收登录信息 (id_token) 以及应用可用来获取访问令牌的项目（在这种情况下，项目为授权代码）。

9. 选择“**保存**”。

## <a name="build-and-run-the-sample"></a>生成和运行示例

1. 下载或克隆适用于 ASP.NET 4.6 的 Microsoft Graph Connect 示例。

2. 在 Visual Studio 中打开示例解决方案。

3. 在根目录的 Web.config 文件中，使用你在应用注册过程中复制的应用程序 ID 和密码来替换 **ida:AppId** 和 **ida:AppSecret** 占位符值。

4. 按 F5 生成和运行此示例。这将还原 NuGet 包依赖项，并打开应用。

   >如果在安装包时出现任何错误，请确保你放置该解决方案的本地路径并未太长/太深。将解决方案移动到更接近驱动器根目录的位置可以解决此问题。

5. 使用个人帐户/工作或学校帐户登录，并授予所请求的权限。

6. 选择“**获取电子邮件地址**”按钮。完成此操作后，网页上会显示登录用户的电子邮件地址。

7. 还可以编辑收件人列表和电子邮件主题，然后选择“**发送电子邮件**”按钮。在邮件发送后，按钮下方将显示成功消息。

8. 后续步骤：签出[适用于 ASP.NET 4.6 的 Microsoft Graph 代码段示例](https://github.com/microsoftgraph/aspnet-snippets-sample)查看 Microsoft Graph 的常见操作示例。

## <a name="code-of-note"></a>要注意的代码

> 注意：要了解在 ASP.NET 4.6 MVC 应用中调用 Microsoft Graph API 的代码，请参阅 [ASP.NET MVC 应用中的 Microsoft Graph 入门](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc)。

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs)。对当前用户进行身份验证，并初始化此示例的令牌缓存。

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs).存储用户的令牌信息。可以使用你自己的自定义令牌缓存来替换此信息。从[在多租户应用程序中缓存访问令牌](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/)了解详细信息。

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs)。实现本地 IAuthProvider 接口，并通过使用 MSAL **AcquireTokenSilentAsync** 方法获取一个访问令牌。可以使用你自己的身份验证提供程序来替换此方法。 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs)。初始化来自用于与 Microsoft Graph 交互的 [Microsoft Graph .NET 客户端库](https://github.com/microsoftgraph/msgraph-sdk-dotnet)中的 **GraphServiceClient**。

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs)。包含使用 **GraphServiceClient** 生成并发送到 Microsoft Graph 服务的调用并处理响应的方法。
   - **GetMyEmailAddress** 操作从 **mail** 或 **userPrincipalName** 属性获取当前用户的电子邮件地址。
   - **SendMail** 操作将代表当前用户发送一封电子邮件。

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml).包含该示例的 UI。 

## <a name="questions-and-comments"></a>问题和意见

我们乐意倾听你有关此示例的反馈。你可以在该存储库中的[问题](https://github.com/microsoftgraph/aspnet-connect-sample/issues) 部分将问题和建议发送给我们。

我们非常重视你的反馈意见。请在 [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph) 上与我们联系。使用 [MicrosoftGraph] 标记出你的问题。

## <a name="contributing"></a>参与 ##

如果想要参与本示例，请参阅 [CONTRIBUTING.md](CONTRIBUTING.md)。

此项目采用 [Microsoft 开源行为准则](https://opensource.microsoft.com/codeofconduct/)。有关详细信息，请参阅 [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)（行为准则常见问题解答），有任何其他问题或意见，也可联系 [opencode@microsoft.com](mailto:opencode@microsoft.com)。

## <a name="additional-resources"></a>其他资源

- [其他 Microsoft Graph Connect 示例](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph 概述](http://graph.microsoft.io)
- [Office 开发人员代码示例](http://dev.office.com/code-samples)
- [Office 开发人员中心](http://dev.office.com/)

## <a name="copyright"></a>版权
版权所有 (c) 2016 Microsoft。保留所有权利。



