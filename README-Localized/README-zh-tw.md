# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>Microsoft Graph Connect 範例 (適用於 ASP.NET 4.6)

## <a name="table-of-contents"></a>目錄

* [必要條件](#prerequisites)
* [註冊應用程式](#register-the-application)
* [建置及執行範例](#build-and-run-the-sample)
* [附註的程式碼](#code-of-note)
* [問題和建議](#questions-and-comments)
* [參與](#contributing)
* [其他資源](#additional-resources)

這個範例會顯示如何使用 Microsoft Graph API 將 ASP.NET 4.6 MVC Web 應用程式連線至 Microsoft 工作或學校 (Azure Active Directory) 或個人 (Microsoft) 帳戶，用來擷取使用者的基本資料圖片、將圖片上傳至 OneDrive，並傳送含有相片作為附件且文字中包含共用連結的電子郵件。 它會使用 [Microsoft Graph.NET 用戶端程式庫](https://github.com/microsoftgraph/msgraph-sdk-dotnet)，使用 Microsoft Graph 所傳回的資料。 

此外，範例會使用 [Microsoft 驗證程式庫 (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) 進行驗證。MSAL SDK 提供功能以使用 [Azure AD v2.0 端點](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview)，可讓開發人員撰寫單一程式碼流程，控制工作或學校 (Azure Active Directory) 和個人 (Microsoft) 帳戶的驗證。

## <a name="important-note-about-the-msal-preview"></a>MSAL 預覽相關的重要事項

這個程式庫適合在實際執行環境中使用。 我們為我們目前的實際執行程式庫提供與此程式庫相同的實際執行層級支援。 在預覽期間，我們可能會變更此程式庫的 API、內部快取格式和其他機制，您將必須對此程式庫進行錯誤修復或增強功能。 這可能會影響您的應用程式。 舉例來說，變更快取格式可能會影響您的使用者，例如需要使用者重新登入。 API 變更可能需要更新您的程式碼。 當我們提供「一般可用性」版本時，將要求您在六個月內更新至「一般可用性」版本，因為使用程式庫預覽版本所撰寫的應用程式可能無法運作。

## <a name="prerequisites"></a>必要條件

此範例需要下列項目：  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * [Microsoft 帳戶](https://www.outlook.com)或[商務用 Office 365 帳戶](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account)。您可以註冊 [Office 365 開發人員訂用帳戶](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account)，其中包含開始建置 Office 365 應用程式所需的資源。

## <a name="register-the-application"></a>註冊應用程式

1. 使用您的個人或工作或學校帳戶登入[應用程式註冊入口網站](https://apps.dev.microsoft.com/)。

2. 選擇 [新增應用程式]****。

3. 為應用程式輸入名稱，然後選擇 [建立應用程式]****。 
    
   [註冊] 頁面隨即顯示，列出您的應用程式的屬性。

4. 複製應用程式 ID。這是您的應用程式的唯一識別碼。 

5. 在 [應用程式密碼]**** 底下，選擇 [產生新密碼]****。從 [產生的新密碼]**** 對話方塊中複製密碼。

   使用應用程式 ID 和密碼，在下一個區段中設定範例應用程式。 

6. 在 [平台]**** 底下，選擇 [新增平台]****。

7. 選擇 [Web]****。

8. 請確定已選取 [允許隱含的流程]**** 核取方塊，然後輸入 *http://localhost:55065/* 做為重新導向 URI。 

   [允許隱含的流程]**** 選項會啟用混合式流程。在驗證期間，這可讓應用程式收到登入資訊 (id_token) 和成品 (在這種情況下，是授權程式碼)，應用程式可以用來取得存取權杖。

9. 選擇 [儲存]****。

## <a name="build-and-run-the-sample"></a>建置及執行範例

1. 下載或複製 Microsoft Graph Connect 範例 (適用於 ASP.NET 4.6)。

2. 在 Visual Studio 中開啟範例解決方案。

3. 在根目錄的 Web.config 檔案中，將 **ida:AppId** 和 **ida:AppSecret** 預留位置值取代為您在應用程式註冊期間複製的應用程式 ID 與密碼。

4. 按 F5 以建置及執行範例。這樣會還原 NuGet 封裝相依性，並開啟應用程式。

   >如果您在安裝封裝時看到任何錯誤，請確定您放置解決方案的本機路徑不會太長/太深。將解決方案移靠近您的磁碟機根目錄可解決這個問題。

5. 登入您的個人或工作或學校帳戶，並授與要求的權限。

6. 選擇 [取得電子郵件地址]**** 按鈕。當作業完成時，登入使用者的電子郵件地址會顯示在頁面中。

7. 選擇性編輯收件者清單和電子郵件主旨，然後選擇 [傳送郵件]**** 按鈕。當郵件傳送時，成功的訊息會顯示在按鈕下方。

8. 後續步驟：查看 [Microsoft Graph 程式碼片段範例 (適用於 ASP.NET 4.6)](https://github.com/microsoftgraph/aspnet-snippets-sample)，以查看一般 Microsoft Graph 作業的範例。

## <a name="code-of-note"></a>程式碼附註

> 附註：若要了解在 ASP.NET MVC 應用程式中用於呼叫 Microsoft Graph API 的程式碼，請參閱[在 ASP.NET 4.6 MVC 應用程式中開始使用 Microsoft Graph](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc)。

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs).驗證目前使用者，並初始化範例的權杖快取。

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs).儲存使用者的權杖資訊。您可以將這個項目取代為您自己的自訂權杖快取。在[多租用戶應用程式中的快取存取權杖](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/)中深入了解。

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs).實作本機 IAuthProvider 介面，並取得存取權杖，方法是使用 MSAL **AcquireTokenSilentAsync** 方法。您可以將這個項目取代為您自己的驗證提供者。 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs).從 **Microsoft Graph.NET 用戶端程式庫**初始化 [GraphServiceClient](https://github.com/microsoftgraph/msgraph-sdk-dotnet) 用來與 Microsoft Graph 互動。

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs).包含方法，該方法使用 **GraphServiceClient** 以建置並傳送呼叫至 Microsoft Graph 服務，並且處理回應。
   - **GetMyEmailAddress** 動作會從 **mail** 或 **userPrincipalName** 屬性，取得目前使用者的電子郵件地址。
   - **SendMail** 動作會代表目前的使用者傳送電子郵件。

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml).包含範例的 UI。 

## <a name="questions-and-comments"></a>問題和建議

我們很樂於收到您對於此範例的意見反應。您可以在此儲存機制的[問題](https://github.com/microsoftgraph/aspnet-connect-sample/issues)區段中，將您的問題及建議傳送給我們。

我們很重視您的意見。請透過 [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph) 與我們連絡。以 [MicrosoftGraph] 標記您的問題。

## <a name="contributing"></a>參與 ##

如果您想要參與這個範例，請參閱 [CONTRIBUTING.md](CONTRIBUTING.md)。

此專案已採用 [Microsoft 開放原始碼執行](https://opensource.microsoft.com/codeofconduct/)。如需詳細資訊，請參閱[程式碼執行常見問題集](https://opensource.microsoft.com/codeofconduct/faq/)，如果有其他問題或意見，請連絡 [opencode@microsoft.com](mailto:opencode@microsoft.com)。

## <a name="additional-resources"></a>其他資源

- [其他 Microsoft Graph connect 範例](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph 概觀](http://graph.microsoft.io)
- [Office 開發人員程式碼範例](http://dev.office.com/code-samples)
- [Office 開發人員中心](http://dev.office.com/)

## <a name="copyright"></a>著作權
Copyright (c) 2016 Microsoft.著作權所有，並保留一切權利。



