using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ApiMockServer.Middleware {

    public class MockEndpointMiddleware {
        private readonly RequestDelegate _next;

        public MockEndpointMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMockEndpointService mockEndpointService, IMockScenarioService mockScenarioService) {
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? String.Empty;
            var endpoint = await mockEndpointService.GetByMethodAndPathAsync(method, path);
            if (endpoint!= null) {
                var scenario = await mockScenarioService.GetActiveScenarioAsync(endpoint.Id);
                if (scenario!= null) {
                    if (scenario.Delay > 0) {
                        await Task.Delay(scenario.Delay);
                    }
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = scenario.StatusCode;
                    await context.Response.WriteAsync(scenario.ResponseBody);
                    return;
            }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = endpoint.StatusCode;
                await context.Response.WriteAsync(endpoint.ResponseBody);
                return;
            }
            await _next(context);
        }

    }

}