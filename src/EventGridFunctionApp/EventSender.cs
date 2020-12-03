using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.DataContracts;

namespace EventGridFunctionApp
{
    public static class EventSender
    {
        [FunctionName("EventSender")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var topicEndpoint = Environment.GetEnvironmentVariable("EG-Topic");
            var topicKey = Environment.GetEnvironmentVariable("EG-Key");
            var topicCredentials = new TopicCredentials(topicKey);
            var topicHostname = new Uri(topicEndpoint).Host;

            var client = new EventGridClient(topicCredentials);

            var telemetry = req.HttpContext.Features.Get<RequestTelemetry>();
            var activity = Activity.Current;

            var eventGridEvent = new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventTime = DateTime.UtcNow,
                EventType = "MyEventType",
                Subject = "MyEventSubject",
                Data = new EventData { ThrowError = req.Query.ContainsKey("Error"), OperationId = telemetry.Context.Operation.Id, ParentOperationId = activity.SpanId.ToString() },
                DataVersion = "1.0"
            };

            await client.PublishEventsAsync(topicHostname, new[] { eventGridEvent });

            log.LogInformation("Send event with id {EventId} and timestamp {EventDateTime}", eventGridEvent.Id, eventGridEvent.EventTime);

            return new OkResult();
        }
    }
}
