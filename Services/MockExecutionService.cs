using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Services;

public class MockExecutionService : IMockExecutionService
{
    private readonly IMockEndpointRepository _endpointRepository;
    private readonly IMockScenarioRepository _scenarioRepository;
    private readonly Random _random = new();

    public MockExecutionService(
        IMockEndpointRepository endpointRepository,
        IMockScenarioRepository scenarioRepository)
    {
        _endpointRepository = endpointRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<IActionResult> ExecuteAsync(
        HttpContext context,
        string dynamicPath)
    {
        

        var requestPath = "/" + Uri.UnescapeDataString(dynamicPath);

        var method = context.Request.Method;
        
        var endpoint = await _endpointRepository
            .GetByMethodAndPathAsync(method, requestPath);

        if (endpoint == null)
        {
            return new NotFoundObjectResult(new
            {
                message = "Mock endpoint not found."
            });
        }

        var scenario = await _scenarioRepository
            .GetActiveScenarioAsync(endpoint.Id);

        if (scenario?.Delay > 0)
        {
            await Task.Delay(scenario.Delay);
        }

        if (scenario?.EnableTimeout == true)
        {
            await Task.Delay(scenario.TimeoutDelay);

            return new ContentResult
            {
                Content = """
                {
                    "message":"Gateway Timeout"
                }
                """,
                StatusCode = StatusCodes.Status504GatewayTimeout,
                ContentType = "application/json"
            };
        }

        if (scenario?.EnableRandomFailure == true)
        {
            int number = _random.Next(1,101);

            if(number <= scenario.FailureRate)
            {
                return new ContentResult
                {
                    Content = scenario.ResponseBody,
                    StatusCode = scenario.StatusCode,
                    ContentType = "application/json"
                };
            }
        }

        if (scenario == null)
        {
            return new ContentResult
            {
                Content = endpoint.ResponseBody,
                StatusCode = endpoint.StatusCode,
                ContentType = "application/json"
            };
        }

        return new ContentResult
        {
            Content = scenario.ResponseBody,
            StatusCode = scenario.StatusCode,
            ContentType = "application/json"
        };
    }
}