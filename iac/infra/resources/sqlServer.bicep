param name string
param location string = resourceGroup().location
param tags object = {}
param keyVaultName string

@secure()
param administratorLogin string

@secure()
param administratorPassword string

param databaseName string

@secure()
param ipAddress string

resource sqlServer 'Microsoft.Sql/servers@2022-08-01-preview' = {
  name: name
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
  }
}

resource firewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'James Localhost'
  parent: sqlServer
  properties: {
    endIpAddress: ipAddress
    startIpAddress: ipAddress
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
  scope: resourceGroup()
}

resource connectionString 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: 'SqlServer--ConnectionString'
  parent: keyVault
  properties: {
    value: 'Data Source=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=REGISTRY;User Id=${administratorLogin};Password=${administratorPassword};'
  }
}
