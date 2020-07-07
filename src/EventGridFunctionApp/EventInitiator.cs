using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EventGridFunctionApp
{
    public static class EventInitiator
    {
        [FunctionName("EventInitiator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await new HttpClient().GetStringAsync("http://localhost:7071/api/EventSender");

            return new OkResult();
        }
    }
}
