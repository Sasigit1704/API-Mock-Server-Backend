using ApiMockServer.Controllers;
using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Controllers;

public class MockScenarioControllerTests
{
    private readonly Mock<IMockScenarioService> _serviceMock;

    private readonly MockScenariosController _controller;

    public MockScenarioControllerTests()
    {
        _serviceMock = new Mock<IMockScenarioService>();

        _controller = new MockScenariosController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var scenarios = new List<MockScenario>
        {
            new MockScenario
            {
                Id="1",
                ScenarioName="Success"
            }
        };

        _serviceMock
            .Setup(x=>x.GetAllAsync())
            .ReturnsAsync(scenarios);

        var result = await _controller.GetAll();

        var ok = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        ok.Value.Should().BeEquivalentTo(scenarios);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk()
    {
        var scenario = new MockScenario
        {
            Id="1",
            ScenarioName="Success"
        };

        _serviceMock
            .Setup(x=>x.GetByIdAsync("1"))
            .ReturnsAsync(scenario);

        var result = await _controller.GetById("1");

        var ok = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        ok.Value.Should().Be(scenario);
    }

    [Fact]
    public async Task Create_ShouldReturnOk()
    {
        var dto = new CreateMockScenarioDTO
        {
            ScenarioName="Success",
            MockEndpointId="endpoint1",
            StatusCode=200,
            ResponseBody="{}"
        };

        var result = await _controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        _serviceMock
            .Setup(x=>x.DeleteAsync("1"))
            .Returns(Task.CompletedTask);

        var result = await _controller.Delete("1");

        result.Should().BeOfType<OkObjectResult>();
    }
}