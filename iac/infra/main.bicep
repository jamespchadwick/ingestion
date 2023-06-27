targetScope = 'subscription'

@minLength(1)
@maxLength(64)
param environmentName string

@minLength(1)
param location string

@minLength(1)
param principalId string

@minLength(1)
param ipAddress string

var abbrs = loadJsonContent('abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }

resource resourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

module keyVault 'resources/keyvault.bicep' = {
  name: 'keyvault'
  scope: resourceGroup
  params: {
    name: '${abbrs.keyVaultVaults}${resourceToken}'
    location: location
    tags: tags
    principalId: principalId
  }
}

module logAnalyticsWorkspace 'resources/logAnalytics.bicep' = {
  name: 'loganalytics'
  scope: resourceGroup
  params: {
    name: '${abbrs.logAnalyticsWorkspace}${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: 'PerGB2018'
    }
  }
}

module applicationInsights 'resources/applicationinsights.bicep' = {
  name: 'applicationinsights'
  scope: resourceGroup
  params: {
    name: '${abbrs.applicationInsights}${resourceToken}'
    location: location
    tags: tags
    workspaceResourceId: logAnalyticsWorkspace.outputs.logAnalyticsId
    keyVaultName: keyVault.outputs.keyVaultName
  }
}

module sqlServer 'resources/sqlServer.bicep' = {
  name: 'sqlserver'
  scope: resourceGroup
  params: {
    name: '${abbrs.sqlServers}${resourceToken}'
    location: location
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
    administratorLogin: 'sqladmin'
    administratorPassword: 'myStr0ngP4ssword!'
    databaseName: environmentName
    ipAddress: ipAddress
  }
}

module storageAccount 'resources/storageAccount.bicep' = {
  name: 'storageaccount'
  scope: resourceGroup
  params: {
    name: '${abbrs.storageStorageAccounts}${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: 'Standard_LRS'
      tier: 'Standard'
    }
    principalId: principalId
    ipAddress: ipAddress
  }
}

module serviceBus 'resources/servicebus.bicep' = {
  name: 'servicebus'
  scope: resourceGroup
  params: {
    name: '${abbrs.serviceBusNamespaces}${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: 'Standard'
      tier: 'Standard'
    }
    deadLetterQueueName: '${abbrs.serviceBusNamespacesQueues}deadletter-${resourceToken}'
    topicName: 'messages'
    keyVaultName: keyVault.outputs.keyVaultName
    principalId: principalId
  }
}

output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
output AZURE_RESOURCE_GROUP string = resourceGroup.name
output AZURE_KEYVAULT_VAULTURI string = keyVault.outputs.keyVaultUri
output AZURE_STORAGE_FILEDROP_CONTAINERURI string = storageAccount.outputs.fileDropContainerUri
