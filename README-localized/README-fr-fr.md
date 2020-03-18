---
page_type: sample
products:
- ms-graph
languages:
- aspx
- javascript
description: "Cet exemple présente la connexion de votre application web ASP.net à l’API de sécurité à l’aide du kit de développement logiciel SDK Microsoft Graph."
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph 
  services:
  - Security
  createdDate: 4/6/2018 9:53:00 AM
---
# Exemple d'API Microsoft Graph Security pour ASP.NET 4.6 (REST)

## Table des matières

* [Introduction](#introduction)
* [Exemple de projet V1.0](#sample-project-v1.0)
* [Exemple de projet V2.0](#sample-project-v2.0)
* [Exemple de projet V3.0](#sample-project-v3.0)
* [Questions et commentaires](#questions-and-comments)
* [Contribution](#contributing)
* [Ressources supplémentaires](#additional-resources)

## Introduction

Commencez la prise en main de l'API Microsoft Graph Security à l'aide de ces trois exemples de projet.

## Exemple de projet V1.0

L'exemple de projet V1.0 crée une interface utilisateur web très simple (appelée UI) illustrant l'utilisation de l'API Microsoft Graph Security pour :

	• Créer et envoyer des requêtes pour récupérer des alertes
	• Mettre à jour les champs de cycle de vie d'une alerte (par ex., l'état, les opinions, les commentaires, etc.)
	• S’abonner à des notifications d’alerte (basées sur une requête filtrée), ainsi qu’un exemple d’écouteur pour les notifications d’alerte. 
Ce projet utilise un kit de développement logiciel (SDK) généré par Microsoft Graph pour interagir avec l'API Microsoft Graph.
	  
	> Remarque : vous pouvez également utiliser directement des requêtes REST pour interagir avec le service API de la sécurité de Graph

## Exemple de projet V2.0

L’exemple de projet V 2.0 crée une version avancée de l’interface utilisateur V1.0, y compris un « ruban de tableau de bord » qui affiche un aperçu statistique des alertes (par ex., les utilisateurs à risque, c’est-à-dire les alertes les plus élevées, etc.) illustrant l’utilisation de l’API Microsoft Graph Security pour :

	• Récupérer le Secure score de l'organisation client et les profils de contrôle du Secure score
	    > En plus des fonctionnalités de V1.0 (ci-dessous)
	• Créer et envoyer des requêtes pour récupérer des alertes
	• Mettre à jour les champs de cycle de vie d'une alerte (par ex., l'état, les opinions, les commentaires, etc.)
	• S’abonner à des notifications d’alerte (basées sur une requête filtrée), ainsi qu’un exemple d’écouteur pour les notifications d’alerte. 

L’interface utilisateur avancée permet de cliquer sur pratiquement n’importe quelle propriété pour générer une requête filtrée pour cette valeur de propriété, ce qui permet d’obtenir une expérience intuitive de recherche « pointer-cliquer ».

	> Remarque : Le projet V 2.0 utilise un kit de développement logiciel (SDK) généré par Microsoft Graph afin d'appeler l’API Microsoft Graph Security pour les alertes et les abonnements,
	 et les appels REST pour Secure score et les profils de contrôle du Secure score (qui sont encore en version bêta).


## Exemple de projet V3.0

L’exemple de projet V3.0 crée une version plus avancée de l’interface utilisateur V2.0, qui utilise un serveur Angular et peut être directement utilisée en tant qu'outil de recherche par des analystes. Il illustre l’utilisation de l’API Microsoft Graph Security pour :

	• Créer des actions de sécurité (Par ex. : bloquer des adresses IP) et les récupérer 
	• Offre la possibilité d’effectuer un filtrage avancé sur l’interface utilisateur pour des alertes, le degré de sécurisation, les actions de sécurité et les abonnements
	    > En plus des fonctionnalités du V2.0 (ci-dessous)
	• Créer et envoyer des requêtes pour récupérer des alertes
	• Mettre à jour les champs de cycle de vie d'une alerte (par ex., l'état, les opinions, les commentaires, etc.)
	• Récupérer le Secure score de l'organisation client et les profils de contrôle du Secure score
	• S’abonner à des notifications d’alerte (basées sur une requête filtrée), ainsi qu’un exemple d’écouteur pour les notifications d’alerte. 


## Questions et commentaires

Nous serions ravis de connaître votre opinion sur cet exemple !
Veuillez nous faire part de vos questions et suggestions dans la rubrique [Problèmes](https://github.com/microsoftgraph/aspnet-connect-rest-sample/issues) de ce référentiel.

Votre avis compte beaucoup pour nous. Communiquez avec nous sur [Stack Overflow](https://stackoverflow.com/questions/tagged/microsoftgraph).
Posez vos questions avec la balise [MicrosoftGraph].

## Contribution ##

Si vous souhaitez contribuer à cet exemple, voir [CONTRIBUTING.md](CONTRIBUTING.md).

Ce projet a adopté le [code de conduite Open Source de Microsoft](https://opensource.microsoft.com/codeofconduct/).
Pour en savoir plus, reportez-vous à la [FAQ relative au code de conduite](https://opensource.microsoft.com/codeofconduct/faq/) ou contactez [opencode@microsoft.com](mailto:opencode@microsoft.com) pour toute question ou tout commentaire.

## Ressources supplémentaires

- [Autres exemples de connexion avec Microsoft Graph](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Présentation de Microsoft Graph](https://graph.microsoft.io)

## Copyright
Copyright (c) 2018 Microsoft. Tous droits réservés.



