---
page_type: sample
products:
- ms-graph
languages:
- aspx
- javascript
description: "This sample shows how to connect your ASP .Net web app to the Security API using Microsoft Graph SDK."
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph 
  services:
  - Security
  createdDate: 4/6/2018 9:53:00 AM
---
# Exemplo de API de Segurança do Microsoft Graph para ASP.NET 4.6 (REST)

## Sumário

* [Introdução](#introduction)
* [Exemplo de projeto V1.0](#sample-project-v1.0)
* [V2.0](#sample-project-v2.0)
* [V3.0](#sample-project-v3.0)
* [Perguntas e comentários](#questions-and-comments)
* [Colaboração](#contributing)
* [Recursos adicionais](#additional-resources)

## Introdução

Oferecemos três exemplos de projeto para você começar a usar a API de Segurança do Microsoft Graph.

## Exemplo de projeto V1.0

O exemplo de projeto V1.0 cria uma IU (interface do usuário) Web muito básica que demonstra o uso da API de Segurança do Microsoft Graph para:

	• Criar e enviar consultas para recuperar alertas
	• Atualizar os campos do ciclo de vida (por exemplo, status, feedback, comentários etc.) de um alerta
	• Inscrever-se para o recebimento de notificações de alerta (com base em uma consulta filtrada) e um exemplo de listener para notificações de alerta. 
Este projeto usa um SDK gerado pelo Microsoft Graph para interagir com a API do Microsoft Graph.
	  
	> Observação: você também pode usar diretamente consultas REST para interagir com o serviço da API de Segurança do Graph

## Exemplo de projeto V2.0

O exemplo de projeto V2.0 cria uma versão avançada da IU do V1.0, com uma "faixa de opções do painel" que mostra uma exibição estatística dos alertas (por exemplo, usuários em risco, com os alertas mais severos etc.) que demonstra o uso da API de Segurança do Microsoft Graph para:

	• Recuperar perfis de controle de pontuação de segurança e pontuação de segurança
	    > Além das funcionalidades do V1.0 (abaixo)
	• Criar e enviar consultas para recuperar alertas
	• Atualizar os campos do ciclo de vida (por exemplo, status, feedback, comentários etc.) de um alerta
	• Inscrever-se para o recebimento de notificações de alerta (com base em uma consulta filtrada) e um exemplo de listener para notificações de alerta. 

Com a IU avançada, é possível clicar em praticamente qualquer propriedade para gerar uma consulta filtrada para esse valor de propriedade, permitindo uma experiência intuitiva de investigação do tipo "apontar e clicar".

	> Observação: o V2.0 usa um SDK gerado pelo Microsoft Graph para chamar a API de Segurança do Microsoft Graph para alertas e inscrições,
	 e chamadas REST para perfis de controle de pontuação de segurança e pontuação de segurança (ainda em versão Beta).


## Exemplo de projeto V3.0

O exemplo de projeto V3.0 cria uma versão mais avançada da IU do V2.0, que usa um servidor angular e pode ser usado diretamente como ferramenta para fins de investigação por analistas, que demonstra o uso da API de Segurança do Microsoft Graph para:

	• Criar ações de segurança (por exemplo, bloquear IP) e recuperá-las 
	• Fazer a filtragem avançada na IU de alertas, pontuação de segurança, ações de segurança e inscrições
	    > Além das funcionalidades do V2.0 (abaixo)
	• Criar e enviar consultas para recuperar alertas
	• Atualizar os campos do ciclo de vida (por exemplo, status, feedback, comentários etc.) de um alerta
	• Recuperar perfis de controle de pontuação de segurança e pontuação de segurança
	• Inscrever-se para o recebimento de notificações de alerta (com base em uma consulta filtrada) e um exemplo de listener para notificações de alerta. 


## Perguntas e comentários

Gostaríamos de saber sua opinião sobre este exemplo.
Envie perguntas e sugestões na seção [Problemas](https://github.com/microsoftgraph/aspnet-connect-rest-sample/issues) deste repositório.

Seus comentários são importantes para nós. Junte-se a nós na página do [Stack Overflow](https://stackoverflow.com/questions/tagged/microsoftgraph).
Marque suas perguntas com [MicrosoftGraph].

## Colaboração ##

Se quiser contribuir para esse exemplo, confira [CONTRIBUTING.md](CONTRIBUTING.md).

Este projeto adotou o [Código de Conduta do Código Aberto da Microsoft](https://opensource.microsoft.com/codeofconduct/).
Para saber mais, confira as [Perguntas frequentes sobre o Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/) ou entre em contato pelo [opencode@microsoft.com](mailto:opencode@microsoft.com) se tiver outras dúvidas ou comentários.

## Recursos adicionais

- [Outros exemplos de conexão usando o Microsoft Graph](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Visão geral do Microsoft Graph](https://graph.microsoft.io)

## Direitos autorais
Copyright (c) 2018 Microsoft. Todos os direitos reservados.



