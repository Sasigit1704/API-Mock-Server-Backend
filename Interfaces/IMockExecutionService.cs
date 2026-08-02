using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Interfaces;

public interface IMockExecutionService
{
    Task<IActionResult> ExecuteAsync(HttpContext context, string dynamicPath);
}