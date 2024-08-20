using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace SimpleOrchestration
{
  public class Engagements
  {

    [Function(nameof(CreateEngagement))]
    public static bool CreateEngagement([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("CreateEngagement");
      logger.LogInformation("Creating engagement for {engagementId}.", engagementId);
      // Insert necessary logic to make calls to external services, handle tries and errors
      return true;
    }

    [Function(nameof(DeleteEngagement))]
    public static bool DeleteEngagement([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("DeleteEngagement");
      logger.LogInformation("Deleting engagement {engagementId}.", engagementId);
      return true;
    }
  }
}
