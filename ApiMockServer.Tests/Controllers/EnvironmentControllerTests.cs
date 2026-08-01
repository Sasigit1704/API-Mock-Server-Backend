using ApiMockServer.Controllers;
using ApiMockServer.DTOs;
using ApiMockServer.Interfaces;
using EnvironmentModel = ApiMockServer.Models.MockEnvironment;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Controllers;

public class EnvironmentControllerTests
{
    private readonly Mock<IEnvironmentService> _serviceMock;
    private readonly EnvironmentController _controller;

    public EnvironmentControllerTests()
    {
        _serviceMock = new Mock<IEnvironmentService>();
        _controller = new EnvironmentController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<EnvironmentModel>());

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        _serviceMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync(new EnvironmentModel());

        var result = await _controller.GetById("1");

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        _serviceMock
            .Setup(x => x.GetByIdAsync("1"))
            .ReturnsAsync((EnvironmentModel?)null);

        var result = await _controller.GetById("1");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk()
    {
        var dto = new CreateEnvironmentDTO();

        var result = await _controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        var dto = new UpdateEnvironmentDTO();

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