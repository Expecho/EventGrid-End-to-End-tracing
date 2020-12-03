# EventGrid-End-to-End-tracing
Demonstrate how to set up end-to-end tracing using application insights.

Currently there is no support fro end-to-end tracing between the Azure Event Grid and an Event Grid triggered Azure Function when using the Event Grid Event schema. Right now Microsoft is working to address this by supporting end-to-end tracing using the CloudEvent schema and an Event Grid triggered Azure Function.
![Demo](assets/end-to-end-output.png?raw=true )

In the end, for end to end tracing to work existing integrations that use the Event Grid Event schema need to be modified to use the CloudEvent schema. Until that moment the work around as described in the repository kan be used.

# Getting started using localhost
1. Clone the repository.
2. Create an Event Grid Topic and store the topic name and topic key in the `local.settings.json` file with keys `EG-Topic` and `EG-Key`. 
3. Create an Application Insights Resource and add a key `APPINSIGHTS_INSTRUMENTATIONKEY` to the `local.settings.json` file.
5. Create a tunnel using [ngrok](https://ngrok.com/) to expose the function to the outside world. For example: `ngrok http 7071`.
4. Run the application.
6. Create an Event Grid Subscription using the Event Grid Event schema to the topic defined in step 2. Use a webhook as a target and the ngrok url as target url: `https://30c2406a223d.ngrok.io/runtime/webhooks/EventGrid?functionName=EventConsumer`.
7. Trigger the process by calling the `EventInitiator` method by issuing a GET request: `http://localhost:7071/api/EventInitiator`.

# Relevant Issues
https://github.com/Azure/azure-webjobs-sdk/issues/1731  
https://github.com/Azure/azure-sdk-for-net/issues/13272  
https://github.com/Azure/azure-sdk-for-net/issues/15466
