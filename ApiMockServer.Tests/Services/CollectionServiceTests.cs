using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using ApiMockServer.Services;

using FluentAssertions;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Services;

public class CollectionServiceTests
{
    private readonly Mock<ICollectionRepository> _repositoryMock;

    private readonly CollectionService _service;

    public CollectionServiceTests()
    {
        _repositoryMock = new Mock<ICollectionRepository>();

        _service = new CollectionService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCollectionAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange

        var dto = new CreateCollectionDTO
        {
            Name = "Users",
            Description = "User APIs"
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync("Users"))
            .ReturnsAsync(new Collection());

        // Act

        Func<Task> action = async () =>
            await _service.CreateAsync(dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldCreateCollection()
    {
        // Arrange

        var dto = new CreateCollectionDTO
        {
            Name = "Users",
            Description = "User APIs"
        };

        _repositoryMock
            .Setup(x => x.GetByNameAsync("Users"))
            .ReturnsAsync((Collection?)null);

        // Act

        await _service.CreateAsync(dto);

        // Assert

        _repositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Collection>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCollectionNotFound_ShouldThrowArgumentException()
    {
        // Arrange

        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((Collection?)null);

        var dto = new UpdateCollectionDTO
        {
            Name = "Users"
        };

        // Act

        Func<Task> action = async () =>
            await _service.UpdateAsync("1", dto);

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task DeleteAsync_WhenCollectionNotFound_ShouldThrowArgumentException()
    {
        // Arrange

        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((Collection?)null);

        // Act

        Func<Task> action = async () =>
            await _service.DeleteAsync("1");

        // Assert

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task DeleteAsync_WhenCollectionExists_ShouldDeleteCollection()
    {
        // Arrange

        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(new Collection());

        // Act

        await _service.DeleteAsync("1");

        // Assert

        _repositoryMock.Verify(
            x => x.DeleteAsync("1"),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCollectionExists_ShouldReturnCollection()
    {
        // Arrange

        var collection = new Collection
        {
            Id = "1",
            Name = "Users"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(collection);

        // Act

        var result = await _service.GetByIdAsync("1");

        // Assert

        result.Should().Be(collection);
    }
}