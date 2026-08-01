using ApiMockServer.Controllers;
using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Controllers;

public class MockEndpointControllerTests
{
    private readonly Mock<IMockEndpointService> _serviceMock;

    private readonly MockEndpointsController _controller;

    public MockEndpointControllerTests()
    {
        _serviceMock = new Mock<IMockEndpointService>();

        _controller = new MockEndpointsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithEndpoints()
    {
        // Arrange

        var endpoints = new List<MockEndpoint>
        {
            new MockEndpoint
            {
                Id="1",
                Name="Users",
                Method="GET",
                Path="/users"
            }
        };

        _serviceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(endpoints);

        // Act

        var result = await _controller.GetAll();

        // Assert

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        okResult.Value.Should().BeEquivalentTo(endpoints);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        // Arrange

        var endpoint = new MockEndpoint
        {
            Id="1",
            Name="Users"
        };

        _serviceMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(endpoint);

        // Act

        var result = await _controller.GetById("1");

        // Assert

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;

        ok.Value.Should().Be(endpoint);
    }

    [Fact]
    public async Task Create_WhenValid_ShouldReturnOk()
    {
        // Arrange

        var dto = new CreateMockEndpointDTO
        {
            Name="Users",
            Method="GET",
            Path="/users",
            StatusCode=200,
            ResponseBody="{}",
            IsEnabled=true,
            CollectionId="collection"
        };

        _serviceMock
            .Setup(x => x.CreateAsync(dto))
            .Returns(Task.CompletedTask);

        // Act

        var result = await _controller.Create(dto);

        // Assert

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_WhenDuplicate_ShouldReturnConflict()
    {
        // Arrange

        var dto = new CreateMockEndpointDTO
        {
            Name="Users",
            Method="GET",
            Path="/users"
        };

        _serviceMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new ArgumentException("already exists"));

        // Act

        var result = await _controller.Create(dto);

        // Assert

        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldReturnOk()
    {
        // Arrange

        _serviceMock
            .Setup(x => x.DeleteAsync("1"))
            .Returns(Task.CompletedTask);

        // Act

        var result = await _controller.Delete("1");

        // Assert

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnBadRequest()
    {
        // Arrange

        _serviceMock
            .Setup(x => x.DeleteAsync("1"))
            .ThrowsAsync(new ArgumentException("Mock endpoint not found."));

        // Act

        var result = await _controller.Delete("1");

        // Assert

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}