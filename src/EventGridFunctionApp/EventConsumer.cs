// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=EventConsumer

using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace EventGridFunctionApp
{
    public class EventConsumer
    {
        private readonly TelemetryClient _telemetryClient;

        public EventConsumer(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryClient = new TelemetryClient(telemetryConfiguration)
            {
                InstrumentationKey = telemetryConfiguration.InstrumentationKey
            };
        }

        [FunctionName("EventConsumer")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, ExecutionContext executionContext)
        {
            // The azure function runtime creates request telemetry for each invocation. When an EventGridTrigger is used this telemetry
            // cannot be accessed. Instead, we can add additional data to the custom properties of the RequestTelemetry by adding tags to the current activity
            Activity.Current
                .AddTag("EventId", eventGridEvent.Id)
                .AddTag("EventDateTime", eventGridEvent.EventTime.ToString(CultureInfo.InvariantCulture));

            // If we want to add (the same) data to telemetry in the same operation we can add baggage to the current activity 
            Activity.Current
                .AddBaggage("EventId", eventGridEvent.Id)
                .AddBaggage("EventType", eventGridEvent.EventType);

            log.LogInformation("Received event. Data: {Data}", eventGridEvent.Data);

            var eventData = eventGridEvent.As<EventData>();

            using (var operation = _telemetryClient.StartOperation<DependencyTelemetry>($"{executionContext.FunctionName}", eventData.OperationId, eventData.ParentOperationId))
            {
                operation.Telemetry.Success = false;
                operation.Telemetry.Type = "EventHandler";
                operation.Telemetry.Properties["EventId"] = eventGridEvent.Id;

                // Any outgoing calls during this invocation are tracked as dependencies belonging to this operation
                await new HttpClient().GetStringAsync("http://blank.org/");

                log.LogInformation("Handled event");

                operation.Telemetry.Success = true;
            }
        }
    }
}
