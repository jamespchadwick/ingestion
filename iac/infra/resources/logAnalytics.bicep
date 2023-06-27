param name string
param location string = resourceGroup().location
param tags object = {}
param sku object

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: name
  location: location
  tags: tags
  properties: {
    retentionInDays: 90
    sku: sku
    workspaceCapping: {
      dailyQuotaGb: '0.023'
    }
  }
}

output logAnalyticsId string = logAnalytics.id
