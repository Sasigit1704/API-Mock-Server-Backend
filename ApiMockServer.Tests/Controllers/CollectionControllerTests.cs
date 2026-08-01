using ApiMockServer.Controllers;
using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Controllers;

public class CollectionControllerTests
{
    private readonly Mock<ICollectionService> _serviceMock;
    private readonly CollectionsController _controller;

    public CollectionControllerTests()
    {
        _serviceMock = new Mock<ICollectionService>();
        _controller = new CollectionsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var collections = new List<Collection>
        {
            new Collection
            {
                Id="1",
                Name="Users"
            }
        };

        _serviceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(collections);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var collection = new Collection
        {
            Id="1",
            Name="Users"
        };

        _serviceMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(collection);

        var result = await _controller.GetById("1");

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        _serviceMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((Collection?)null);

        var result = await _controller.GetById("1");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk()
    {
        var dto = new CreateCollectionDTO
        {
            Name="Users"
        };

        var result = await _controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        var dto = new UpdateCollectionDTO
        {
            Name="Updated"
        };

        var result = await _controller.Update("1", dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        var result = await _controller.Delete("1");

        result.Should().BeOfType<OkObjectResult>();
    }
}