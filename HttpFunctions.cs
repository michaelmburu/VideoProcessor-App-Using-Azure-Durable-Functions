using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VideoProcessor
{
    public static class HttpFunctions
    {
        [FunctionName("Function1_HttpStart")]
        public static async Task<IActionResult> HttpStart(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
          [DurableClient] IDurableOrchestrationClient starter,
          ILogger log)
        {
            string video = req.GetQueryParameterDictionary()["video"];
            if(video == null)
            {
                return new BadRequestObjectResult("Please pass the video location of the query string");
            }
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ProcessVideoOrchestrator", null, video);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
