param name string
param location string = resourceGroup().location
param tags object = {}
param sku object

@secure()
param principalId string

@secure()
param ipAddress string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: name
  location: location
  tags: tags
  sku: sku
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    // allowSharedKeyAccess: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Deny'
      ipRules: [
        {
          value: ipAddress
          action: 'Allow'
        }
      ]
    }
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  name: 'default'
  parent: storageAccount
  properties: { }
}

resource fileDropContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: 'file-drop'
  parent: blobService
  properties: {
    publicAccess: 'None'
  }
}

resource storageBlobDataReaderRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
}

resource fileDropContainerPermissions 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(fileDropContainer.id, principalId, storageBlobDataReaderRoleDefinition.id)
  scope: fileDropContainer
  properties: {
    principalId: principalId
    principalType: 'User'
    roleDefinitionId: storageBlobDataReaderRoleDefinition.id
  }
}

output fileDropContainerUri string = 'https://${storageAccount.name}.blob.core.windows.net/${fileDropContainer.name}'
