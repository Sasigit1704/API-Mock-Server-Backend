using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ApiMockServer.Middleware {

    public class MockEndpointMiddleware {
        private readonly RequestDelegate _next;
        private readonly IMockEndpointService _mockEndpointService;

        public MockEndpointMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMockEndpointService mockEndpointService) {
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? String.Empty;
            var endpoint = await mockEndpointService.GetByMethodAndPathAsync(method, path);
            if (endpoint != null) {
                context.Response.StatusCode = endpoint.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(endpoint.ResponseBody);
                return;
            }
            await _next(context);
        }

    }

}