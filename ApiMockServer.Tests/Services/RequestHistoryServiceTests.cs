using ApiMockServer.Interfaces;
using ApiMockServer.Models;
using ApiMockServer.Services;

using FluentAssertions;

using Moq;

using Xunit;

namespace ApiMockServer.Tests.Services;

public class RequestHistoryServiceTests
{
    private readonly Mock<IRequestHistoryRepository> _repositoryMock;

    private readonly RequestHistoryService _service;

    public RequestHistoryServiceTests()
    {
        _repositoryMock = new Mock<IRequestHistoryRepository>();

        _service = new RequestHistoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllHistory()
    {
        // Arrange

        var history = new List<RequestHistory>
        {
            new(),
            new(),
            new()
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(history);

        // Act

        var result = await _service.GetAllAsync();

        // Assert

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateHistoryRecord()
    {
        // Arrange

        var request = new RequestHistory
        {
            Method = "GET",
            Path = "/users",
            StatusCode = 200,
            ResponseTimeMs = 35,
            MockEndpointId = "endpoint-id",
            MockScenarioId = "scenario-id"
        };

        // Act

        await _service.CreateAsync(request);

        // Assert

        _repositoryMock.Verify(
            x => x.CreateAsync(request),
            Times.Once);
    }
}