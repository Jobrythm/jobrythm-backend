using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Jobs.Commands.UpdateStatus;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Jobrythm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class UpdateJobStatusCommandHandlerTests
{
    private readonly UpdateJobStatusCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IJobRepository> _jobRepositoryMock;

    public UpdateJobStatusCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _jobRepositoryMock = new Mock<IJobRepository>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        _handler = new UpdateJobStatusCommandHandler(_jobRepositoryMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatus_WhenValid()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new Job { Id = jobId, UserId = "user-1", Title = "Test Job", Status = JobStatus.Draft };
        _jobRepositoryMock.Setup(x => x.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var command = new UpdateJobStatusCommand(jobId, JobStatus.Active);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        job.Status.Should().Be(JobStatus.Active);
        _jobRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        _jobRepositoryMock.Setup(x => x.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);
            
        var command = new UpdateJobStatusCommand(jobId, JobStatus.Completed);

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowForbidden_WhenJobDoesNotBelongToUser()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new Job { Id = jobId, UserId = "other-user", Title = "Other Job" };
        _jobRepositoryMock.Setup(x => x.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var command = new UpdateJobStatusCommand(jobId, JobStatus.Completed);

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ForbiddenException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
