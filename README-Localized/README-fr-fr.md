# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>Exemple de connexion de Microsoft Graph pour ASP.NET 4.6

## <a name="table-of-contents"></a>Sommaire

* [Conditions préalables](#prerequisites)
* [Inscription de l’application](#register-the-application)
* [Création et exécution de l’exemple](#build-and-run-the-sample)
* [Code de note](#code-of-note)
* [Questions et commentaires](#questions-and-comments)
* [Contribution](#contributing)
* [Ressources supplémentaires](#additional-resources)

Cet exemple montre comment connecter une application web ASP.NET 4.6 MVC à un compte professionnel ou scolaire (Azure Active Directory), ou personnel (Microsoft) à l’aide de l’API Microsoft Graph pour récupérer l’image de profil d’un utilisateur, télécharger l’image vers OneDrive, et envoyer un courrier électronique contenant la photo en tant que pièce jointe et le lien de partage dans son texte. Il utilise la [bibliothèque Microsoft Graph .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) pour exploiter les données renvoyées par Microsoft Graph. 

En outre, l’exemple utilise la [bibliothèque d’authentification Microsoft (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) pour l’authentification. Le kit de développement logiciel (SDK) MSAL offre des fonctionnalités permettant d’utiliser le [point de terminaison Azure AD v2.0](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), qui permet aux développeurs d’écrire un flux de code unique qui gère l’authentification des comptes professionnels ou scolaires (Azure Active Directory) et personnels (Microsoft).

## <a name="important-note-about-the-msal-preview"></a>Remarque importante à propos de la version d’essai MSAL

La bibliothèque peut être utilisée dans un environnement de production. Nous fournissons la même prise en charge du niveau de production pour cette bibliothèque que pour nos bibliothèques de production actuelles. Lors de la version d’essai, nous pouvons apporter des modifications à l’API, au format de cache interne et à d’autres mécanismes de cette bibliothèque que vous devrez prendre en compte avec les correctifs de bogues ou les améliorations de fonctionnalités. Cela peut avoir un impact sur votre application. Par exemple, une modification du format de cache peut avoir un impact sur vos utilisateurs. Par exemple, il peut leur être demandé de se connecter à nouveau. Une modification de l’API peut vous obliger à mettre à jour votre code. Lorsque nous fournissons la version de disponibilité générale, vous devez effectuer une mise à jour vers la version de disponibilité générale dans un délai de six mois, car les applications écrites à l’aide de la version d’évaluation de la bibliothèque ne fonctionneront plus.

## <a name="prerequisites"></a>Conditions préalables

Cet exemple nécessite les éléments suivants :  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * Soit un [compte Microsoft](https://www.outlook.com), soit un [compte Office 365 pour entreprise](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account). Vous pouvez vous inscrire à [Office 365 Developer](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account) pour accéder aux ressources dont vous avez besoin afin de commencer à créer des applications Office 365.

## <a name="register-the-application"></a>Inscription de l’application

1. Connectez-vous au [portail d’inscription des applications](https://apps.dev.microsoft.com/) en utilisant votre compte personnel, professionnel ou scolaire.

2. Choisissez **Ajouter une application**.

3. Entrez un nom pour l’application, puis choisissez **Créer une application**. 
    
   La page d’inscription s’affiche, répertoriant les propriétés de votre application.

4. Copiez l’ID de l’application. Il s’agit de l’identificateur unique de votre application. 

5. Sous **Secrets de l'application**, choisissez **Générer un nouveau mot de passe**. Copiez le mot de passe à partir de la boîte de dialogue **Nouveau mot de passe créé**.

   Vous utiliserez l’ID de l’application et le mot de passe pour configurer l’exemple d’application dans la section suivante. 

6. Sous **Plateformes**, choisissez **Ajouter une plateforme**.

7. Choisissez **Web**.

8. Assurez-vous que la case **Autoriser le flux implicite** est cochée, puis entrez *https://localhost:55065/* comme URI de redirection. 

   L’option **Autoriser le flux implicite** active le flux hybride. Lors de l’authentification, cela permet à l’application de recevoir les informations de connexion (id_token) et les artefacts (dans ce cas, un code d’autorisation) qui servent à obtenir un jeton d’accès.

9. Cliquez sur **Enregistrer**.

## <a name="build-and-run-the-sample"></a>Création et exécution de l’exemple

1. Téléchargez ou clonez l’exemple de connexion Microsoft Graph pour ASP.NET 4.6.

2. Ouvrez l’exemple de solution dans Visual Studio.

3. Dans le fichier Web.config dans le répertoire racine, remplacez les valeurs d’espace réservé **ida:AppId** et **ida:AppSecret** par l’ID de l’application et le mot de passe que vous avez copiés lors de l’inscription de l’application.

4. Appuyez sur F5 pour créer et exécuter l’exemple. Cela entraîne la restauration des dépendances du package NuGet et l’ouverture de l’application.

   >Si vous constatez des erreurs pendant l’installation des packages, vérifiez que le chemin d’accès local où vous avez sauvegardé la solution n’est pas trop long/profond. Pour résoudre ce problème, il vous suffit de déplacer la solution dans un dossier plus près du répertoire racine de votre lecteur.

5. Connectez-vous à votre compte personnel, professionnel ou scolaire, et accordez les autorisations demandées.

6. Choisissez le bouton **Obtenir l’adresse de messagerie**. Une fois l’opération terminée, l’adresse de messagerie de l’utilisateur connecté s’affiche dans la page.

7. Vous pouvez également modifier la liste des destinataires et l’objet de l’e-mail, puis cliquer sur le bouton **Envoyer un message électronique**. Lorsque le message est envoyé, un message de réussite s’affiche sous le bouton.

8. Étapes suivantes : Consultez l’[exemple d’extraits de code Microsoft Graph pour ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-snippets-sample) pour voir des exemples d’opérations courantes de Microsoft Graph.

## <a name="code-of-note"></a>Code de note

> Remarque : pour comprendre le code permettant d’appeler l’API Microsoft Graph dans une application ASP.NET MVC, consultez la rubrique relative à la [prise en main de Microsoft Graph dans une application ASP.NET 4.6 MVC](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc).

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs). Authentifie l’utilisateur actuel et initialise le cache de jetons de l’exemple.

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs). Stocke les informations du jeton de l’utilisateur. Vous pouvez le remplacer par votre propre cache de jetons personnalisé. Pour plus d’informations, voir [Mise en cache des jetons d’accès dans une application mutualisée](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/).

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs). Implémente l’interface IAuthProvider locale et obtient un jeton d’accès à l’aide de la méthode MSAL **AcquireTokenSilentAsync**. Vous pouvez utiliser, à la place, votre propre fournisseur d’authentification. 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs). Initialise **GraphServiceClient** à partir de la [bibliothèque client Microsoft Graph .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) qui sert à interagir avec Microsoft Graph.

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs). Contient des méthodes qui utilisent **GraphServiceClient** pour créer et envoyer les appels au service Microsoft Graph et traiter la réponse.
   - L’action **GetMyEmailAddress** permet d’obtenir l’adresse de messagerie de l’utilisateur actuel à partir de la propriété **mail** ou **userPrincipalName**.
   - L’action **SendMail** envoie un message électronique au nom de l’utilisateur actuel.

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml). Contient l’interface utilisateur de l’exemple. 

## <a name="questions-and-comments"></a>Questions et commentaires

Nous serions ravis de connaître votre opinion sur cet exemple. Vous pouvez nous faire part de vos questions et suggestions dans la rubrique [Problèmes](https://github.com/microsoftgraph/aspnet-connect-sample/issues) de ce référentiel.

Votre avis compte beaucoup pour nous. Communiquez avec nous sur [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph). Posez vos questions avec la balise [MicrosoftGraph].

## <a name="contributing"></a>Contribution ##

Si vous souhaitez contribuer à cet exemple, voir [CONTRIBUTING.MD](CONTRIBUTING.md).

Ce projet a adopté le [code de conduite Microsoft Open Source](https://opensource.microsoft.com/codeofconduct/). Pour plus d’informations, reportez-vous à la [FAQ relative au code de conduite](https://opensource.microsoft.com/codeofconduct/faq/) ou contactez [opencode@microsoft.com](mailto:opencode@microsoft.com) pour toute question ou tout commentaire.

## <a name="additional-resources"></a>Ressources supplémentaires

- [Autres exemples de connexion avec Microsoft Graph](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Présentation de Microsoft Graph](http://graph.microsoft.io)
- [Exemples de code du développeur Office](http://dev.office.com/code-samples)
- [Centre de développement Office](http://dev.office.com/)

## <a name="copyright"></a>Copyright
Copyright (c) 2016 Microsoft. Tous droits réservés.



