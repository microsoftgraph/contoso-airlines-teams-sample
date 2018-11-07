# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>Microsoft Graph Connect-Beispiel für ASP.NET 4.6

## <a name="table-of-contents"></a>Inhalt

* [Voraussetzungen](#prerequisites)
* [Registrieren der App](#register-the-application)
* [Erstellen und Ausführen des Beispiels](#build-and-run-the-sample)
* [Relevanter Code](#code-of-note)
* [Fragen und Kommentare](#questions-and-comments)
* [Mitwirkung](#contributing)
* [Zusätzliche Ressourcen](#additional-resources)

Dieses Beispiel zeigt, wie Sie eine ASP.NET 4.6 MVC-Web-App mit einem Microsoft-Geschäfts-, Schul- oder Unikonto (Azure Active Directory) oder einem persönlichen (Microsoft) Konto anhand der Microsoft Graph API verbinden, um das Profilbild eines Benutzers abzurufen, das Bild in OneDrive hochzuladen und eine E-Mail zu senden, das das Foto als Anhang und den Freigabelink als Text enthält. Es verwendet die [Microsoft Graph .NET-Clientbibliothek](https://github.com/microsoftgraph/msgraph-sdk-dotnet), um mit Daten zu arbeiten, die von Microsoft Graph zurückgegeben werden. 

Das Beispiel verwendet außerdem die [Microsoft-Authentifizierungsbibliothek (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) für die Authentifizierung. Das MSAL-SDK bietet Features für die Arbeit mit dem [Azure AD v2.0-Endpunkt](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), der es Entwicklern ermöglicht, einen einzelnen Codefluss zu schreiben, der die Authentifizierung sowohl für Geschäfts- oder Schulkonten (Azure Active Directory) als auch für persönliche Konten (Microsoft) verarbeitet. 

## <a name="important-note-about-the-msal-preview"></a>Wichtiger Hinweis zur MSAL-Vorschau

Diese Bibliothek eignet sich für die Verwendung in einer Produktionsumgebung. Wir bieten für diese Bibliothek den gleichen Support auf Produktionsebene wie für alle anderen aktuellen Produktionsbibliotheken. Während der Vorschau nehmen wir möglicherweise Änderungen an der API, dem internen Cacheformat und anderen Mechanismen dieser Bibliothek vor, die Sie zusammen mit Fehlerbehebungen oder Funktionsverbesserungen übernehmen müssen. Dies kann sich auf Ihre Anwendung auswirken. So kann sich eine Änderung des Cacheformats beispielsweise auf die Benutzer auswirken, indem sie sich z. B. erneut anmelden müssen. Eine Änderung der API kann dazu führen, dass Sie den Code aktualisieren müssen. Wenn wir das allgemein verfügbare Release bereitstellen, müssen Sie innerhalb von sechs Monaten auf die allgemein verfügbare Version aktualisieren, da Anwendungen, die mit einer Vorschauversion der Bibliothek erstellt wurden, möglicherweise nicht mehr funktionieren.

## <a name="prerequisites"></a>Voraussetzungen

Für dieses Beispiel ist Folgendes erforderlich:  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * Entweder ein [Microsoft-Konto](https://www.outlook.com) oder ein [Office 365 for Business-Konto](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account). Sie können sich für [ein Office 365-Entwicklerabonnement](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account) registrieren. Dieses umfasst die Ressourcen, die Sie zum Erstellen von Office 365-Apps benötigen.

## <a name="register-the-application"></a>Registrieren der App

1. Melden Sie sich beim [App-Registrierungsportal](https://apps.dev.microsoft.com/) entweder mit Ihrem persönlichen oder geschäftlichen Konto oder mit Ihrem Schulkonto an.

2. Klicken Sie auf **App hinzufügen**.

3. Geben Sie einen Namen für die App ein, und wählen Sie **Anwendung erstellen** aus. 
    
   Die Registrierungsseite wird angezeigt wird, und die Eigenschaften der App werden aufgeführt.

4. Kopieren Sie die Anwendungs-ID: Dies ist der eindeutige Bezeichner für Ihre App. 

5. Wählen Sie unter **Anwendungsgeheimnisse** die Option **Neues Kennwort generieren** aus. Kopieren Sie das Kennwort aus dem Dialogfeld **Neues Kennwort wurde generiert**.

   Verwenden Sie die Anwendungs-ID und das Kennwort zur Konfiguration der Beispiel-App im nächsten Abschnitt. 

6. Wählen Sie unter **Plattformen** die Option **Plattform hinzufügen** aus.

7. Wählen Sie **Web** aus.

8. Stellen Sie sicher, dass das Kontrollkästchen **Impliziten Fluss zulassen** aktiviert ist, und geben Sie *http://localhost:55065/* als Umleitungs-URI ein. 

   Die Option **Impliziten Fluss zulassen** ermöglicht den Hybridfluss. Während der Authentifizierung ermöglicht dies der App, sowohl Anmeldeinformationen (das id_token) als auch Artefakte (in diesem Fall ein Autorisierungscode) abzurufen, den die App zum Abrufen eines Zugriffstokens verwenden kann.

9. Wählen Sie **Speichern** aus.

## <a name="build-and-run-the-sample"></a>Erstellen und Ausführen des Beispiels

1. Laden Sie das Microsoft Graph Connect-Beispiel für ASP.NET 4.6 herunter.

2. Öffnen Sie die Projektmappe in Visual Studio.

3. Ersetzen Sie in der Datei „Web.config“ im Stammverzeichnis die Platzhalterwerte **ida: AppId** und **ida: AppSecret** durch die Anwendungs-ID und das Kennwort, die bzw. das Sie während der App-Registrierung kopiert haben.

4. Drücken Sie zum Erstellen und Ausführen des Beispiels F5. Dadurch werden NuGet-Paketabhängigkeiten wiederhergestellt, und die App wird geöffnet.

   >Wenn beim Installieren der Pakete Fehler angezeigt werden, müssen Sie sicherstellen, dass der lokale Pfad, unter dem Sie die Projektmappe abgelegt haben, weder zu lang noch zu tief ist. Dieses Problem lässt sich beheben, indem Sie den Pfad auf Ihrem Laufwerk verkürzen.

5. Melden Sie sich mit Ihrem persönlichen Konto oder mit Ihrem Geschäfts- oder Schulkonto an, und gewähren Sie die erforderlichen Berechtigungen.

6. Klicken Sie auf die Schaltfläche **E-Mail-Adresse abrufen**. Wenn der Vorgang abgeschlossen ist, wird die E-Mail-Adresse des angemeldeten Benutzers auf der Seite angezeigt.

7. Optional können Sie die Empfängerliste und den Betreff der E-Mail bearbeiten. Klicken Sie dann auf die Schaltfläche **E-Mail senden**. Nachdem die E-Mail gesendet wurde, wird unter der Schaltfläche eine Erfolgsmeldung angezeigt.

8. Nächste Schritte: Schauen Sie sich das [Microsoft Graph-Codeausschnittbeispiel für ASP.NET 4.6 ](https://github.com/microsoftgraph/aspnet-snippets-sample) an, um Beispiele für allgemeine Microsoft Graph-Vorgänge anzuzeigen.

## <a name="code-of-note"></a>Relevanter Code

> Hinweis: Informationen für das Verständnis des Codes für den Aufruf der Microsoft Graph-API in einer ASP.NET MVC-App finden Sie im Thema über das [Erste Schritte mit Microsoft Graph in einer ASP.NET 4.6 MVC-App](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc).

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs). Authentifiziert den aktuellen Benutzer und initialisiert den Tokencache des Beispiels.

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs). Speichert die Tokeninformationen des Benutzers. Sie können dies durch Ihren eigenen benutzerdefinierten Tokencache ersetzen. Weitere Informationen finden Sie unter [Zwischenspeichern von Zugriffstoken in einer Anwendung für mehrere Mandanten](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/).

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs). Implementiert die lokale IAuthProvider-Schnittstelle und ruft ein Zugriffstoken mithilfe der MSAL-Methode **AcquireTokenSilentAsync** ab. Sie können dies durch Ihren eigenen Authentifizierungsanbieter ersetzen. 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs). Initialisiert den **GraphServiceClient** aus der [Microsoft Graph .NET-Clientbibliothek](https://github.com/microsoftgraph/msgraph-sdk-dotnet), die für die Interaktion mit dem Microsoft Graph verwendet wird.

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs). Enthält Methoden, die den **GraphServiceClient** zum Erstellen und Senden von Aufrufen des Microsoft Graph-Diensts und zum Verarbeiten der Antwort verwendet.
   - Die **GetMyEmailAddress**-Aktion ruft die E-Mail-Adresse des aktuellen Benutzers aus der **Mail**- oder der **userPrincipalName**-Eigenschaft ab.
   - Die **SendMail**-Aktion sendet eine E-Mail im Auftrag des aktuellen Benutzers.

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml). Enthält die Benutzeroberfläche des Beispiels. 

## <a name="questions-and-comments"></a>Fragen und Kommentare

Wir schätzen Ihr Feedback hinsichtlich dieses Beispiels. Sie können uns Ihre Fragen und Vorschläge über den Abschnitt [Probleme](https://github.com/microsoftgraph/aspnet-connect-sample/issues) dieses Repositorys senden.

Ihr Feedback ist uns wichtig. Nehmen Sie unter [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph) Kontakt mit uns auf. Taggen Sie Ihre Fragen mit [MicrosoftGraph].

## <a name="contributing"></a>Mitwirkung ##

Wenn Sie einen Beitrag zu diesem Beispiel leisten möchten, finden Sie unter [CONTRIBUTING.md](CONTRIBUTING.md) weitere Informationen.

In diesem Projekt wurden die [Microsoft Open Source-Verhaltensregeln](https://opensource.microsoft.com/codeofconduct/) übernommen. Weitere Informationen finden Sie unter [Häufig gestellte Fragen zu Verhaltensregeln](https://opensource.microsoft.com/codeofconduct/faq/), oder richten Sie Ihre Fragen oder Kommentare an [opencode@microsoft.com](mailto:opencode@microsoft.com).

## <a name="additional-resources"></a>Zusätzliche Ressourcen

- [Weitere Microsoft Graph Connect-Beispiele](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph-Übersicht](http://graph.microsoft.io)
- [Office-Entwicklercodebeispiele](http://dev.office.com/code-samples)
- [Office Dev Center](http://dev.office.com/)

## <a name="copyright"></a>Copyright
Copyright (c) 2016 Microsoft. Alle Rechte vorbehalten.



