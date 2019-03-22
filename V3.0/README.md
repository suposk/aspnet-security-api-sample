# Graph Security API Demo v3

## Configurations

#### Server application

Configurations for server side application are in the `appsettings.<ASPNETCORE_ENVIRONMENT>.json` (e.g. appsettings.Development.json or appsettings.Production.json) files in the project root directory. When you start the application, the settings are automatically read from the file whose name matches the current environment name. The default in the Visual Studio is Development environment. You can add any other environments and settings for them at your discretion. 
App settings consist of 3 main properties:
1.	**AzureAd** is a complex-type property for setup auth using Azure Active Directory.
2.	**UseMockFilters** is a boolean value. If set to true, then the application uses mock data from `mockData.json`. Read more in the 'Caching and mock data' section.
3.	**CacheTime** is a complex-type property that contains times  in seconds for caching for all actions, that support cache.
	
#### Client application

The client application is located in the `ClientApp` folder. And the settings for it are in the `ClientApp/environments` folder. As well as on the server, there can be many different environments and settings for each of them. By default, there are settings for two environments:
1. `environment.ts` is settings for development environment.
2. `environment.prod.ts` is the settings that are used when building the production version of the Angular application by default.

## Development and launch app locally

In order to run the application in a local environment, you need to perform the following steps:
1. Install [.Net Core SDK v2.2](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.104-windows-x64-installer)
2. Install LTS version of [NodeJS](https://nodejs.org/uk/)
3. Install Globally [angular cli](https://cli.angular.io/) using command `npm install -g @angular/cli`.
4. Restore Nuget packages for .Net core app.
5. Install client dependencies using `npm install` command in the client app root directory (ClientApp).
6. Run server app.
7. Run client app using `npm start` command in the client app root directory (ClientApp).

#### Create subscriptions and get notifications in local environment

In order to work with subscriptions in local environment you need to use ngrok tool to proxy you port with server app.
Use `ngrok http <server_app_port>` to run ngrok tool and get urls, that are available outside your local environment. By default server app use 5000 port, so command to run ngrok looks like `ngrok http 5000`.
You need to use **https** url from ngrok in the `appsettings.Development.json` in the `AzureAD.NotificationUri` property.

## Deployment to Azure

1. Download publish profile for application from Azure portal.
2. Setup appsettings for Production environment.
3. In the Azure portal in application settings setup new environment variable with name `ASPNETCORE_ENVIRONMENT` and `Production` value (by default). This is necessary so that the application reads correct appsettings file at startup.
4. Publish project in Visual Studio.

#### Create subscriptions and get notifications in Azure

In order to use Notification in application deployed to Azure, you need to enable **Web sockets** option in application settings of this WebApp in the Azure Portal.

## Caching and mock data

Because for the application to work, we have to do many simultaneous requests to the Graph API, and the response time of the Graph API is often very large. Moreover, not all requests can be parallelized, since for some queries, we need to wait for the results of previous queries. So we use cache. And there are two sections in the appsettings file that allow you to customize this behavior:
1. `UseMockFilters`. If is true, we use date for alerts' and actions' filters from `mockData.json`. This file contains a list of possible providers, categories and other values needed to build filters on the client. This method works much faster. Therefore, for better performance, this method will be preferable. If `UseMockFilters` parameter is false, then when the application is started (once), we load the list of categories, providers and other data from the Graph API, and then use them. This method allows you to get more relevant data from Graph API but significantly increases the time of the first launch of the application.
2. `CacheTime`. In this section of settings you can specify the desired time to cache data on our server in seconds. To disable caching for any action, specify 0. MemoryCache used for caching data from Graph API. You can significantly speed up subsequent queries for the same data using caching.

## Where are list items' order defined in code?

You can change order of displaying lists with properties on the alert details page in the following file:
`ClientApp/src/components/pages/alert-details/alert-details.component.html`. All lists with details are in this file in the following section:
```
    <!-- #region scrollable-detail-list -->
    ...
    <!--#endregion scrollable-detail-list -->
```
You can change the order at any time at your discretion.