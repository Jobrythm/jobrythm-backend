using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.LineItems.Commands.CreateLineItem;
using Jobrythm.Application.Tests.Common;
using Jobrythm.Domain.Entities;
using Jobrythm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class CreateLineItemCommandHandlerTests
{
    private readonly CreateLineItemCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IJobRepository> _jobRepositoryMock;
    private readonly Mock<ILineItemRepository> _lineItemRepositoryMock;

    public CreateLineItemCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _jobRepositoryMock = new Mock<IJobRepository>();
        _lineItemRepositoryMock = new Mock<ILineItemRepository>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        _handler = new CreateLineItemCommandHandler(
            _jobRepositoryMock.Object, 
            _lineItemRepositoryMock.Object, 
            _currentUserServiceMock.Object, 
            MapperFactory.Create());
    }

    [Fact]
    public async Task Handle_ShouldCreateLineItem_WhenValid()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new Job { Id = jobId, UserId = "user-1", Title = "Test Job" };
        _jobRepositoryMock.Setup(x => x.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var command = new CreateLineItemCommand
        {
            JobId = jobId,
            Description = "Test Item",
            Quantity = 2,
            UnitCost = 1000,
            UnitPrice = 2000
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("Test Item");
        _lineItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<LineItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowForbidden_WhenJobDoesNotBelongToUser()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new Job { Id = jobId, UserId = "other-user", Title = "Other Job" };
        _jobRepositoryMock.Setup(x => x.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var command = new CreateLineItemCommand { JobId = jobId, Description = "Item" };

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ForbiddenException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        _jobRepositoryMock.Setup(x => x.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);

        var command = new CreateLineItemCommand { JobId = jobId, Description = "Item" };

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
