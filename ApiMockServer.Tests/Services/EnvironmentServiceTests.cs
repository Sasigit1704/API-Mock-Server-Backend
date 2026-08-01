using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using ApiMockServer.Services;

using FluentAssertions;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Services;

public class EnvironmentServiceTests
{
    private readonly Mock<IEnvironmentRepository> _repositoryMock;
    private readonly EnvironmentService _service;

    public EnvironmentServiceTests()
    {
        _repositoryMock = new Mock<IEnvironmentRepository>();

        _service = new EnvironmentService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEnvironments()
    {
        var list = new List<MockEnvironment>
        {
            new(),
            new()
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(list);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEnvironment()
    {
        var env = new MockEnvironment
        {
            Id = "1",
            Name = "Development"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(env);

        var result = await _service.GetByIdAsync("1");

        result.Should().Be(env);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateEnvironment()
    {
        var dto = new CreateEnvironmentDTO
        {
            Name = "Development",
            BaseUrl = "http://localhost",
            Description = "Local",
            IsActive = false
        };

        await _service.CreateAsync(dto);

        _repositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<MockEnvironment>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenActive_ShouldDeactivateExistingEnvironments()
    {
        var environments = new List<MockEnvironment>
        {
            new()
            {
                Id="1",
                Name="Dev",
                IsActive=true
            },
            new()
            {
                Id="2",
                Name="QA",
                IsActive=false
            }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(environments);

        var dto = new CreateEnvironmentDTO
        {
            Name="Prod",
            BaseUrl="http://prod",
            Description="Production",
            IsActive=true
        };

        await _service.CreateAsync(dto);

        _repositoryMock.Verify(
            x => x.UpdateAsync(
                "1",
                It.IsAny<MockEnvironment>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<MockEnvironment>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEnvironment()
    {
        var dto = new UpdateEnvironmentDTO
        {
            Name="Updated",
            BaseUrl="http://updated",
            Description="Updated",
            IsActive=false
        };

        await _service.UpdateAsync("1", dto);

        _repositoryMock.Verify(
            x => x.UpdateAsync(
                "1",
                It.IsAny<MockEnvironment>()),
            Times.Once);
    }

    [Fact]
    public async Task PatchAsync_WhenEnvironmentNotFound_ShouldThrowArgumentException()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((MockEnvironment?)null);

        Func<Task> action = async () =>
            await _service.PatchAsync(
                "1",
                new PatchEnvironmentDTO());

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Environment not found*");
    }

    [Fact]
    public async Task PatchAsync_ShouldPatchEnvironment()
    {
        var env = new MockEnvironment
        {
            Id="1",
            Name="Dev",
            BaseUrl="http://localhost"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(env);

        var dto = new PatchEnvironmentDTO
        {
            Name="Production"
        };

        await _service.PatchAsync("1", dto);

        _repositoryMock.Verify(
            x => x.PatchAsync(
                "1",
                It.IsAny<MockEnvironment>()),
            Times.Once);
    }

    [Fact]
    public async Task PatchAsync_WhenActivated_ShouldDeactivateOtherEnvironments()
    {
        var current = new MockEnvironment
        {
            Id="2",
            Name="QA",
            IsActive=false
        };

        var environments = new List<MockEnvironment>
        {
            new()
            {
                Id="1",
                Name="Dev",
                IsActive=true
            },
            current
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync("2"))
            .ReturnsAsync(current);

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(environments);

        await _service.PatchAsync(
            "2",
            new PatchEnvironmentDTO
            {
                IsActive=true
            });

        _repositoryMock.Verify(
            x => x.UpdateAsync(
                "1",
                It.IsAny<MockEnvironment>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.PatchAsync(
                "2",
                It.IsAny<MockEnvironment>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteEnvironment()
    {
        await _service.DeleteAsync("1");

        _repositoryMock.Verify(
            x => x.DeleteAsync("1"),
            Times.Once);
    }
}