using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace SimpleOrchestration
{
  public class SimpleTrigger
  {
    [Function("SimplesOrchestration_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("SimplesOrchestration_HttpStart");

      // Function input comes from the request content.
      string instanceId = req.Query["id"] != null ? req.Query["id"] : Guid.NewGuid().ToString("N");
      await client.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator.RunOrchestrator), instanceId, new StartOrchestrationOptions() { InstanceId = instanceId });

      logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

      // Returns an HTTP 202 response with an instance management payload.
      // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
      return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }

  }
}
