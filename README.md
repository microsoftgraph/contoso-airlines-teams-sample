# Contoso Airlines sample for Microsoft Teams Graph APIs

This sample demonstrates using the Microsoft Graph APIs for Microsoft teams to automate team lifecycles for Contoso Airlines: every night, they create a new team for each flight they are flying the following day, and after the flight, they archive the team.

## Build and run

To run, you'll need to register your application.

1. Sign into the Azure [app registration portal](https://go.microsoft.com/fwlink/?linkid=2083908) using either your personal or work or school account.

2. Choose **New registration** near the top.

3. Enter a name for the app. Under **Supported account types**, select **Accounts in any organizational directory and personal Microsoft accounts (e.g. Skype, Xbox, Outlook.com)**.

4. For **Redirect URI (optional)**, choose **Web** and then enter *http://localhost:55065/*.

   The app overview page displays, listing the properties of your app.

5. Copy the value for **Application (client) ID**. This is the unique identifier for your app.

6. Select the **Certificated & secrets** page. Under **Client secrets**, choose **New client secret**.

7. Enter a label for the client secret and select an expiration period. Select **Add**.

8. Copy the value for your new client secret. This is the only time you will be able to see this value.

   You'll use the application ID and secret to configure the sample app in the next section.

9. Navigate to the **Authentication** page.

10. Under **Advanced settings**, find the **Implicit grant** section. Check the boxes for **Access tokens** and **ID tokens** to enable implicit flow.

    These options enable the hybrid flow. During authentication, this enables the app to receive both sign-in info (the id_token) and artifacts (in this case, an authorization code) that the app can use to obtain an access token.

11. Choose **Save** at the top.

12. Create a file named Web.config.secrets (put it next to Web.config), and add in your application ID and client (app) secret:

```xml
<?xml version="1.0" encoding="utf-8"?>
  <appSettings >
    <add key="ida:AppId" value="xxxxx"/>
    <add key="ida:AppSecret" value="xxxxx"/>
  </appSettings>
```

## Code of note

> Note: To understand the code for calling the Microsoft Graph API in an ASP.NET MVC app, see [Get started with Microsoft Graph in an ASP.NET 4.6 MVC app](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc).

- [GraphService.cs](/project/Models/GraphService.cs). This is where the Graph API calls are.

- [GraphResource.cs](/project//Models/GraphResource.cs). Strongly typed wrappers for various Graph resources.

- [HomeController.cs](/project/Controllers/HomeController.cs). Contains methods that drive the UI, as well as some authentication logic.

- [Graph.cshtml](/project/Views/Home/Graph.cshtml). Contains the sample's UI.

A lot of this code came from the [Graph Quickstart for ASP.NET MVC](https://developer.microsoft.com/en-us/graph/quick-start?platform=option-dotnet).

## Questions and comments

We'd love to get your feedback about this sample. You can send us your questions and suggestions in the [Issues](https://github.com/microsoftgraph/aspnet-connect-sample/issues) section of this repository.

Your feedback is important to us. Connect with us on [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph). Tag your questions with [MicrosoftGraph].

## Contributing

If you'd like to contribute to this sample, see [CONTRIBUTING.md](CONTRIBUTING.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Additional resources

- [C# Teams Sample app for Graph](https://github.com/microsoftgraph/csharp-teams-sample-graph)
- [Other Microsoft Graph Connect samples](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph overview](http://graph.microsoft.io)
- [Office developer code samples](http://dev.office.com/code-samples)
- [Office dev center](http://dev.office.com/)

## Copyright

Copyright (c) 2018 Microsoft. All rights reserved.
