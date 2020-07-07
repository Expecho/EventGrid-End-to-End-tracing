// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace EventGridFunctionApp
{
    public class EventConsumer
    {
        private readonly TelemetryClient _telemetryClient;

        public EventConsumer(TelemetryConfiguration telemetryConfiguration)
        {
            _telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [FunctionName("EventConsumer")]
        public void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation("Received event with id {EventId} and timestamp {EventDateTime}", eventGridEvent.Id, eventGridEvent.EventTime);
            var eventData = eventGridEvent.As<EventData>();

            using (var operation = _telemetryClient.StartOperation<DependencyTelemetry>($"{executionContext.FunctionName}", eventData.OperationId, eventData.ParentOperationId))
            {
                operation.Telemetry.Success = false;
                operation.Telemetry.Type = "EventHandler";
                operation.Telemetry.Properties["EventId"] = eventGridEvent.Id;

                // Any outgoing calls during this invocation are tracked as dependencies belonging to this operation

                log.LogInformation("Handled event with id {EventId} and timestamp {EventDateTime}", eventGridEvent.Id,
                    eventGridEvent.EventTime);

                operation.Telemetry.Success = true;
            }
        }
    }
}
