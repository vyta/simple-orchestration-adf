using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SimpleOrchestration
{
  public class Engagements
  {
    private static HttpClient _httpClient = null!;

    public Engagements(IHttpClientFactory httpClientFactory){
      _httpClient = httpClientFactory.CreateClient("EngagementClient");
    }

    [Function(nameof(CreateEngagement))]
    public async Task<bool> CreateEngagement([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("CreateEngagement");
      logger.LogInformation("Creating engagement for {engagementId}.", engagementId);
      // Insert necessary logic to make calls to external services, handle tries and errors
      var response = await _httpClient.GetAsync("/");
      if (response.IsSuccessStatusCode)
      {
        logger.LogInformation($"HttpRequest successfully made to {_httpClient.BaseAddress}");
        return true;
      }
      logger.LogError($"HttpRequest to {_httpClient.BaseAddress} unsuccessful");
      return false;
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
