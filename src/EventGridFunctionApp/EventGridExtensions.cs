using System;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json.Linq;

namespace EventGridFunctionApp
{
    public static class EventGridExtensions
    {
        public static T As<T>(this EventGridEvent eventGridEvent)
        {
            if (eventGridEvent == null)
                throw new ArgumentNullException(nameof(eventGridEvent));

            return ((JObject)eventGridEvent.Data).ToObject<T>();
        }
    }
}