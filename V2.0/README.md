# Microsoft Graph Security API Sample for ASP.NET 4.6 (REST)

## Table of contents

- [Introduction](#introduction)
- [Prerequisites](#prerequisites)
- [Register the application](#register-the-application)
- [Grant Admin consent to view Security data](#grant-admin-consent-to-view-security-data)
- [Build and run the sample](#build-and-run-the-sample)
- [Microsoft Graph Security API Sample App v2.0 UI Walkthrough](#microsoft-graph-security-api-sample-app-v2.0-ui-walkthrough)
- [Deploy the sample to Azure](#deploy-the-sample-to-azure)
- [Code of note](#code-of-note)
- [Questions and comments](#questions-and-comments)
- [Contributing](#contributing)
- [Additional resources](#additional-resources)

## Introduction

This sample shows how to connect an ASP.NET 4.6 MVC web app using a Microsoft work or school (Azure Active Directory) account to the Microsoft Graph security API to retrieve security Alerts, update an Alert, Subscribe to Alert notifications, and also a sample listener for Alert notifications. It uses REST calls to interact with the Microsoft Graph API.

The sample uses the [Microsoft Authentication Library (MSAL)](https://www.nuget.org/packages/Microsoft.Identity.Client/) for authentication. The MSAL SDK provides features for working with the [Azure AD v2.0 endpoint](https://azure.microsoft.com/en-us/documentation/articles/active-directory-appmodel-v2-overview) that enables developers to write a single code flow that handles authentication for both work or school (Azure Active Directory) accounts.

## Important Note about the MSAL Preview

This preview library is suitable for use in a production environment. We provide the same production level support for this library as we do our current production libraries. During the preview we may make changes to the API, internal cache format, and other mechanisms of this library, which you will be required to incorporate along with bug fixes and/or feature improvements. This may impact your applications, for example, a change to the cache format may impact your users, by requiring them to sign in again. An API change may require you to update your code. When we provide the General Availability release we will require you to update to the General Availability version within six months, as applications written using a preview version of library may no longer work.

## Prerequisites

This sample requires the following:  

  * [Visual Studio 2015 or higher](https://www.visualstudio.com/en-us/downloads) 
  * [Microsoft work or school account](https://www.outlook.com) 

## Getting started with sample

 1. Download or clone the Microsoft Graph Security API Sample for ASP.NET 4.6 (REST).

### Create your app

#### Choose the tenant where you want to create your app

1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account.
1. If your account is present in more than one Azure AD tenant:
   1. Select your profile from the menu on the top right corner of the page, and then **Switch directory**.
   1. Change your session to the Azure AD tenant where you want to create your application.

#### Register the app

1. Navigate to the [Azure portal > App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) to register your app.
1. Select **New registration**.
1. When the **Register an application page** appears, enter your app's registration information:
   1. In the **Name** section, enter a meaningful name that will be displayed to users of the app. For example: `MyWebApp`
   1. In the **Supported account types** section, select **Accounts in any organizational directory and personal Microsoft accounts (e.g. Skype, Xbox, Outlook.com)**.
1. Select **Register** to create the app.
1. On the app's **Overview** page, find the **Application (client) ID** value and record it for later. You'll need this value to configure the Visual Studio configuration file for this project.
1. In the list of pages for the app, select **Authentication**.
   1. In the **Redirect URIs** section, select **Web** in the combo-box and enter the following redirect URIs:
       - `https://localhost:44334/signin-oidc`
       - `https://localhost:44334/Account/GrantPermissions`
1. In the **Advanced settings** > **Implicit grant** section, check **ID tokens** as this sample requires the [Implicit grant flow](https://docs.microsoft.com/azure/active-directory/develop/v2-oauth2-implicit-grant-flow) to be enabled to sign-in the user and call an API.
1. Select **Save**.
1. From the **Certificates & secrets** page, in the **Client secrets** section, choose **New client secret**.
   1. Enter a key description (of instance `app secret`).
   1. Select a key duration of either **In 1 year**, **In 2 years**, or **Never Expires**.
   1. When you click the **Add** button, the key value will be displayed. Copy the key value and save it in a safe location.

      You'll need this key later to configure the project in Visual Studio. This key value will not be displayed again, nor retrievable by any other means, so record it as soon as it is visible from the Azure portal.

1. In the list of pages for the app, select **API permissions**.
   1. Click the **Add a permission** button and then make sure that the **Microsoft APIs** tab is selected.
   1. In the **Commonly used Microsoft APIs** section, select **Microsoft Graph**.
   1. In the **Delegated permissions** section, make sure that the following permissions are checked: **SecurityEvents.Read.All**, **SecurityEvents.ReadWrite.All**, **User.Read.All** permission is checked. Use the search box if necessary.
    > These permissions will allow the sample application to read and modify security events (alerts) and to retrieve information about users from Azure Active Directory via the Microsoft Graph API.
   1. Select the **Add permissions** button.

## Grant Admin consent to view Security data

### Assign Scope (permission)

1. Provide your Administrator the **Application Id** and the **Redirect URI** that you used in the previous steps. The organization’s Azure Active Directory Tenant Administrator is required to grant the required consent (permissions) to the application.
2.	As the Tenant Administrator for your organization, open a browser window and paste the following URL in the address bar (after adding values for TENANT_ID, APPLICATION_ID and REDIRECT_URL):
https://login.microsoftonline.com/TENANT_ID/adminconsent?client_id=APPLICATION_ID&state=12345&redirect_uri=REDIRECT_URL.
> **Note:** Tenant_ID is the same as the AAD Directory ID, which can be found in the Azure Active Directory Blade within [Azure Portal](portal.azure.com). To find your directory ID, Log into [Azure Portal](portal.azure.com) with a tenant admin account. Navigate to “Azure Active Directory”, then “Properties”. Copy your ID under the "Directory ID" field to be used as **TENANT_ID**.
3.	After authenticating, the Tenant Administrator will be presented with a dialog like the following (depending on the permissions the application is requesting)

      >![Scope consent dialog](readme-images/Scope.PNG)

4. By clicking on "Accept" in this dialog, the Tenant Administrator is granting consent to all users of this organization to use this application. Now this application will have the correct scopes (permissions) need to access the Security API, the next section explains how to authorize a specific user within your organization (tenant).
    >**Note:** Because there is no application currently running at the redirect URL you will be receive an error message. This behavior is expected. The Tenant Administrator consent will have been granted by the time this error page is shown.</br>![Scope consent dialog](readme-images/GrantError.png)

### Authorize users in your organization to access the Microsoft Graph security API (Assign required Azure role)

To access security data through the Microsoft Graph security API, the client application must be granted the required permissions and when operating in Delegated Mode, the user signed in to the application must also be authorized to call the Microsoft Graph security API.
This section describes how the Tenant Administrator can authorize specific users in the organization.

1. As a Tenant **Administrator**, sign in to the [Azure Portal](https://portal.azure.com).

2. Navigate to the Azure Active Directory blade.

3. Select **Users**.

4. Select a user account that you want to authorize to access to the Microsoft Graph security API.

5. Select **Directory Role**.

6. Select the **Limited Administrator** radio button and select the check box next to **Security administrator** role

     >  ![Role consent dialog](readme-images/SecurityRole.png)

7. Click the **Save** button at the top of the page

Repeat this action for each user in the organization that is authorized to use applications that call the Microsoft Graph security API. Currently, this permission cannot be granted to security groups.

> **Note:** For more details about the authorization flow, read [Authorization and the Microsoft Graph Security API](https://developer.microsoft.com/en-us/graph/docs/concepts/security-authorization).

## Configure and run the sample for **MicrosoftGraph_Security_API_Sample** project (Part 1)

1. Open the MicrosoftGraph_Security_API_Sample project. In the Web.config file in the root directory, replace the **ida:AppId** and **ida:AppSecret** placeholder values with the application ID and password that you copied during app registration. 
Replace the **ida:RedirectUri** placeholder value with the redirect url that you defined during the application registration

2. Press F5 to build and run the sample. This will restore NuGet package dependencies and open the application.

   >If you see any errors while installing packages, make sure the local path where you placed the solution is not too long/deep. Moving the solution closer to the root of your drive will resolve this issue.

3. Sign in with your work or school account and grant the requested permissions.

4. If your application doesn't have proper scopes or if you haven't followed the previous steps properly, you will see the following page.

     >![Admin consent dialog](readme-images/AdminConsent.PNG)

   >For more details about the authorization flow, read [Authorization and the Microsoft Graph Security API](https://developer.microsoft.com/en-us/graph/docs/concepts/security-authorization) and try again. Once your application has proper scope and admin consent, the home page will be displayed.

## Configure the sample Notification Listener (Part 2).

### **Set up the ngrok proxy**

Set up the ngrok proxy to check the notifications generated for your subscriptions. This is required if you want to test the sample Notification Listener on localhost. 
You must expose a public HTTPS endpoint to create a subscription and receive notifications from Microsoft Graph. While testing, you can use ngrok to temporarily allow messages from Microsoft Graph to tunnel to a *localhost* port on your computer. 

You can use the ngrok web interface (http://127.0.0.1:4040) to inspect the HTTP traffic that passes through the tunnel. To learn more about using ngrok, see the [ngrok website](https://ngrok.com/).  

1. Copy the **URL** port number from the **Properties** window. If the **Properties** window isn't displayed, choose **View > Properties Window**. 

	 >![The URL port number in the Properties window](readme-images/ngrok.JPG)

2. [Download ngrok](https://ngrok.com/download) for Windows.  

3. Unzip the package and run ngrok.exe.

4. Replace the two *{port-number}* placeholder values in the following command with the port number you copied above, and run the following command in the ngrok console:

   `ngrok http {port-number} -host-header=localhost:{port-number}`

	  >![Example command to run in the ngrok console](readme-images/ngrok1.PNG)

5. Copy the HTTPS URL displayed in the console. You'll use this to configure your notification URL in the sample.

	  >![The forwarding HTTPS URL in the ngrok console](readme-images/ngrok2.PNG)

   >Keep the console open while testing. If you close it, the tunnel will also be closed, and you'll need to generate a new URL and update the sample.

See [Hosting without a tunnel](https://github.com/microsoftgraph/nodejs-webhooks-rest-sample/wiki/Hosting-the-sample-without-a-tunnel) and [Why do I have to use a tunnel?](https://github.com/microsoftgraph/nodejs-webhooks-rest-sample/wiki/Why-do-I-have-to-use-a-tunnel) for more information.

### Changes to make in **MicrosoftGraph_Security_API_Sample** project

1. Open the **Web.config** file. For the **NotificationUrl** key, replace *ENTER_YOUR_URL* with the HTTPS URL. Keep the */notification/listen* portion. If you're using ngrok, use the HTTPS URL that you copied. The value will look something like this:

   `<add key="ida:NotificationUrl" value="https://0f6fd138.ngrok.io/notification/listen" />`

2. Make sure that the ngrok console is still running, then press F5 to build and run the solution in debug mode. 

   >If you get errors while installing packages, make sure the local path where you placed the solution is not too long/deep. Moving the solution closer to the root drive will resolve this issue.
   
   >If you update any dependencies for this sample, make sure you **do not update** System.IdentityModel.Tokens.Jwt to v5, which is designed for use with .NET Core.

## Microsoft Graph Security API Sample App v2.0 UI Walkthrough

### Using the Microsoft Security Graph API Sample App v2.0

a. Click on "Sign in with Microsoft" in the black navigation bar at the top right of the webpage to launch the Microsoft login experience.

   >![Figure 1. Microsoft Graph Security API Client Demo landing page (pre-sign-in)](readme-images/UI1.JPG)
    Figure 1. Microsoft Graph Security API Client Demo landing page (pre-sign-in)

b. Enter/choose the account you want to sign in with and enter your password.
   >![Figure 2. Microsoft account sign-in](readme-images/UI2.png) Figure 2. Microsoft account sign-in

c. After signing in successfully, the Microsoft Graph Security API Sample App v2.0 UI is displayed.

   >![Figure 3. The Demo Web UI landing page (after sign-in) – alert details tab](readme-images/UI3.png) Figure 3. The Demo Web UI landing page (after sign-in) – alert details tab

### The Microsoft Graph Security API Sample App v2.0 UI (figure 3) consists of the following parts:

1. **“Ribbon” Dashboard**: tiles of statistics, such as [SecureScore](https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/resources/securescores), number of active [alerts](https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/resources/alert) (by severity), users with the most severe alerts, hosts with the most severe alerts, etc.
2. **Visual ‘Query Builder’**: allows selecting the conditions/filters to retrieve alerts by
3. **Alert results**: a list of the alerts matching the defined filtered query; fields shown include: </br>title, severity, status, DateTime, and provider
4. [Subscribe to alert notifications](https://developer.microsoft.com/en-us/graph/docs/concepts/webhooks): receive notifications – from all providers - on new/updated alerts that match the current filtered query
5. There are additional tabs on the right side of the UI in addition to the **alert details** tab (see following figures, and later in the scenarios section):

    a. **Users & hosts** (figure 4) - details retrieved from Azure Active Directory via the Microsoft Graph API about the user and host (if any) related to the alert (in userStates and/or hostStates collections in the alert details).</br>
    b. **Manage alert** (figure 5) - provides a UI to modify the alert status, feedback, and comments.</br>
    c. **Subscriptions** (figure 6) - displays the list of active subscriptions – and a ‘Notifications’ button that displays the notifications pane (in a separate browser tab – figure 7).</br>
    d. **Control Scores**: displays the [Secure Score control profiles](https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/api/securescorecontrolprofiles_list) (figure 8).</br>

   >![Figure 4. Sample App v2.0 UI – Users & Hosts tab (right pane)](readme-images/UI4.png) Figure 4. Sample App v2.0 UI – Users & Hosts tab (right pane).

   >![Figure 5. Sample App v2.0 UI - Manage alerts pane (right tab)](readme-images/UI5.png) Figure 5. Sample App v2.0 UI - Manage alerts pane (right tab).

   >![Figure 6. Sample App v2.0 UI – Subscriptions pane](readme-images/UI6.png) Figure 6. Sample App v2.0 UI – Subscriptions pane.

   >![Figure 7. Sample App v2.0 UI - Notification browser tab](readme-images/UI7.png) Figure 7. Sample App v2.0 UI - Notification browser tab.

   >![Figure 8. Sample App v2.0 UI - Secure Score - left and right panes](readme-images/UI8.png) Figure 8. Sample App v2.0 UI - Secure Score - left and right panes.

   >![Figure 9. Return to the Alert pane after viewing Secure Score panes](readme-images/UI9.png) Figure 9. Return to the Alert pane after viewing Secure Score panes.

### Scenario 1 - Select alerts to view: click on dashboard statistics or build a filtered query

1. Click on “High” under the Active alerts ribbon tile to view all ‘High’ severity active (new and InProgress) alerts (note relevant SDK and REST queries are automatically generated & displayed).

   >![Figure 10. Clicking on dashboard statistic ('High' Active alerts) retrieves all ‘High’ alerts](readme-images/UI10.png) Figure 10. Clicking on dashboard statistic ('High' Active alerts) retrieves all ‘High’ alerts.
 
2. Select one or more filters to apply to the “Get alerts” query, e.g. alerts between two dates: 

   >![Figure 11. Get all alerts during a given time range](readme-images/UI11.png) Figure 11. Get all alerts during a given time range.

Any combination of [filter conditions](https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/resources/alert) may be selected to ‘fine tune’ the alert query, including:

- **Top** (i.e. latest) ‘X’ alerts, e.g. to ensure you’re viewing alerts from all providers.
- Alert **category**, e.g. to easily view all Ransomware-related alerts.
- Alert **status**, e.g. to only view new (unassigned) alerts, or alerts in progress.
- Alert **severity**, e.g. to focus on ‘High’ severity alerts.
- Related **host** -  enables viewing alerts from all providers for a given host (by FQDN, or Fully Qualified Domain Name, which is the NetBIOSName and domain).
- Related **user** - enables viewing alerts from all providers for a given user (by UPN, or User Principal Name, which is the user name and domain)

   >![Figure 12. Get Top (i.e. latest) alert from all providers – matching alerts table](readme-images/UI12.png) Figure 12. Get Top (i.e. latest) alert from all providers – matching alerts table.

    > **Note**: clicking on the REST query (hyperlink) in the right-hand pane will open a new tab displaying the Microsoft Graph Explorer that is prepopulated with the REST query.

   >![Figure 13. MS Graph Explorer pre-populated with the selected REST query](readme-images/UI13.png) Figure 13. MS Graph Explorer pre-populated with the selected REST query.

**Display alert details**: Clicking on any alert title (hyperlink) in the ‘matching alerts’ table will retrieve the alert metadata and display it in the right-hand pane in the **Alert details** tab

   >![Figure 14. Click on an alert title to view alert details (in right hand pane)](readme-images/UI14.png) Figure 14. Click on an alert title to view alert details (in right hand pane).

**Display user and device details**: Select the **User & hosts** tab in the right-hand pane to display details - from Azure Active Directory - of the user related to the alert, and details of the related device, or host.

   >![Figure 15. View alert-related user and device details (Users & hosts tab)](readme-images/UI15.png) Figure 15. View alert-related user and device details (Users & hosts tab).

### Scenario 2: Manage alert lifecycle

This scenario demonstrates a common flow of updating lifecycle alert fields, e.g. "status", "assignedTo", etc. – one that usually occurs in a ticketing/case management system, not in each provider’s UI/portal. After selecting an alert from results list, select the **Manage alert** tab in the right pane.
>**Note**: updating an alert requires its alert ID (OData PATCH operations require entity ID as a parameter).

1. **Set alert status, feedback, and enter comments** </br>Enter the ID of the alert to be updated in the ‘Alert ID’ text box, select desired values from the ‘Status’ and ‘Feedback’ drop down menus, enter any comments, and click ‘Update alert’ button.

*Note: updates to alerts in the demo tenant are reset to their original values after ~15 minutes.*

   >![Figure 16. Scenario 2: Update alert request (in right pane, Manage alert tab)](readme-images/UI16.png) Figure 16. Scenario 2: Update alert request (in right pane, Manage alert tab).

2. **Alert update – results tab** </br>After the alert update request is submitted, the demo displays the ‘before’ and ‘after’ state of the alert fields (by retrieving the alert after the update is applied to it).

   >![Figure 17. Scenario 2: The screen after updating an alert: 'before' & 'after' ](readme-images/UI17.png) Figure 17. Scenario 2: The screen after updating an alert: 'before' & 'after'.

### Scenario 3: Subscribe to alert notifications

The last scenario demonstrates subscribing to notifications for new or updated alerts that match specific criteria (basically a filtered query). Microsoft Graph Security API implements [alert notification subscriptions using webhooks](https://developer.microsoft.com/en-us/graph/docs/concepts/webhooks), and if the subscription criteria do not include a specific provider, the subscription is federated to all providers, i.e. any alert’s matching the subscription criteria – from any provider – will generate notifications.

*The Microsoft Graph Security API Demo includes a ‘listener’ process that alert notifications are sent to by each subscription – clicking on the “Notify” button will open a new web tab that will be updated with any received alert notifications.*

1. **Create alert notifications subscription** </br>In the area immediately below the ‘Query Builder’, the Graph Security API SDK and REST queries are displayed (see figure 17). To subscribe to notifications of new or modified alerts matching this query, click the ‘Subscribe’ button on the right to create the subscription (webhook).</br>
    **Note**: since the Graph Security API service federates queries to all licensed providers (unless provider/s is/are specified in the query), the subscription to alert notifications matching this query will be submitted to all providers, and they will send notification of any matching alerts.

   >![Figure 18. Scenario 3: Subscribe to alert notifications (userPrincipalName= ‘pattif@M365x594651.onmicrosoft.com’)](readme-images/UI18.png) Figure 18. Scenario 3: Subscribe to alert notifications (userPrincipalName= ‘pattif@M365x594651.onmicrosoft.com’).

    After clicking the “Subscribe” button, a new tab is displayed in the right-hand pane, displaying the details of the newly created subscription.

   >![Figure 19. Scenario 3: Create alert notification subscription - details pane](readme-images/UI19.png) Figure 19. Scenario 3: Create alert notification subscription - details pane.

2. **View current subscriptions** </br>Clicking on the “Subscriptions” tab in the right pane will display the list of all current (i.e. active) subscriptions. The “Notifications” button will open a new browser tab for alert notifications.

   >![Figure 20. Scenario 3: Subscriptions tab - shows list of all current subscriptions](readme-images/UI2.png) Figure 20. Scenario 3: Subscriptions tab - shows list of all current subscriptions.

3. **Listen for/view subscribed to alert notifications** </br>Clicking on the “Notifications” button in the “Subscriptions” pane opens a new browser tab for the alert listener process and displays received alert notification (note: this tab will self-refresh).

   >![Figure 21. Scenario 3 – Open an alert notification tab (listener)](readme-images/UI21.png) Figure 21. Scenario 3 – Open an alert notification tab (listener).

4. **Test the alert notification subscription** </br>Update an alert that was generated by Cloud Application Security (per the subscription described above, e.g. `alertId=’9092165B-6260-31B5-A242-7244681BC65’`), changing its status to `‘InProgress’`, its feedback value to `‘TruePositive’`, and enter a comment (see “Scenario 2: Alert Management” above). Then open the **Notifications** tab and wait a few seconds, and the alert notification will be displayed there.

   >![Figure 22. Scenario 3 – Testing alert notifications (listener tab)](readme-images/UI22.png) Figure 22. Scenario 3 – Testing alert notifications (listener tab).

This concludes the Microsoft Graph Security API Sample App v2.0 walkthrough – feel free to explore it further and create your own filtered queries!

## Deploy the sample to Azure

If you have an Azure subscription, you can publish the sample app to an Azure website. 
The following instructions assume that you've already [registered the sample app](#register-the-application) in the Application Registration Portal.

### Create a web app (website) in the Azure portal

1. Sign into the [Azure Portal](https://portal.azure.com) with your Azure credentials.

2. Choose **New > Web + Mobile > Web App**.

3. Give the website a unique name. Choose a resource group, and click **Create**.

4. Select the new web app in your list of resources.

5. Choose **Overview** in the left-hand pane, and then choose **Get publish profile** (or **More > Get publish profile**) from the menu above the web app's Essentials pane.

6. Save the profile locally.

### Publish the MicrosoftGraph_Security_API_Sample app from Visual Studio

1. In Visual Studio, open the MicrosoftGraph_Security_API_Sample application. 
Right-click the **MicrosoftGraph_Security_API_Sample** project node and choose **Publish**.

2. In the Publish dialog, choose **Import**, and choose the publish profile file you downloaded. 

3. On the Connection tab of the Publish dialog, change the *http* protocol to *https* in the Destination URL of your new web app.

4. Copy the Destination URL and click **Publish**. Close the browser window that is opened.

5. In the Web.config file, replace the **ida:RedirectUri** value to the Destination URL that you copied and the **ida:NotificationUrl** value to the Destination URL, keeping the /notification/listen part. 

      Ex: <add key="ida:RedirectUri" value="https://sampletest.azurewebsites.net/" 

      <add key="ida:NotificationUrl" value="https://sampletest.azurewebsites.net/notification/listen"

6. Choose **View > Other Windows > Web Publish Activity**. 

7. In the Web Publish Activity window, click the **Publish Web** button (it looks like a globe) on the toolbar. This is how you update the published project after you make changes.

### Update the redirect URI in the Application Registration Portal

1. In the [Application Registration Portal](https://apps.dev.microsoft.com), open the application that you registered for the sample (as described in [Register the application](#register-the-application)). 

2. In the Web platform section, replace the *http://localhost:55065/* Redirect URI with the Destination URL of your new MicrosoftGraph_Security_API_Sample web application. 
(Alternatively, you can click **Add Url** and add the Destination URL.)

3. Click **Save**.

The new Azure web application should now be ready to use.

## Code of note

- [Startup.Auth.cs](MicrosoftGraph_Security_API_Sample/App_Start/Startup.Auth.cs). 
Authenticates the current user and initializes the sample's token cache.

- [SessionTokenCache.cs](MicrosoftGraph_Security_API_Sample/TokenStorage/SessionTokenCache.cs). Stores the user's token information. You can replace this with your own custom token cache. 
Learn more in [Caching access tokens in a multitenant application](https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/).

- [SampleAuthProvider.cs](MicrosoftGraph_Security_API_Sample/Helpers/SampleAuthProvider.cs). Implements the local IAuthProvider interface, and gets an access token by using the MSAL **AcquireTokenSilentAsync** method. You can replace this with your own authentication provider.

- [GraphService.cs](MicrosoftGraph_Security_API_Sample/Models/GraphService.cs).
Contains methods (called by HomeController) that build and send REST calls to the Microsoft Graph API and process the response.

  - The **GetAlerts** action gets alerts based on the filtering criteria.
  - The **GetAlertById** action gets the specific alert filtered by alert ID.
  - The **UpdateAlert** action updates specific fields of the alert.
  - The **GetUserDetails** action gets the user's details filtered by the user principal name in the alert details.
  - The **GetDeviceById** action gets the device details filtered by the fully qualified domain name (fqdn) in the alert details.
  - The **Subscribe** action creates a subscription according to the subscription filters.
  - The **ListSubscriptions** action gets all the active subscriptions for the application.
  - The **GetSecureScore** action gets the secure score for the tenant.
  - The **GetSecureScoreControlProfiles** action gets the secure score control profiles for the tenant.


- [Graph.cshtml](MicrosoftGraph_Security_API_Sample/Views/Home/Graph.cshtml). 
Contains the sample's UI.

- [NotificationController.cs](MicrosoftGraph_Security_API_Sample/Controllers/NotificationController.cs). 
Contains Listen method to listen to the notifications.

- [Notification.cs](MicrosoftGraph_Security_API_Sample/Models/Notification.cs). 
Represents a change notification.

- [Notification.cshtml](MicrosoftGraph_Security_API_Sample/Views/Notification/Notification.cshtml). Displays information about received messages.

## Questions and comments

We'd love to get your feedback about this sample! 
Please send us your questions and suggestions in the [Issues](https://github.com/microsoftgraph/aspnet-connect-rest-sample/issues) section of this repository.

Your feedback is important to us. Connect with us on [Stack Overflow](https://stackoverflow.com/questions/tagged/microsoftgraph).
Tag your questions with [MicrosoftGraph].

## Contributing

If you'd like to contribute to this sample, see [CONTRIBUTING.md](CONTRIBUTING.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Additional resources

- [Microsoft Graph Security API Documentaion](https://aka.ms/graphsecuritydocs)
- [Other Microsoft Graph Connect samples](https://github.com/MicrosoftGraph?utf8=%E2%9C%93&query=-Connect)
- [Microsoft Graph overview](https://graph.microsoft.io)

## Copyright
Copyright &copy; 2018 Microsoft. All rights reserved.
