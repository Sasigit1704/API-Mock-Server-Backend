using System;
using System.Diagnostics;
using ApiMockServer.Models;
using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ApiMockServer.Middleware {

    public class MockEndpointMiddleware {
        private readonly RequestDelegate _next;
        private static readonly Random _random = new();

        public MockEndpointMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(
        HttpContext context,
        IMockEndpointService mockEndpointService,
        IMockScenarioService mockScenarioService,
        IRequestHistoryService requestHistoryService)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? String.Empty;
            var endpoint = await mockEndpointService.GetByMethodAndPathAsync(method, path);
            if (endpoint!= null) {
                var scenario = await mockScenarioService.GetActiveScenarioAsync(endpoint.Id);
                if (scenario!= null) {
                    if (scenario.Delay > 0) {
                        await Task.Delay(scenario.Delay);
                    }
                    if (scenario.EnableTimeout)
                    {
                        await Task.Delay(scenario.TimeoutDelay);

                        stopwatch.Stop();

                        await requestHistoryService.CreateAsync(new RequestHistory
                        {
                            Method = method,
                            Path = path,
                            StatusCode = StatusCodes.Status408RequestTimeout,
                            RequestTime = DateTime.UtcNow,
                            ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                            MockEndpointId = endpoint.Id,
                            MockScenarioId = scenario.Id
                        });

                        context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync("""
                        {
                            "success": false,
                            "message": "Request Timeout Simulation",
                            "statusCode": 408
                        }
                        """);

                        return;
                    }
                    if (scenario.EnableRandomFailure)
                    {
                        int randomNumber = _random.Next(1, 101);

                        if (randomNumber <= scenario.FailureRate)
                        {
                            stopwatch.Stop();

                            await requestHistoryService.CreateAsync(new RequestHistory
                            {
                                Method = method,
                                Path = path,
                                StatusCode = StatusCodes.Status500InternalServerError,
                                RequestTime = DateTime.UtcNow,
                                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                                MockEndpointId = endpoint.Id,
                                MockScenarioId = scenario.Id
                            });

                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsync("""
                            {
                                "success": false,
                                "message": "Random Failure Simulation",
                                "statusCode": 500
                            }
                            """);

                            return;
                        }
                    }
                   
                    stopwatch.Stop();

                    await requestHistoryService.CreateAsync(new RequestHistory
                    {
                        Method = method,
                        Path = path,
                        StatusCode = scenario.StatusCode,
                        RequestTime = DateTime.UtcNow,
                        ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                        MockEndpointId = endpoint.Id,
                        MockScenarioId = scenario.Id
                    });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = scenario.StatusCode;
                    await context.Response.WriteAsync(scenario.ResponseBody);
                    return;
                }
                stopwatch.Stop();

                await requestHistoryService.CreateAsync(new RequestHistory
                {
                    Method = method,
                    Path = path,
                    StatusCode = endpoint.StatusCode,
                    RequestTime = DateTime.UtcNow,
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                    MockEndpointId = endpoint.Id,
                    MockScenarioId = null
                });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = endpoint.StatusCode;
                await context.Response.WriteAsync(endpoint.ResponseBody);
                return;
            }
            await _next(context);
        }

    }

}