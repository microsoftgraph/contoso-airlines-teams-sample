# <a name="microsoft-graph-connect-sample-for-aspnet-46"></a>Exemplo de conexão com o Microsoft Graph para ASP.NET 4.6

## <a name="table-of-contents"></a>Sumário

* [Pré-requisitos](#prerequisites)
* [Registrar o aplicativo](#register-the-application)
* [Criar e executar o exemplo](#build-and-run-the-sample)
* [Código da observação](#code-of-note)
* [Perguntas e comentários](#questions-and-comments)
* [Colaboração](#contributing)
* [Recursos adicionais](#additional-resources)

Este exemplo mostra como conectar um aplicativo Web ASP.NET 4.6 MVC a uma conta corporativa ou de estudante (Azure Active Directory) da Microsoft ou a uma conta pessoal (Microsoft) usando a API do Microsoft Graph para recuperar a imagem de perfil de um usuário, carregar a imagem para o OneDrive e enviar um email que contém a foto como anexo e o link de compartilhamento em seu texto. O exemplo usa a [Biblioteca de Cliente .NET do Microsoft Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet) para trabalhar com dados retornados pelo Microsoft Graph. 

Além disso, o exemplo usa a [Biblioteca de Autenticação da Microsoft (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) para autenticação. O SDK da MSAL fornece recursos para trabalhar com o [ponto de extremidade do Microsoft Azure AD versão 2.0](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview), que permite aos desenvolvedores gravar um único fluxo de código para tratar da autenticação de contas pessoais (Microsoft), corporativas ou de estudantes (Azure Active Directory).

## <a name="important-note-about-the-msal-preview"></a>Observação importante sobre a Visualização da MSAL

Esta biblioteca é adequada para uso em um ambiente de produção. Ela recebe o mesmo suporte de nível de produção que fornecemos às nossas bibliotecas de produção atuais. Durante a visualização, podemos fazer alterações na API, no formato de cache interno e em outros mecanismos desta biblioteca, que você será solicitado a implementar juntamente com correções de bugs ou melhorias de recursos. Isso pode impactar seu aplicativo. Por exemplo, uma alteração no formato de cache pode impactar seus usuários, exigindo que eles entrem novamente. Uma alteração na API pode requerer que você atualize seu código. Quando fornecermos a versão de Disponibilidade Geral, você será solicitado a atualizar a versão de Disponibilidade Geral no prazo de seis meses, pois os aplicativos escritos usando uma versão de visualização da biblioteca podem não funcionar mais.

## <a name="prerequisites"></a>Pré-requisitos

Este exemplo requer o seguinte:  

  * [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads) 
  * A [conta da Microsoft](https://www.outlook.com) ou a [conta do Office 365 para empresas](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account). Inscreva-se em uma [Assinatura de Desenvolvedor do Office 365](https://msdn.microsoft.com/en-us/office/office365/howto/setup-development-environment#bk_Office365Account) que inclua os recursos necessários para começar a criar aplicativos do Office 365.

## <a name="register-the-application"></a>Registrar o aplicativo

1. Entre no [Portal de Registro do Aplicativo](https://apps.dev.microsoft.com/) usando sua conta pessoal ou sua conta comercial ou escolar.

2. Escolha **Adicionar um aplicativo**.

3. Insira um nome para o aplicativo e escolha **Criar aplicativo**. 
    
   A página de registro é exibida, listando as propriedades do seu aplicativo.

4. Copie a ID do Aplicativo. Esse é o identificador exclusivo do aplicativo. 

5. Em **Segredos do Aplicativo**, escolha **Gerar Nova Senha**. Copie a senha da caixa de diálogo **Nova senha gerada**.

   Você usará a ID do aplicativo e a senha para configurar o aplicativo de exemplo na próxima seção. 

6. Em **Plataformas**, escolha **Adicionar plataforma**.

7. Escolha **Web**.

8. Verifique se a caixa de seleção **Permitir Fluxo Implícito** está selecionada e insira *http://localhost:55065/* como o URI de redirecionamento. 

   A opção **Permitir Fluxo Implícito** habilita o fluxo híbrido. Durante a autenticação, isso permite que o aplicativo receba informações de entrada (o id_token) e artefatos (neste caso, um código de autorização) que o aplicativo pode usar para obter um token de acesso.

9. Escolha **Salvar**.

## <a name="build-and-run-the-sample"></a>Criar e executar o exemplo

1. Baixe ou clone o Exemplo de Conexão com o Microsoft Graph para ASP.NET 4.6.

2. Abra a solução de exemplo no Visual Studio.

3. No arquivo Web.config no diretório raiz, substitua os valores dos espaços reservados **ida:AppId** e **ida:AppSecret** pela ID de aplicativo e senha copiadas durante o registro do aplicativo.

4. Pressione F5 para criar e executar o exemplo. Isso restaurará dependências do pacote NuGet e abrirá o aplicativo.

   >Caso receba mensagens de erro durante a instalação de pacotes, verifique se o caminho para o local onde você colocou a solução não é muito longo ou extenso. Para resolver esse problema, coloque a solução junto à raiz da unidade.

5. Entre com sua conta pessoal,corporativa ou de estudante, e conceda as permissões solicitadas.

6. Escolha o botão **Obter endereço de email**. Quando a operação for concluída, o endereço de email do usuário conectado será exibido na página.

7. Como alternativa, edite a lista de destinatários e o assunto do email e, em seguida, escolha o botão **Enviar email**. Quando o email for enviado, será exibida uma mensagem de sucesso abaixo do botão.

8. Próximas etapas: Confira o tópico [Exemplo de trechos de código do Microsoft Graph para ASP.NET 4.6](https://github.com/microsoftgraph/aspnet-snippets-sample) para ver exemplos de operações comuns do Microsoft Graph.

## <a name="code-of-note"></a>Código da observação

> Observação: Para compreender o código de chamada para a API do Microsoft Graph em um aplicativo ASP.NET MVC, confira o tópico [Começar a usar o Microsoft Graph em um aplicativo ASP.NET 4.6 MVC](https://graph.microsoft.io/en-us/docs/platform/aspnetmvc).

- [Startup.Auth.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/App_Start/Startup.Auth.cs). Autentica o usuário atual e inicializa o cache de token do exemplo.

- [SessionTokenCache.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/TokenStorage/SessionTokenCache.cs). Armazena as informações de token do usuário. Você pode substituir pelo seu próprio cache de token personalizado. Saiba mais em [Armazenamento de tokens de acesso em cache em um aplicativo de vários locatários](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/).

- [SampleAuthProvider.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SampleAuthProvider.cs). Implementa a interface IAuthProvider local e obtém acesso a um token usando o método **AcquireTokenSilentAsync da MSAL**. Isso pode ser substituído pelo seu próprio provedor de autenticação. 

- [SDKHelper.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Helpers/SDKHelper.cs). Inicializa o **GraphServiceClient**, na [Biblioteca do Cliente .NET para Microsoft Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet), que é usado para interagir com o Microsoft Graph.

- [HomeController.cs](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Controllers/HomeController.cs). Contém métodos que usam o **GraphServiceClient** para criar e enviar chamadas para o serviço do Microsoft Graph e processar a resposta.
   - A ação **GetMyEmailAddress** obtém o endereço de email do usuário atual das propriedades **mail** ou **userPrincipalName**.
   - A ação **SendMail** envia um email em nome do usuário atual.

- [Graph.cshtml](/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Microsoft%20Graph%20SDK%20ASPNET%20Connect/Views/Home/Graph.cshtml). Contém a interface de usuário do exemplo. 

## <a name="questions-and-comments"></a>Perguntas e comentários

Gostaríamos de saber sua opinião sobre este exemplo. Você pode nos enviar suas perguntas e sugestões por meio da seção [Issues](https://github.com/microsoftgraph/aspnet-connect-sample/issues) deste repositório.

Seus comentários são importantes para nós. Junte-se a nós na página [Stack Overflow](http://stackoverflow.com/questions/tagged/microsoftgraph). Marque suas perguntas com [MicrosoftGraph].

## <a name="contributing"></a>Colaboração ##

Se quiser contribuir para esse exemplo, confira [CONTRIBUTING.md](CONTRIBUTING.md).

Este projeto adotou o [Código de Conduta do Código Aberto da Microsoft](https://opensource.microsoft.com/codeofconduct/). Para saber mais, confira as [Perguntas frequentes do Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/) ou contate [opencode@microsoft.com](mailto:opencode@microsoft.com) se tiver outras dúvidas ou comentários.

## <a name="additional-resources"></a>Recursos adicionais

- [Outros exemplos de conexão usando o Microsoft Graph](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Visão geral do Microsoft Graph](http://graph.microsoft.io)
- [Exemplos de código para desenvolvedores do Office](http://dev.office.com/code-samples)
- [Centro de Desenvolvimento do Office](http://dev.office.com/)

## <a name="copyright"></a>Direitos autorais
Copyright © 2016 Microsoft. Todos os direitos reservados.



