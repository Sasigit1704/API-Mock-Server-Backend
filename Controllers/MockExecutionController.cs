using ApiMockServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMockServer.Controllers;

[ApiController]
[Route("api/mock")]
public class MockExecutionController : ControllerBase
{
    private readonly IMockExecutionService _executionService;

    public MockExecutionController(
        IMockExecutionService executionService)
    {
        _executionService = executionService;
    }

    [Route("{**dynamicPath}")]
    [AcceptVerbs(
        "GET",
        "POST",
        "PUT",
        "PATCH",
        "DELETE"
    )]
    public async Task<IActionResult> Execute(
        string dynamicPath)
    {
        return await _executionService.ExecuteAsync(
            HttpContext,
            dynamicPath
        );
    }
}