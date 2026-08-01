using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using ApiMockServer.Services;

using FluentAssertions;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Services;

public class MockEndpointServiceTests
{
    private readonly Mock<IMockEndpointRepository> _endpointRepositoryMock;
    private readonly Mock<ICollectionRepository> _collectionRepositoryMock;

    private readonly MockEndpointService _service;

    public MockEndpointServiceTests()
    {
        _endpointRepositoryMock = new Mock<IMockEndpointRepository>();

        _collectionRepositoryMock = new Mock<ICollectionRepository>();

        _service = new MockEndpointService(
            _endpointRepositoryMock.Object,
            _collectionRepositoryMock.Object);
    }
    
    [Fact]
    public async Task CreateAsync_WhenEndpointAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        var dto = new CreateMockEndpointDTO
        {
            Name = "Users",
            Method = "GET",
            Path = "/users",
            StatusCode = 200,
            ResponseBody = "{}",
            IsEnabled = true,
            CollectionId = "collection-id"
        };
        _endpointRepositoryMock
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync(new MockEndpoint());

        // ACT
        Func<Task> action = async () =>
            await _service.CreateAsync(dto);

        // ASSERT
        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task CreateAsync_WhenValidEndpoint_ShouldCreateEndpoint()
    {
        // Arrange
        var dto = new CreateMockEndpointDTO
        {
            Name = "Users",
            Method = "GET",
            Path = "/users",
            StatusCode = 200,
            ResponseBody = "{}",
            IsEnabled = true,
            CollectionId = "collection-id"
        };
        _endpointRepositoryMock
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync((MockEndpoint?)null);
        _collectionRepositoryMock
            .Setup(x => x.ExistsAsync("collection-id"))
            .ReturnsAsync(true);

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _endpointRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<MockEndpoint>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenCollectionDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange

        var dto = new CreateMockEndpointDTO
        {
            Name = "Users",
            Method = "GET",
            Path = "/users",
            StatusCode = 200,
            ResponseBody = "{}",
            IsEnabled = true,
            CollectionId = "collection-id"
        };

        _endpointRepositoryMock
            .Setup(x => x.GetByMethodAndPathAsync("GET", "/users"))
            .ReturnsAsync((MockEndpoint?)null);

        _collectionRepositoryMock
            .Setup(x => x.ExistsAsync("collection-id"))
            .ReturnsAsync(false);

        // Act

        Func<Task> action = async () =>
            await _service.CreateAsync(dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Collection does not exist*");
    }

    [Fact]
    public async Task UpdateAsync_WhenEndpointNotFound_ShouldThrowArgumentException()
    {
        // Arrange

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((MockEndpoint?)null);

        var dto = new UpdateMockEndpointDTO
        {
            Name = "Users",
            Method = "GET",
            Path = "/users",
            StatusCode = 200,
            ResponseBody = "{}",
            IsEnabled = true,
            CollectionId = "collection-id"
        };

        // Act

        Func<Task> action = async () =>
            await _service.UpdateAsync("1", dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Mock endpoint not found*");
    }

    [Fact]
    public async Task DeleteAsync_WhenEndpointNotFound_ShouldThrowArgumentException()
    {
        // Arrange

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((MockEndpoint?)null);

        // Act

        Func<Task> action = async () =>
            await _service.DeleteAsync("1");

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Mock endpoint not found*");
    }

    [Fact]
    public async Task DeleteAsync_WhenEndpointExists_ShouldDeleteEndpoint()
    {
        // Arrange

        _endpointRepositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(new MockEndpoint());

        // Act

        await _service.DeleteAsync("1");

        // Assert

        _endpointRepositoryMock.Verify(
            x => x.DeleteAsync("1"),
            Times.Once);
    }
}