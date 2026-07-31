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
            if (endpoint == null) {
                await _next(context);
                return;
            }
            var scenario = await mockScenarioService.GetActiveScenarioAsync(endpoint.Id);
            if (scenario!= null) {
                if (scenario.Delay > 0) {
                    await Task.Delay(scenario.Delay);
                }
                if (scenario.EnableTimeout)
                {
                    await Task.Delay(scenario.TimeoutDelay);

                    stopwatch.Stop();

                    await SaveRequestHistoryAsync(requestHistoryService, method, path, StatusCodes.Status408RequestTimeout, stopwatch.ElapsedMilliseconds, endpoint.Id, scenario.Id);
                
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

                        await SaveRequestHistoryAsync(requestHistoryService, method, path, StatusCodes.Status500InternalServerError, stopwatch.ElapsedMilliseconds, endpoint.Id, scenario.Id);
                

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

                await SaveRequestHistoryAsync(requestHistoryService, method, path, scenario.StatusCode, stopwatch.ElapsedMilliseconds, endpoint.Id, scenario.Id);
                await WriteResponseAsync(context, scenario.StatusCode, scenario.ResponseBody);
                return;
            }
            stopwatch.Stop();

            await SaveRequestHistoryAsync(requestHistoryService, method, path, endpoint.StatusCode, stopwatch.ElapsedMilliseconds, endpoint.Id, null);
            await WriteResponseAsync(context, endpoint.StatusCode, endpoint.ResponseBody);
            return;
        }

//HELPER METHODS

        private static async Task SaveRequestHistoryAsync(
            IRequestHistoryService requestHistoryService,
            string method,
            string path,
            int statusCode,
            long responseTimeMs,
            string endpointId,
            string? scenarioId)
        {
            await requestHistoryService.CreateAsync(new RequestHistory
            {
                Method = method,
                Path = path,
                StatusCode = statusCode,
                RequestTime = DateTime.UtcNow,
                ResponseTimeMs = responseTimeMs,
                MockEndpointId = endpointId,
                MockScenarioId = scenarioId
            });
        }
        
        private static async Task WriteResponseAsync(HttpContext context, int statusCode, string responseBody)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseBody);
        }

    }

}