using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using ApiMockServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Moq;
using Xunit;

namespace ApiMockServer.Tests.Services;

public class MockExecutionServiceTests
{
    private readonly Mock<IMockEndpointRepository> _endpointRepository;
    private readonly Mock<IMockScenarioRepository> _scenarioRepository;

    private readonly MockExecutionService _service;

    public MockExecutionServiceTests()
    {
        _endpointRepository = new Mock<IMockEndpointRepository>();
        _scenarioRepository = new Mock<IMockScenarioRepository>();

        _service = new MockExecutionService(
            _endpointRepository.Object,
            _scenarioRepository.Object
        );
    }

    private DefaultHttpContext CreateContext(string method)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        return context;
    }
    [Fact]
    public async Task ExecuteAsync_ShouldReturn404_WhenEndpointNotFound()
    {
        var context = CreateContext("GET");

        _endpointRepository
            .Setup(x => x.GetByMethodAndPathAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((MockEndpoint?)null);

        var result = await _service.ExecuteAsync(
            context,
            "api/products"
        );

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEndpointResponse_WhenScenarioDoesNotExist()
    {
        var context = CreateContext("GET");

        var endpoint = new MockEndpoint
        {
            Id = "1",
            Path = "/api/products",
            Method = "GET",
            StatusCode = 200,
            ResponseBody = "{\"message\":\"endpoint\"}",
            IsEnabled = true
        };

        _endpointRepository
            .Setup(x => x.GetByMethodAndPathAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(endpoint);

        _scenarioRepository
            .Setup(x => x.GetActiveScenarioAsync(endpoint.Id))
            .ReturnsAsync((MockScenario?)null);

        var result = await _service.ExecuteAsync(
            context,
            "api/products"
        );

        var response = Assert.IsType<ContentResult>(result);

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(endpoint.ResponseBody, response.Content);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnScenarioResponse_WhenScenarioExists()
    {
        var context = CreateContext("GET");

        var endpoint = new MockEndpoint
        {
            Id = "1",
            Path = "/api/products",
            Method = "GET",
            StatusCode = 200,
            ResponseBody = "{}",
            IsEnabled = true
        };

        var scenario = new MockScenario
        {
            MockEndpointId = endpoint.Id,
            StatusCode = 503,
            ResponseBody = "{\"message\":\"maintenance\"}"
        };

        _endpointRepository
            .Setup(x => x.GetByMethodAndPathAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(endpoint);

        _scenarioRepository
            .Setup(x => x.GetActiveScenarioAsync(endpoint.Id))
            .ReturnsAsync(scenario);

        var result = await _service.ExecuteAsync(
            context,
            "api/products"
        );

        var response = Assert.IsType<ContentResult>(result);

        Assert.Equal(503, response.StatusCode);
        Assert.Equal(scenario.ResponseBody, response.Content);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturn504_WhenTimeoutEnabled()
    {
        var context = CreateContext("GET");

        var endpoint = new MockEndpoint
        {
            Id = "1",
            Path = "/api/products",
            Method = "GET",
            IsEnabled = true
        };

        var scenario = new MockScenario
        {
            MockEndpointId = endpoint.Id,
            EnableTimeout = true,
            TimeoutDelay = 1
        };

        _endpointRepository
            .Setup(x => x.GetByMethodAndPathAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(endpoint);

        _scenarioRepository
            .Setup(x => x.GetActiveScenarioAsync(endpoint.Id))
            .ReturnsAsync(scenario);

        var result = await _service.ExecuteAsync(
            context,
            "api/products"
        );

        var response = Assert.IsType<ContentResult>(result);

        Assert.Equal(504, response.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailureResponse_WhenFailureRateIs100()
    {
        var context = CreateContext("GET");

        var endpoint = new MockEndpoint
        {
            Id = "1",
            Path = "/api/products",
            Method = "GET",
            StatusCode = 200,
            ResponseBody = "{\"message\":\"success\"}",
            IsEnabled = true
        };

        var scenario = new MockScenario
        {
            MockEndpointId = endpoint.Id,
            EnableRandomFailure = true,
            FailureRate = 100,
            StatusCode = 500,
            ResponseBody = "{\"message\":\"Random Failure Simulated\"}"
        };

        _endpointRepository
            .Setup(x => x.GetByMethodAndPathAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(endpoint);

        _scenarioRepository
            .Setup(x => x.GetActiveScenarioAsync(endpoint.Id))
            .ReturnsAsync(scenario);

        var result = await _service.ExecuteAsync(
            context,
            "api/products"
        );

        var response = Assert.IsType<ContentResult>(result);

        Assert.Equal(500, response.StatusCode);
        Assert.Equal(
            "{\"message\":\"Random Failure Simulated\"}",
            response.Content
        );
    }

    [Fact]
    public async Task ExecuteAsync_ShouldWaitForConfiguredDelay()
    {
        var context = CreateContext("GET");

        var endpoint = new MockEndpoint
        {
            Id = "1",
            Path = "/api/products",
            Method = "GET",
            StatusCode = 200,
            ResponseBody = "{\"message\":\"success\"}",
            IsEnabled = true
        };

        var scenario = new MockScenario
        {
            MockEndpointId = endpoint.Id,
            Delay = 100,
            StatusCode = 200,
            ResponseBody = "{\"message\":\"Delayed Response\"}"
        };

        _endpointRepository
            .Setup(x => x.GetByMethodAndPathAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(endpoint);

        _scenarioRepository
            .Setup(x => x.GetActiveScenarioAsync(endpoint.Id))
            .ReturnsAsync(scenario);

        var stopwatch = Stopwatch.StartNew();

        await _service.ExecuteAsync(
            context,
            "api/products"
        );

        stopwatch.Stop();

        Assert.True(
            stopwatch.ElapsedMilliseconds >= 100,
            $"Expected at least 100ms delay but got {stopwatch.ElapsedMilliseconds}ms."
        );
    }
}