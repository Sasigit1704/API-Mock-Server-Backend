using ApiMockServer.Interfaces;
using ApiMockServer.Middleware;
using ApiMockServer.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using System.Net;
using Xunit;

namespace ApiMockServer.Tests.Middleware;

public class MockEndpointMiddlewareTests
{
    private readonly Mock<IMockEndpointService> _endpointService;
    private readonly Mock<IMockScenarioService> _scenarioService;
    private readonly Mock<IRequestHistoryService> _historyService;

    public MockEndpointMiddlewareTests()
    {
        _endpointService = new Mock<IMockEndpointService>();

        _scenarioService = new Mock<IMockScenarioService>();

        _historyService = new Mock<IRequestHistoryService>();
    }

    private TestServer CreateServer()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(_endpointService.Object);
                services.AddSingleton(_scenarioService.Object);
                services.AddSingleton(_historyService.Object);
            })
            .Configure(app =>
            {
                app.UseMiddleware<MockEndpointMiddleware>();

                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Next Middleware");
                });
            });

        return new TestServer(builder);
    }

    [Fact]
    public async Task UnknownEndpoint_ShouldCallNextMiddleware()
    {
        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync((MockEndpoint?)null);

        using var server = CreateServer();

        var client = server.CreateClient();

        var response = await client.GetAsync("/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Be("Next Middleware");
    }

    [Fact]
    public async Task EndpointExists_ShouldReturnMockEndpointResponse()
    {
        // Arrange
        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync(new MockEndpoint
            {
                Id = "endpoint1",
                Method = "GET",
                Path = "/users",
                StatusCode = 200,
                ResponseBody = """
                {
                    "message":"Hello"
                }
                """
            });

        _scenarioService
            .Setup(x => x.GetActiveScenarioAsync("endpoint1"))
            .ReturnsAsync((MockScenario?)null);

        using var server = CreateServer();

        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content.Headers.ContentType!.MediaType
            .Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Hello");
    }

    [Fact]
    public async Task ActiveScenario_ShouldReturnScenarioResponse()
    {
        // Arrange
        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync(new MockEndpoint
            {
                Id = "endpoint1",
                Method = "GET",
                Path = "/users",
                StatusCode = 200,
                ResponseBody = "{\"message\":\"Endpoint\"}"
            });

        _scenarioService
            .Setup(x => x.GetActiveScenarioAsync("endpoint1"))
            .ReturnsAsync(new MockScenario
            {
                Id = "scenario1",
                MockEndpointId = "endpoint1",
                ScenarioName = "Success",
                StatusCode = 201,
                ResponseBody = "{\"message\":\"Scenario\"}",
                IsActive = true
            });

        using var server = CreateServer();
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Scenario");
    }

    [Fact]
    public async Task TimeoutScenario_ShouldReturn408()
    {
        // Arrange

        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync(new MockEndpoint
            {
                Id = "endpoint1",
                Method = "GET",
                Path = "/users",
                StatusCode = 200,
                ResponseBody = "{\"message\":\"Endpoint\"}"
            });

        _scenarioService
            .Setup(x => x.GetActiveScenarioAsync("endpoint1"))
            .ReturnsAsync(new MockScenario
            {
                Id = "scenario1",
                MockEndpointId = "endpoint1",
                ScenarioName = "Timeout",
                EnableTimeout = true,
                TimeoutDelay = 1
            });

        using var server = CreateServer();

        var client = server.CreateClient();

        // Act

        var response = await client.GetAsync("/users");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.RequestTimeout);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Request Timeout");
    }

    [Fact]
    public async Task RandomFailureScenario_ShouldReturn500()
    {
        // Arrange

        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync(new MockEndpoint
            {
                Id = "endpoint1",
                Method = "GET",
                Path = "/users",
                StatusCode = 200,
                ResponseBody = "{\"message\":\"Endpoint\"}"
            });

        _scenarioService
            .Setup(x => x.GetActiveScenarioAsync("endpoint1"))
            .ReturnsAsync(new MockScenario
            {
                Id = "scenario1",
                MockEndpointId = "endpoint1",
                EnableRandomFailure = true,
                FailureRate = 100
            });

        using var server = CreateServer();

        var client = server.CreateClient();

        // Act

        var response = await client.GetAsync("/users");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Random Failure");
    }

    [Fact]
    public async Task DelayScenario_ShouldStillReturnEndpointResponse()
    {
        // Arrange

        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync(new MockEndpoint
            {
                Id = "endpoint1",
                Method = "GET",
                Path = "/users",
                StatusCode = 200,
                ResponseBody = "{\"message\":\"Endpoint\"}"
            });

        _scenarioService
            .Setup(x => x.GetActiveScenarioAsync("endpoint1"))
            .ReturnsAsync(new MockScenario
            {
                Id = "scenario1",
                MockEndpointId = "endpoint1",
                Delay = 1,
                StatusCode = 201,
                ResponseBody = "{\"message\":\"Delayed\"}"
            });

        using var server = CreateServer();

        var client = server.CreateClient();

        // Act

        var response = await client.GetAsync("/users");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Delayed");
    }

    [Fact]
    public async Task Exception_ShouldReturn500()
    {
        // Arrange

        _endpointService
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ThrowsAsync(new Exception("Database Failure"));

        using var server = CreateServer();

        var client = server.CreateClient();

        // Act

        var response = await client.GetAsync("/users");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Internal Server Error");
    }
}