using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;

namespace SimpleOrchestration
{
  public class Engagements
  {
    private static HttpClient _httpClient = null!;

    public Engagements(IHttpClientFactory httpClientFactory){
      _httpClient = httpClientFactory.CreateClient("EngagementClient");
    }

    [Function(nameof(CreateEngagement))]
    public async Task<string?> CreateEngagement([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("CreateEngagement");
      logger.LogInformation("Creating engagement for {engagementId}.", engagementId);
      // Insert necessary logic to make calls to external services, handle tries and errors
      var obj = new {
        id = engagementId,
        name = engagementId,
        description = $"A new one for {engagementId}."
      };
      var response = await _httpClient.PostAsync("/lists", new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));
      var responseContent = await response.Content.ReadAsStringAsync();
      if (response.IsSuccessStatusCode && !String.IsNullOrEmpty(responseContent))
      {
        logger.LogInformation($"Post request successfully made to {_httpClient.BaseAddress}");
        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
        return responseObject?.id;
      }
      logger.LogError($"Post request to {_httpClient.BaseAddress} unsuccessful");
      return null;
    }

    [Function(nameof(DeleteEngagement))]
    public async Task<bool> DeleteEngagement([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("DeleteEngagement");
      logger.LogInformation("Deleting engagement {engagementId}.", engagementId);
      try
      {
        var response = await _httpClient.DeleteAsync($"/lists/{engagementId}");
        if (response.IsSuccessStatusCode)
        {
          logger.LogInformation($"Delete request successfully made to {_httpClient.BaseAddress}");
          return true;
        }
      }
      catch (Exception ex) {
        logger.LogError($"Delete request to {_httpClient.BaseAddress} unsuccessful. Exception: {ex.Message}.");
      }
      return false;
    }
  }
}
