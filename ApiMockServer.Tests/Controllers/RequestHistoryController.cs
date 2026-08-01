using ApiMockServer.Controllers;
using ApiMockServer.Interfaces;
using ApiMockServer.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Controllers;

public class RequestHistoryControllerTests
{
    private readonly Mock<IRequestHistoryService> _serviceMock;
    private readonly RequestHistoryController _controller;

    public RequestHistoryControllerTests()
    {
        _serviceMock = new Mock<IRequestHistoryService>();
        _controller = new RequestHistoryController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        _serviceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<RequestHistory>());

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();
    }
}