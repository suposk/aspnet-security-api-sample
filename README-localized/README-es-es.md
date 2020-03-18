---
page_type: sample
products:
- ms-graph
languages:
- aspx
- javascript
description: "En este ejemplo se muestra cómo conectar la aplicación Web de ASP .net a la API de seguridad con el SDK de Microsoft Graph"
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph 
  services:
  - Security
  createdDate: 4/6/2018 9:53:00 AM
---
# Ejemplo de la API de seguridad de Microsoft Graph para ASP.NET 4,6 (REST)

## Tabla de contenido

* [Introducción](#introduction)
* [Proyecto de ejemplo V 1.0](#sample-project-v1.0)
* [Ejemplo de Project V 2.0](#sample-project-v2.0)
* [Ejemplo de Project V 3.0](#sample-project-v3.0)
* [Preguntas y comentarios](#questions-and-comments)
* [Colaboradores](#contributing)
* [Recursos adicionales](#additional-resources)

## Introducción

Hay tres proyectos de ejemplo para que pueda empezar a usar la API de seguridad de Microsoft Graph.

## Proyecto de ejemplo V 1.0

El proyecto de muestra V1.0 crea una interfaz de usuario web muy básica (también conocida como UI) que permite demostrar el uso de la API de seguridad de gráficos de Microsoft:

	• Crear y enviar consultas para lograr la recuperación de alertas
	• Actualizar los campos del ciclo de vida (por ejemplo, estado, retroalimentación, sugerencias, etc.) de una alerta
	• Suscríbase a las notificaciones de alerta (basada en una consulta filtrada), así como un oyente de ejemplo para las notificaciones de alerta. 
Este proyecto utiliza un SDK generado por Microsoft Graph para interactuar con la API de Microsoft Graph.
	  
	> Nota: también puede usar directamente consultas REST para interactuar con el servicio API de seguridad de Graph.

## Ejemplo de Project V 2.0

El proyecto de muestra V 2.0 crea una versión avanzada de la interfaz de usuario de V 1.0, lo que incluye una "cinta de opciones de panel" que muestra una vista estadística de las alertas (por ejemplo, los usuarios en peligro, es decir, con las alertas de gravedad, etc.), lo que demuestra el uso de la API de seguridad de Microsoft Graph para:

	• Recuperar los perfiles de calificación segura y control de calificación segura de la organización del cliente.
	    > Además de la funcionalidad de V 1.0 (que se muestra a continuación)
	• Crear y enviar consultas para lograr la recuperación de alertas
	• Actualizar los campos del ciclo de vida (por ejemplo, estado, retroalimentación, sugerencias, etc.) de una alerta
	• Suscríbase a las notificaciones de alerta (basada en una consulta filtrada), así como un oyente de ejemplo para las notificaciones de alerta. 

La interfaz de usuario avanzada le permite hacer clic en prácticamente cualquier propiedad para generar una consulta filtrada para el valor de la propiedad, lo que permite una experiencia de investigación intuitiva con un punto y un clic.

	> Nota: V 2.0 usa un SDK generado por Microsoft Graph para llamar a la API de seguridad de Microsoft Graph para las alertas y suscripciones,
	 y las llamadas REST para puntuaciones seguras y los perfiles de control de calificación segura (ya que se siguen en la versión beta).


## Ejemplo de Project V 3.0

El proyecto de ejemplo V 3.0 crea una versión más avanzada de la interfaz de usuario de V 2.0, en la que se usa un servidor angular y se puede usar directamente como una herramienta para propósitos de investigación por analistas, muestra cómo usar la API de seguridad de Microsoft Graph para:

	• Cree acciones de seguridad (ej.: Bloquear IP) y recuperarlas 
	• Ofrece funciones para realizar un filtrado avanzado en la interfaz de usuario de alertas, calificación segura, acciones de seguridad y suscripciones.
	    > Además de la funcionalidad de V2.0 (que se muestra a continuación)
	• Crear y enviar consultas para lograr la recuperación de alertas
	• Actualizar los campos del ciclo de vida (por ejemplo, estado, retroalimentación, sugerencias, etc.) de una alerta
	• Recuperar los perfiles de calificación segura y control de calificación segura de la organización del cliente.
	• Suscríbase a las notificaciones de alerta (basada en una consulta filtrada), así como un oyente de ejemplo para las notificaciones de alerta. 


## Preguntas y comentarios

¡Nos encantaría recibir sus comentarios sobre este ejemplo!
Envíenos sus preguntas y sugerencias en la sección [problemas](https://github.com/microsoftgraph/aspnet-connect-rest-sample/issues) de este repositorio.

Su opinión es importante para nosotros. Conecte con nosotros en [Stack Overflow](https://stackoverflow.com/questions/tagged/microsoftgraph).
Etiquete sus preguntas con [MicrosoftGraph].

## Colaboradores ##

Si le gustaría contribuir a este ejemplo, vea [CONTRIBUTING.md](CONTRIBUTING.md).

Este proyecto ha adoptado el [Código de conducta de código abierto de Microsoft](https://opensource.microsoft.com/codeofconduct/).
Para obtener más información, vea [Preguntas frecuentes sobre el código de conducta](https://opensource.microsoft.com/codeofconduct/faq/) o póngase en contacto con [opencode@microsoft.com](mailto:opencode@microsoft.com) si tiene otras preguntas o comentarios.

## Recursos adicionales

- [Otros ejemplos Connect de Microsoft Graph](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Información general de Microsoft Graph](https://graph.microsoft.io)

## Derechos de autor
Copyright (c) 2018 Microsoft. Todos los derechos reservados.



