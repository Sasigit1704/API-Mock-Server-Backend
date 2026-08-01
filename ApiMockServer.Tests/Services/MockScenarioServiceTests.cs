using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using ApiMockServer.Services;

using FluentAssertions;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Services;

public class MockScenarioServiceTests
{
    private readonly Mock<IMockScenarioRepository> _scenarioRepositoryMock;

    private readonly Mock<IMockEndpointRepository> _endpointRepositoryMock;

    private readonly MockScenarioService _service;

    public MockScenarioServiceTests()
    {
        _scenarioRepositoryMock =
            new Mock<IMockScenarioRepository>();

        _endpointRepositoryMock =
            new Mock<IMockEndpointRepository>();

        _service = new MockScenarioService(
            _scenarioRepositoryMock.Object,
            _endpointRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValidScenario_ShouldCreateScenario()
    {
        // Arrange

        var dto = new CreateMockScenarioDTO
        {
            MockEndpointId = "endpoint-id",
            ScenarioName = "Success",
            StatusCode = 200,
            ResponseBody = "{}",
            Delay = 0,
            IsActive = true,
            EnableRandomFailure = false,
            FailureRate = 0,
            EnableTimeout = false,
            TimeoutDelay = 0
        };

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("endpoint-id"))
            .ReturnsAsync(new MockEndpoint());

        _scenarioRepositoryMock
            .Setup(x => x.GetByMockEndpointIdAsync("endpoint-id"))
            .ReturnsAsync(new List<MockScenario>());

        // Act

        await _service.CreateAsync(dto);

        // Assert

        _scenarioRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<MockScenario>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEndpointDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange

        var dto = new CreateMockScenarioDTO
        {
            MockEndpointId = "endpoint-id",
            ScenarioName = "Success",
            StatusCode = 200,
            ResponseBody = "{}",
            Delay = 0,
            IsActive = true,
            EnableRandomFailure = false,
            FailureRate = 0,
            EnableTimeout = false,
            TimeoutDelay = 0
        };

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("endpoint-id"))
            .ReturnsAsync((MockEndpoint?)null);

        // Act

        Func<Task> action = async () =>
            await _service.CreateAsync(dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*MockEndpoint does not exist*");
    }

    [Fact]
    public async Task CreateAsync_WhenFailureRateIsInvalid_ShouldThrowArgumentException()
    {
        // Arrange

        var dto = new CreateMockScenarioDTO
        {
            MockEndpointId = "endpoint-id",
            ScenarioName = "Failure",
            StatusCode = 500,
            ResponseBody = "{}",
            Delay = 0,
            IsActive = true,
            EnableRandomFailure = true,
            FailureRate = 120,
            EnableTimeout = false,
            TimeoutDelay = 0
        };

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("endpoint-id"))
            .ReturnsAsync(new MockEndpoint());

        // Act

        Func<Task> action = async () =>
            await _service.CreateAsync(dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Failure Rate must be between 0 and 100*");
    }

    [Fact]
    public async Task CreateAsync_WhenTimeoutDelayIsNegative_ShouldThrowArgumentException()
    {
        // Arrange

        var dto = new CreateMockScenarioDTO
        {
            MockEndpointId = "endpoint-id",
            ScenarioName = "Timeout",
            StatusCode = 200,
            ResponseBody = "{}",
            Delay = 0,
            IsActive = true,
            EnableRandomFailure = false,
            FailureRate = 0,
            EnableTimeout = true,
            TimeoutDelay = -10
        };

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("endpoint-id"))
            .ReturnsAsync(new MockEndpoint());

        // Act

        Func<Task> action = async () =>
            await _service.CreateAsync(dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Timeout Delay cannot be negative*");
    }

    [Fact]
    public async Task DeleteAsync_WhenScenarioExists_ShouldDeleteScenario()
    {
        // Arrange

        // Act

        await _service.DeleteAsync("1");

        // Assert

        _scenarioRepositoryMock.Verify(
            x => x.DeleteAsync("1"),
            Times.Once);
    }

    [Fact]
    public async Task PatchAsync_WhenScenarioDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange

        _scenarioRepositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((MockScenario?)null);

        var dto = new PatchMockScenarioDTO
        {
            ScenarioName = "Updated"
        };

        // Act

        Func<Task> action = async () =>
            await _service.PatchAsync("1", dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*MockScenario not found*");
    }

    [Fact]
    public async Task PatchAsync_WhenScenarioActivated_ShouldDeactivateOtherScenarios()
    {
        // Arrange

        var activeScenario = new MockScenario
        {
            Id = "1",
            MockEndpointId = "endpoint",
            IsActive = false
        };

        var otherScenario = new MockScenario
        {
            Id = "2",
            MockEndpointId = "endpoint",
            IsActive = true
        };

        _scenarioRepositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(activeScenario);

        _scenarioRepositoryMock
            .Setup(x => x.GetByMockEndpointIdAsync("endpoint"))
            .ReturnsAsync(new List<MockScenario>
            {
                activeScenario,
                otherScenario
            });

        // Act

        await _service.PatchAsync(
            "1",
            new PatchMockScenarioDTO
            {
                IsActive = true
            });

        // Assert

        _scenarioRepositoryMock.Verify(
            x => x.UpdateAsync(
                "2",
                It.IsAny<MockScenario>()),
            Times.Once);
    }
}