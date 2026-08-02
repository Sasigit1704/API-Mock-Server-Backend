using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Services;

public class MockExecutionService : IMockExecutionService
{
    private readonly IMockEndpointRepository _endpointRepository;
    private readonly IMockScenarioRepository _scenarioRepository;

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

        if (scenario == null)
        {
            return new ObjectResult(endpoint.ResponseBody)
            {
                StatusCode = endpoint.StatusCode
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