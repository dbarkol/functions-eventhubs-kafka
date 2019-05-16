# Azure Functions, Event Hubs and Kafka Demo

This code in this repository demonstrates how to:

* Send messages from an Azure Function to [Event Hubs](https://azure.microsoft.com/en-us/services/event-hubs/) using the [Kafka](https://kafka.apache.org/) protocol.
* [Use dependency injection in .NET Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection) for a Kafka producer.
* Consume message from a Kafka topic using [kafkacat](https://github.com/edenhill/kafkacat).

## Setup using the Azure CLI:

The following steps will create an instance of Azure Event Hubs that is enabled with the Kafka protocol. It will also create an event hub (kafka topic) and access policy for sending and listening to events.

#### Initialize some variables:
```
rgname={your resource group name}
location={preferred azure region}
ehnamespace={unique event hubs namespace name}
ehname={event hub/topic name}
authorizationrule=authorizationpolicy
```

#### Create resources:
```
# create resource group
az group create -n $rgname -l $location

# create event hubs namespace with kafka enabled
az eventhubs namespace create -g $rgname --name $ehnamespace -l $location --sku Standard --enable-kafka true

# create event hub
az eventhubs eventhub create -g $rgname --namespace-name $ehnamespace --name $ehname

# create shared access policy with send and listen rights
az eventhubs eventhub authorization-rule create --eventhub-name $ehname --name $authorizationrule --namespace-name $ehnamespace -g $rgname --rights Send Listen

# query for the primary connection string
az eventhubs eventhub authorization-rule keys list --resource-group $rgname --namespace-name $ehnamespace --eventhub-name $ehname --name $authorizationrule --query "primaryConnectionString"
```

## Azure Function as a Kafa Producer

This is an example of how an HTTP-triggered function can be used as a producer to a Kafka topic.

#### Function settings for local.settings.json:
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "IsLocal": "1",
        "EventHubConnectionString": "{your-event-hub-connection-string}",
        "EventHubFqdn": "{event-hub-namespace}.servicebus.windows.net:9093",
        "EventHubName": "{event-hub-name}"
    }
}
```

When deployed in Azure, the last three settings (EventHubConnectionString, EventHubFqdn, EventHubName) must be configured in the application settings. The IsLocal should be emitted.

## Consume events with kafkacat

[kafkacat]((https://github.com/edenhill/kafkacat)) is a great and easy-to-use utility for consuming events from a Kafka topic.

#### Setup
Install kafkacat: https://github.com/edenhill/kafkacat. 

If you are using a Windows machine, you can use the [Windows Subsystem for Linux (WSL)](https://docs.microsoft.com/en-us/windows/wsl/about).  

#### Consume events
From the terminal:

```
# initialize variables
connectionString="{your event hub connection string}"
kafkaEndpoint="{event hub namespace name}.servicebus.windows.net:9093"

# consume events (starting at the end offset)
kafkacat -X security.protocol=SASL_SSL -X sasl.mechanisms=PLAIN -X sasl.username="\$ConnectionString" -X sasl.password=$connectionString -b $kafkaEndpoint -t $topicname -o end
```


