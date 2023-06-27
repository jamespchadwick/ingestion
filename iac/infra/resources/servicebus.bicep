param name string
param location string = resourceGroup().location
param tags object = {}
param sku object
param deadLetterQueueName string
param topicName string
param keyVaultName string

@secure()
param principalId string

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: name
  location: location
  tags: tags
  sku: sku
  properties: {
    minimumTlsVersion: '1.2'
  }
}

resource deadLetterQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  name: deadLetterQueueName
  parent: serviceBus
  properties: { }
}

resource topic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  name: topicName
  parent: serviceBus
  properties: { }

  resource processingCliSubscription 'subscriptions@2022-10-01-preview' = {
    name: 'Ingestion-Processing-Cli'
    properties: {
      forwardDeadLetteredMessagesTo: deadLetterQueue.name
    }
  }
}

resource serviceBusDataReceiverRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
}

resource serviceBusDataSenderRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
}

resource serviceBusReceiverPermission 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(serviceBus.id, principalId, serviceBusDataReceiverRoleDefinition.id)
  scope: serviceBus
  properties: {
    principalId: principalId
    principalType: 'User'
    roleDefinitionId: serviceBusDataReceiverRoleDefinition.id
  }
}

resource serviceBusSenderPermissions 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(serviceBus.id, principalId, serviceBusDataSenderRoleDefinition.id)
  scope: serviceBus
  properties: {
    principalId: principalId
    principalType: 'User'
    roleDefinitionId: serviceBusDataSenderRoleDefinition.id
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
  scope: resourceGroup()
}

resource serviceBusFullyQualifiedNamespace 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: 'ServiceBus--FullyQualifiedNamespace'
  parent: keyVault
  properties: {
    value: '${serviceBus.name}.servicebus.windows.net'
  }
}
