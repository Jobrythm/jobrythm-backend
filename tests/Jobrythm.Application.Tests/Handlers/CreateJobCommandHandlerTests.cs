using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Jobs.Commands.CreateJob;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Tests.Common;
using Jobrythm.Domain.Entities;
using Jobrythm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class CreateJobCommandHandlerTests
{
    private readonly CreateJobCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IJobRepository> _jobRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;

    public CreateJobCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _jobRepositoryMock = new Mock<IJobRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        _handler = new CreateJobCommandHandler(
            _jobRepositoryMock.Object, 
            _clientRepositoryMock.Object, 
            _currentUserServiceMock.Object, 
            MapperFactory.Create());
    }

    [Fact]
    public async Task Handle_ShouldCreateJob_WhenValid()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client { Id = clientId, UserId = "user-1", Name = "Test Client" };
        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var command = new CreateJobCommand
        {
            ClientId = clientId,
            Title = "New Job",
            Description = "Test Job"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Job");
        _jobRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var command = new CreateJobCommand
        {
            ClientId = clientId,
            Title = "New Job"
        };

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowForbidden_WhenClientDoesNotBelongToUser()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client { Id = clientId, UserId = "other-user", Name = "Other Client" };
        _clientRepositoryMock.Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var command = new CreateJobCommand
        {
            ClientId = clientId,
            Title = "New Job"
        };

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ForbiddenException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
