param name string
param location string = resourceGroup().location
param tags object = {}
param workspaceResourceId string
param keyVaultName string

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    RetentionInDays: 90
    WorkspaceResourceId: workspaceResourceId
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
  scope: resourceGroup()
}

resource applicationInsightsInstrumentationKey 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: 'ApplicationInsights--InstrumentationKey'
  parent: keyVault
  properties: {
    value: applicationInsights.properties.InstrumentationKey
  }
}
