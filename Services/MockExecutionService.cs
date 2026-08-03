using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ApiMockServer.Models;
using System.Diagnostics;

namespace ApiMockServer.Services;

public class MockExecutionService : IMockExecutionService
{
    private readonly IMockEndpointRepository _endpointRepository;
    private readonly IMockScenarioRepository _scenarioRepository;
    private readonly IRequestHistoryService _requestHistoryService;
    private readonly Random _random = new();

    public MockExecutionService(
        IMockEndpointRepository endpointRepository,
        IMockScenarioRepository scenarioRepository,
        IRequestHistoryService requestHistoryService)
    {
        _endpointRepository = endpointRepository;
        _scenarioRepository = scenarioRepository;
        _requestHistoryService = requestHistoryService;
    }

    public async Task<IActionResult> ExecuteAsync(
    HttpContext context,
    string dynamicPath)
    {
        var stopwatch = Stopwatch.StartNew();

        var requestPath = "/" + Uri.UnescapeDataString(dynamicPath);

        var method = context.Request.Method;

        var endpoint = await _endpointRepository
            .GetByMethodAndPathAsync(method, requestPath);

        IActionResult result;

        MockScenario? scenario = null;

        if (endpoint == null)
        {
            result = new NotFoundObjectResult(new
            {
                message = "Mock endpoint not found."
            });
        }
        else
        {
            scenario = await _scenarioRepository
                .GetActiveScenarioAsync(endpoint.Id);

            if (scenario != null && scenario.Delay > 0)
            {
                await Task.Delay(scenario.Delay);
            }

            if (scenario != null && scenario.EnableTimeout)
            {
                await Task.Delay(scenario.TimeoutDelay);

                result = new ObjectResult(new
                {
                    message = "Gateway Timeout"
                })
                {
                    StatusCode = StatusCodes.Status504GatewayTimeout
                };
            }
            else if (scenario != null &&
                    scenario.EnableRandomFailure)
            {
                int number = _random.Next(1, 101);

                if (number <= scenario.FailureRate)
                {
                    result = new ObjectResult(new
                    {
                        message = "Random Failure Simulated"
                    })
                    {
                        StatusCode = 500
                    };
                }
                else
                {
                    result = new ContentResult
                    {
                        Content = scenario.ResponseBody,
                        StatusCode = scenario.StatusCode,
                        ContentType = "application/json"
                    };
                }
            }
            else if (scenario == null)
            {
                result = new ContentResult
                {
                    Content = endpoint.ResponseBody,
                    StatusCode = endpoint.StatusCode,
                    ContentType = "application/json"
                };
            }
            else
            {
                result = new ContentResult
                {
                    Content = scenario.ResponseBody,
                    StatusCode = scenario.StatusCode,
                    ContentType = "application/json"
                };
            }
        }

        stopwatch.Stop();

        int statusCode = result switch
        {
            ObjectResult obj => obj.StatusCode ?? 200,
            ContentResult content => content.StatusCode ?? 200,
            StatusCodeResult status => status.StatusCode,
            _ => 200
        };

        await _requestHistoryService.CreateAsync(new RequestLog
        {
            Method = method,
            Path = requestPath,
            StatusCode = statusCode,
            RequestTime = DateTime.UtcNow,
            ResponseTimeMs = stopwatch.ElapsedMilliseconds,
            MockEndpointId = endpoint?.Id,
            MockScenarioId = scenario?.Id,
            IPAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers["User-Agent"].ToString()
        });

        return result;
    }
}