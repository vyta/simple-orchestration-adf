using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace SimpleOrchestration
{
    public class Orchestrator
    {
        [Function("RunOrchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(RunOrchestrator));
            logger.LogInformation("Starting orchestrator...");

            var outputs = new List<string>();

            string? deliverable = context.GetInput<string>();

            // Define steps and their compensations
            var steps = new List<(string ActivityName, string CompensationActivityName)>
            {
                ("CreateEngagement", "DeleteEngagement"),
                ("CreateWorkbenchStorage","DeleteWorkbenchStorage")
            };

            int completed = 0;
            try
            {
                foreach (var step in steps)
                {
                    // Can additionally add retries here with retry options for the activity itself
                    if (await context.CallActivityAsync<bool>(step.ActivityName, deliverable))
                    {
                        completed++;
                        outputs.Add($"{step.ActivityName} completed successfully.");
                    }
                    else
                    {
                        outputs.Add($"{step.ActivityName} failed.");
                        throw new Exception($"Unable to complete {step.ActivityName}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                logger.LogInformation("Starting to execute compensation...");
                foreach (var completedStep in steps.Take(completed).Reverse())
                {
                    // if for some reason an activity function is flaky, leverage automatic retry on call
                    var options = TaskOptions.FromRetryPolicy(new RetryPolicy(
                        maxNumberOfAttempts: 3,
                        firstRetryInterval: TimeSpan.FromSeconds(5)));
                        
                    if (await context.CallActivityAsync<bool>(completedStep.CompensationActivityName, deliverable, options: options))
                    {
                        outputs.Add($"{completedStep.ActivityName} rolled back with {completedStep.CompensationActivityName} successfully.");
                    }
                    else
                    {
                        outputs.Add($"{completedStep.CompensationActivityName} failed.");
                        throw new Exception($"Unable to complete {completedStep.CompensationActivityName} ");
                    }
                }
                outputs.Add("Orchestration failed");
            }

            return outputs;
        }

    }
}
