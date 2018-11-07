# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>Пример приложения, подключающегося с использованием Microsoft Graph, для ASP.NET 4.6

## <a name="table-of-contents"></a>Содержание

* [Необходимые компоненты](#prerequisites)
* [Регистрация приложения](#register-the-application)
* [Сборка и запуск примера](#build-and-run-the-sample)
* [Полезный код](#code-of-note)
* [Вопросы и комментарии](#questions-and-comments)
* [Участие](#contributing)
* [Дополнительные ресурсы](#additional-resources)

В этом примере показано, как подключить веб-приложение на базе ASP.NET 4.6 MVC к рабочей или учебной учетной записи Майкрософт (Azure Active Directory) либо личной учетной записи Майкрософт с помощью API Microsoft Graph, чтобы получить изображение профиля пользователя, передать это изображение в OneDrive и отправить электронное письмо с вложенной фотографией и ссылкой для общего доступа, указанной в тексте сообщения. Для работы с данными, возвращаемыми Microsoft Graph, используется [клиентская библиотека Microsoft Graph .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet). 

Кроме того, для проверки подлинности в этом примере используется [Microsoft Authentication Library (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/). В пакете SDK MSAL предусмотрены функции для работы с [конечной точкой Azure AD версии 2.0](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), которая позволяет разработчикам создать единый поток кода для проверки подлинности как рабочих или учебных (Azure Active Directory), так и личных учетных записей Майкрософт.

## <a name="important-note-about-the-msal-preview"></a>Важное примечание о предварительной версии MSAL

Эту библиотеку можно использовать в рабочей среде. Для этой библиотеки мы предоставляем тот же уровень поддержки, что и для текущих библиотек рабочей среды. Мы можем внести изменения в API, формат внутреннего кэша и другие функциональные элементы, касающиеся этой предварительной версии библиотеки, которые вам потребуется принять вместе с улучшениями или исправлениями. Это может повлиять на ваше приложение. Например, в результате изменения формата кэша пользователям может потребоваться опять выполнить вход. При изменении API может потребоваться обновить код. Когда мы предоставим общедоступный выпуск, вам потребуется выполнить обновление до общедоступной версии в течение шести месяцев, так как приложения, написанные с использованием предварительной версии библиотеки, могут больше не работать.

## <a name="prerequisites"></a>Необходимые условия

Для этого примера требуются следующие компоненты:  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * [Учетная запись Майкрософт](https://www.outlook.com) или [учетная запись Office 365 для бизнеса](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account). Вы можете подписаться на [план Office 365 для разработчиков](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account), который включает ресурсы, необходимые для создания приложений Office 365.

## <a name="register-the-application"></a>Регистрация приложения

1. Войдите на [портал регистрации приложений](https://apps.dev.microsoft.com/) с помощью личной, рабочей или учебной учетной записи.

2. Нажмите кнопку **Добавить приложение**.

3. Введите имя приложения и нажмите кнопку **Создать приложение**. 
    
   Откроется страница регистрации со свойствами приложения.

4. Скопируйте идентификатор приложения. Это уникальный идентификатор приложения. 

5. В разделе **Секреты приложения** нажмите кнопку **Создать новый пароль**. Скопируйте пароль из диалогового окна **Новый пароль создан**.

   Идентификатор приложения и пароль будут использованы для настройки примера приложения в следующем разделе. 

6. В разделе **Платформы** выберите элемент **Добавление платформы**.

7. Выберите пункт **Веб**.

8. Установите флажок **Разрешить неявный поток** и введите *http://localhost:55065/* в качестве URI перенаправления. 

   Параметр **Разрешить неявный поток** включает гибридный поток. Благодаря этому при проверке подлинности приложение может получить данные для входа (id_token) и артефакты (в данном случае — код авторизации), которые оно может использовать, чтобы получить маркер доступа.

9. Нажмите кнопку **Сохранить**.

## <a name="build-and-run-the-sample"></a>Сборка и запуск примера

1. Скачайте или клонируйте пример приложения, подключающегося с использованием Microsoft Graph, для ASP.NET 4.6.

2. Откройте пример решения в Visual Studio.

3. В корневом каталоге в файле Web.config замените заполнители **ida:AppId** и **ida:AppSecret** на идентификатор приложения и пароль, которые вы скопировали при регистрации приложения.

4. Нажмите клавишу F5 для сборки и запуска примера. При этом будут восстановлены зависимости пакета NuGet и открыто приложение.

   >Если при установке пакетов возникают ошибки, убедитесь, что локальный путь к решению не слишком длинный. Чтобы устранить эту проблему, переместите решение ближе к корню диска.

5. Войдите с помощью личной, рабочей или учебной учетной записи и предоставьте необходимые разрешения.

6. Нажмите кнопку **Получить адрес электронной почты**. После завершения операции на странице отобразится электронный адрес пользователя, выполнившего вход.

7. При необходимости измените список получателей и тему сообщения электронной почты, а затем нажмите кнопку **Отправить сообщение**. Под кнопкой отобразится сообщение об успешной отправке почты.

8. Дальнейшие действия: В статье [Пример фрагментов кода Microsoft Graph для ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-snippets-sample) ознакомьтесь с примерами наиболее распространенных операций Microsoft Graph.

## <a name="code-of-note"></a>Полезный код

> Примечание. Сведения о коде для вызова API Microsoft Graph в приложении ASP.NET MVC см. в статье [Начало работы с Microsoft Graph в приложении ASP.NET 4.6 MVC](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc).

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs). Выполняет проверку подлинности для текущего пользователя и инициализирует кэш маркеров примера.

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs). Хранит информацию о маркере пользователя. Вы можете заменить его на собственный кэш маркеров. Дополнительные сведения см. в статье [Кэширование маркеров доступа в мультитенантном приложении](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/).

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs). Реализует локальный интерфейс IAuthProvider и получает маркер доступа с помощью метода MSAL **AcquireTokenSilentAsync**. Вы можете заменить его на собственного поставщика проверки подлинности. 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs). Инициализирует класс **GraphServiceClient** из [клиентской библиотеки .NET Microsoft Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet), используемой для взаимодействия с Microsoft Graph.

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs). Содержит методы, использующие класс **GraphServiceClient** для создания и отправки вызовов в службу Microsoft Graph и обработки ответа.
   - Действие **GetMyEmailAddress** позволяет получить адрес электронной почты текущего пользователя из свойства **mail** или **userPrincipalName**.
   - Действие **SendMail** позволяет отправить сообщение электронной почты от имени текущего пользователя.

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml). Содержит пользовательский интерфейс образца. 

## <a name="questions-and-comments"></a>Вопросы и комментарии

Мы будем рады узнать ваше мнение об этом примере. Вы можете отправлять нам вопросы и предложения на вкладке [Issues](https://github.com/microsoftgraph/aspnet-connect-sample/issues) этого репозитория.

Ваш отзыв важен для нас. Для связи с нами используйте сайт [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph). Помечайте свои вопросы тегом [MicrosoftGraph].

## <a name="contributing"></a>Участие ##

Если вы хотите добавить код в этот пример, просмотрите статью [CONTRIBUTING.md](CONTRIBUTING.md).

Этот проект соответствует [правилам поведения Майкрософт, касающимся обращения с открытым кодом](https://opensource.microsoft.com/codeofconduct/). Читайте дополнительные сведения в [разделе вопросов и ответов по правилам поведения](https://opensource.microsoft.com/codeofconduct/faq/) или отправляйте новые вопросы и замечания по адресу [opencode@microsoft.com](mailto:opencode@microsoft.com).

## <a name="additional-resources"></a>Дополнительные ресурсы

- [Другие примеры Microsoft Graph Connect](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Общие сведения о Microsoft Graph](http://graph.microsoft.io)
- [Примеры кода приложений для Office](http://dev.office.com/code-samples)
- [Центр разработки для Office](http://dev.office.com/)

## <a name="copyright"></a>Авторское право
(c) Корпорация Майкрософт (Microsoft Corporation), 2016. Все права защищены.



