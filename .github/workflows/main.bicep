param functionAppName string = 'crypto-pilot-functionapp'
param coinGeckoBaseUrl string = 'https://api.coingecko.com/api/v3'
param location string = 'uksouth'

param sqlServerAppName string = 'crypto-pilot-sql-server'
param sqlDatabaseName string = 'crypto-pilot-sql-database'

@secure()
param sqlAdminLogin string

@secure()
param sqlAdminPassword string

@description('Name of the Azure Communication Service resource')
param acsResourceName string = 'crypto-pilot-acs-sms-spain'

@description('Data location for the Azure Communication Service')
param acsDataLocation string = 'Europe'

@description('Name of the ACS Email Domain resource')
param acsEmailDomainName string = 'crypto-pilot-email-domain'

@description('Name of the App Service plan for the web app')
param webAppPlanName string = 'crypto-pilot-web-plan'

@description('Name of the Web App for the frontend')
param webAppName string = 'crypto-pilot-webapp'

resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: uniqueString(resourceGroup().id, functionAppName)
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource hostingPlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${functionAppName}-plan'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${functionAppName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
  }
}

resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;EndpointSuffix=${environment().suffixes.storage};AccountName=${storage.name};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME_VERSION'
          value: '~8'
        }
        {
          name: 'CoinGeckoOptions:BaseUrl'
          value: coinGeckoBaseUrl
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'WEBSITE_BITNESS'
          value: '64'
        }
        {
          name: 'EmailService:ConnectionString'
          value: acs.listKeys().primaryConnectionString
        }
        {
          name: 'EmailService:SenderAddress'
          value: 'DoNotReply@${acsEmailDomain.properties.fromSenderDomain}'
        }
        {
          name: 'EMAIL_DOMAIN_RESOURCE_ID'
          value: acsEmailDomain.id
        }
        {
          name: 'CryptoPilotDatabase:Server'
          value: '${sqlServer.name}.database.windows.net'
        }
        {
          name: 'CryptoPilotDatabase:Database'
          value: sqlDatabase.name
        }
        {
          name: 'CryptoPilotDatabase:User'
          value: sqlAdminLogin
        }
        {
          name: 'CryptoPilotDatabase:Password'
          value: sqlAdminPassword
        }
      ]
    }
    httpsOnly: true
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: sqlServerAppName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    zoneRedundant: false
  }
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  dependsOn: [
    sqlServer
  ]
}

resource acs 'Microsoft.Communication/communicationServices@2024-09-01-preview' = {
  name: acsResourceName
  location: 'global'
  properties: {
    dataLocation: acsDataLocation
  }
}

resource emailService 'Microsoft.Communication/emailServices@2023-03-31' = {
  name: '${acsResourceName}-email'
  location: 'global'
  properties: {
    dataLocation: acsDataLocation
  }
}

// Add ACS Email Domain resource
resource acsEmailDomain 'Microsoft.Communication/emailServices/domains@2023-03-31' = {
  name: 'AzureManagedDomain'
  parent: emailService
  location: 'global'
  properties: {
    domainManagement: 'AzureManaged'
    userEngagementTracking: 'Disabled'
  }
}

// App Service plan for the web app (Standard Linux for static SPA hosting)
resource webAppPlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: webAppPlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// Update web app settings to use parameters instead
param functionAppClientId string = ''
param webAppClientId string = ''

resource webAppSettings 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: webApp
  name: 'appsettings'
  properties: {
    WEBSITES_ENABLE_APP_SERVICE_STORAGE: 'true'
    WEBSITE_RUN_FROM_PACKAGE: '1'
    VITE_AZURE_AD_AUTHORITY: 'https://login.microsoftonline.com/${tenant().tenantId}'
    VITE_AZURE_AD_CLIENT_ID: webAppClientId
    VITE_AZURE_FUNCTION_API_SCOPE: 'api://${functionAppClientId}/user_impersonation'
  }
}

output functionAppName string = functionApp.name
output coinGeckoBaseUrl string = coinGeckoBaseUrl
output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
output acsResourceName string = acs.name
output acsLocation string = acs.location
output acsDataLocation string = acs.properties.dataLocation
output acsConnectionString string = acs.listKeys().primaryConnectionString
output acsEmailDomainName string = acsEmailDomain.name
output acsEmailDomainResourceId string = acsEmailDomain.id
output emailSenderAddress string = 'DoNotReply@${acsEmailDomain.properties.fromSenderDomain}'
output webAppName string = webApp.name
output functionAppClientId string = functionAppClientId
output webAppClientId string = webAppClientId
