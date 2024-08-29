using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace SimpleOrchestration
{
  public class Workbench
  {
    private static HttpClient _httpClient = null!;

    public Workbench(IHttpClientFactory httpClientFactory)
    {
      _httpClient = httpClientFactory.CreateClient("WorkbenchClient");
    }

    [Function(nameof(CreateWorkbenchStorage))]
    public async Task<bool> CreateWorkbenchStorage([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("CreateWorkbenchStorage");
      logger.LogInformation("Creating storage for {engagementId}.", engagementId);
      try
      {
        var response = await withRetry(logger).ExecuteAsync(async token => await _httpClient.GetAsync("/", token));

        if (response.IsSuccessStatusCode)
        {
          logger.LogInformation($"HttpRequest successfully made to {_httpClient.BaseAddress}");
          return true;
        }
      }
      catch (Exception ex)
      {
        logger.LogError($"HttpRequest to {_httpClient.BaseAddress} unsuccessful. Exception: {ex.Message}.");
      }
      return false;
    }

    [Function(nameof(DeleteWorkbenchStorage))]
    public static bool DeleteWorkbenchStorage([ActivityTrigger] string engagementId, FunctionContext executionContext)
    {
      ILogger logger = executionContext.GetLogger("DeleteWorkbenchStorage");
      logger.LogInformation("Deleting storage {engagementId}.", engagementId);
      return true;
    }

    private ResiliencePipeline withRetry(ILogger logger)
    {
      return new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
          // Customize and configure the retry logic.
          BackoffType = DelayBackoffType.Exponential,
          MaxRetryAttempts = 5,
          UseJitter = true,
          OnRetry = (args) =>
        {
          logger.LogWarning($"Retry {args.AttemptNumber} encountered an error: {args.Outcome.Exception?.Message}. Waiting {args.RetryDelay} before next retry.");
          return ValueTask.CompletedTask;
        }
        })
        .Build();
    }
  }
}
