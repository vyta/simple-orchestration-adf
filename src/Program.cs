using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        // Configures a named EngagementClient http with StandardResilienceHandler. 
        // For more information: https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience?tabs=dotnet-cli#standard-resilience-handler-defaults
        services.AddHttpClient("EngagementClient", client =>
        {
            // configure client here. Ex: client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "<token>");
            client.BaseAddress = new Uri("https://app-api-xk37pjmwrig4e.azurewebsites.net/");
        })
        .AddStandardResilienceHandler();

        services.AddHttpClient("WorkbenchClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost");
        });
    })
    .Build();

host.Run();
