param location string = resourceGroup().location

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: 'HybridMessengerKeyVaul1123123123t'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: 'd265db50-98a9-435b-bd92-c58679944d3d'
    accessPolicies: []
  }
}

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'hybridmessengerstorage'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

// Blob Service
resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2021-02-01' = {
  name: 'default'
  parent: storageAccount
  properties: {}
}

// Blob Container
resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: 'hybridmessengercontainer'
  parent: blobService
  properties: {}
}

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: 'hybridmessengersqlserver'
  location: location
  properties: {
    administratorLogin: 'adminUser'
    administratorLoginPassword: 'yourPassword123!'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  name: 'hybridmessengerdb'
  parent: sqlServer
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: 'hybridmessengerappserviceplan'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
}

// Web App
resource webApp 'Microsoft.Web/sites@2021-01-15' = {
  name: 'hybridmessengerwebapp'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
  }
}

// Outputs
output keyVaultUri string = keyVault.properties.vaultUri
output storageAccountName string = storageAccount.name
output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
output webAppName string = webApp.name
