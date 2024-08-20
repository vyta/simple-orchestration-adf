using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace SimpleOrchestration
{
  public class Workbench
  {

    [Function(nameof(CreateWorkbenchStorage))]
    public static bool CreateWorkbenchStorage([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("CreateWorkbenchStorage");
      logger.LogInformation("Creating storage for {engagementId}.", engagementId);
      return true;
    }

    [Function(nameof(DeleteWorkbenchStorage))]
    public static bool DeleteWorkbenchStorage([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("DeleteWorkbenchStorage");
      logger.LogInformation("Deleting storage {engagementId}.", engagementId);
      return true;
    }
  }
}
