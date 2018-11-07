# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>Ejemplo Connect de Microsoft Graph para ASP.NET 4.6

## <a name="table-of-contents"></a>Tabla de contenido

* [Requisitos previos](#prerequisites)
* [Registrar la aplicación](#register-the-application)
* [Compilar y ejecutar el ejemplo](#build-and-run-the-sample)
* [Código de nota](#code-of-note)
* [Preguntas y comentarios](#questions-and-comments)
* [Colaboradores](#contributing)
* [Recursos adicionales](#additional-resources)

Este ejemplo muestra cómo conectar una aplicación web de ASP.NET 4.6 MVC a una cuenta profesional o educativa de Microsoft (Azure Active Directory) o a una cuenta personal (Microsoft) usando la API de Microsoft Graph para recuperar la imagen del perfil de un usuario, cargar la imagen en OneDrive y enviar un correo electrónico que contiene la foto como un archivo adjunto y el vínculo para compartir en su texto. Usa la [biblioteca del cliente .NET de Microsoft Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet) para trabajar con los datos devueltos por Microsoft Graph. 

Además, el ejemplo usa la [biblioteca de autenticación de Microsoft (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) para la autenticación. El SDK de MSAL ofrece características para trabajar con el [punto de conexión v2.0 de Azure AD](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), lo que permite a los desarrolladores escribir un flujo de código único que controla la autenticación para las cuentas profesionales, educativas (Azure Active Directory) o las cuentas personales (Microsoft).

## <a name="important-note-about-the-msal-preview"></a>Nota importante acerca de la vista previa MSAL

Esta biblioteca es apta para utilizarla en un entorno de producción. Ofrecemos la misma compatibilidad de nivel de producción de esta biblioteca que la de las bibliotecas de producción actual. Durante la vista previa podemos realizar cambios en la API, el formato de caché interna y otros mecanismos de esta biblioteca, que deberá tomar junto con correcciones o mejoras. Esto puede afectar a la aplicación. Por ejemplo, un cambio en el formato de caché puede afectar a los usuarios, como que se les pida que vuelvan a iniciar sesión. Un cambio de API puede requerir que actualice su código. Cuando ofrecemos la versión de disponibilidad General, deberá actualizar a la versión de disponibilidad General dentro de seis meses, ya que las aplicaciones escritas mediante una versión de vista previa de biblioteca puede que ya no funcionen.

## <a name="prerequisites"></a>Requisitos previos

Este ejemplo necesita lo siguiente:  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * Ya sea una [cuenta de Microsoft](https://www.outlook.com) u [Office 365 para una cuenta empresarial](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account) Puede registrarse para [una suscripción de Office 365 Developer](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account), que incluye los recursos que necesita para comenzar a crear aplicaciones de Office 365.

## <a name="register-the-application"></a>Registrar la aplicación

1. Inicie sesión en el [Portal de registro de la aplicación](https://apps.dev.microsoft.com/) mediante su cuenta personal, profesional o educativa.

2. Seleccione **Agregar una aplicación**.

3. Escriba un nombre para la aplicación y seleccione **Crear aplicación**. 
    
   Se muestra la página de registro, indicando las propiedades de la aplicación.

4. Copie el Id. de aplicación. Se trata del identificador único para su aplicación. 

5. En **Secretos de aplicación**, seleccione **Generar nueva contraseña**. Copie la contraseña del cuadro de diálogo de **Nueva contraseña generada**.

   Usará el identificador de la aplicación y la contraseña para configurar la aplicación de ejemplo en la sección siguiente. 

6. En **Plataformas**, seleccione **Agregar plataforma**.

7. Seleccione **Web**.

8. Asegúrese de que la casilla **Permitir flujo implícito** esté seleccionada y escriba *http://localhost:55065/* como URI de redireccionamiento. 

   La opción **Permitir flujo implícito** permite el flujo híbrido. Durante la autenticación, esto permite que la aplicación reciba la información de inicio de sesión (id_token) y artefactos (en este caso, un código de autorización) que la aplicación puede usar para obtener un token de acceso.

9. Elija **Guardar**.

## <a name="build-and-run-the-sample"></a>Compilar y ejecutar el ejemplo

1. Descargue o clone el ejemplo de Microsoft Graph Connect para ASP.NET 4.6

2. Abra la solución del ejemplo en Visual Studio.

3. En el archivo Web.config en el directorio raíz, reemplace los valores de los marcadores de posición **ida:AppId** e **ida:AppSecret** por los valores que ha copiado durante el registro de la aplicación.

4. Pulse F5 para compilar y ejecutar el ejemplo. Esto restaurará las dependencias de paquetes de NuGet y abrirá la aplicación.

   >Si observa algún error durante la instalación de los paquetes, asegúrese de que la ruta de acceso local donde colocó la solución no es demasiado larga o profunda. Para resolver este problema, mueva la solución más cerca de la raíz de la unidad.

5. Inicie sesión con su cuenta personal, profesional o educativa y conceda los permisos solicitados.

6. Seleccione el botón **Obtener la dirección de correo electrónico**. Cuando finaliza la operación, la dirección de correo electrónico del usuario con sesión iniciada se muestra en la página.

7. De forma opcional, modifique la lista de destinatarios y el asunto del correo electrónico y, después, seleccione el botón **Enviar correo electrónico**. Cuando se envía el correo, se muestra un mensaje de Operación correcta debajo del botón.

8. Pasos siguientes: Consulte el [Ejemplo de fragmentos de código de muestra de Microsoft Graph para ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-snippets-sample) para ver ejemplos de operaciones habituales con Microsoft Graph.

## <a name="code-of-note"></a>Código de nota

> Nota: Para entender el código para llamar a la API de Microsoft Graph en una aplicación ASP.NET MVC, consulte [Empezar a usar Microsoft Graph en una aplicación ASP.NET 4.6 MVC](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc).

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs). Autentica al usuario actual e inicializa la memoria caché de token del ejemplo.

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs). Almacena información de token del usuario. Se puede reemplazar por su memoria caché de token personalizada. Más información en [Almacenamiento en la memoria caché de los tokens de acceso en una aplicación de varios inquilinos](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/).

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs). Implementa la interfaz local de IAuthProvider, y obtiene un token de acceso usando el método **AcquireTokenSilentAsync** de MSAL. Se puede reemplazar por su propio proveedor de autenticación. 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs). Inicializa **GraphServiceClient** de la [biblioteca de cliente de Microsoft Graph .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) que se usa para interactuar con Microsoft Graph.

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs). Contiene métodos que usan **GraphServiceClient** para crear y enviar llamadas al servicio Microsoft Graph y procesar la respuesta.
   - La acción **GetMyEmailAddress** obtiene la dirección de correo electrónico del usuario actual de la propiedad **mail** o **userPrincipalName**.
   - La acción **SendMail** envía un correo electrónico en nombre del usuario actual.

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml). Contiene la interfaz de usuario de ejemplo. 

## <a name="questions-and-comments"></a>Preguntas y comentarios

Nos encantaría recibir sus comentarios acerca de este ejemplo. Puede enviarnos sus preguntas y sugerencias a través de la sección [Problemas](https://github.com/microsoftgraph/aspnet-connect-sample/issues) de este repositorio.

Su opinión es importante para nosotros. Conecte con nosotros en [Desbordamiento de pila](http://stackoverflow.com/questions/tagged/microsoftgraph). Etiquete sus preguntas con [MicrosoftGraph].

## <a name="contributing"></a>Colaboradores ##

Si le gustaría contribuir a este ejemplo, consulte [CONTRIBUTING.md](CONTRIBUTING.md).

Este proyecto ha adoptado el [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/) (Código de conducta de código abierto de Microsoft). Para obtener más información, consulte las [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) (Preguntas más frecuentes del código de conducta) o póngase en contacto con [opencode@microsoft.com](mailto:opencode@microsoft.com) con otras preguntas o comentarios.

## <a name="additional-resources"></a>Recursos adicionales

- [Otros ejemplos de Connect de Microsoft Graph](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Información general de Microsoft Graph](http://graph.microsoft.io)
- [Ejemplos de código de Office Developer](http://dev.office.com/code-samples)
- [Centro de desarrollo de Office](http://dev.office.com/)

## <a name="copyright"></a>Copyright
Copyright (c) 2016 Microsoft. Todos los derechos reservados.



