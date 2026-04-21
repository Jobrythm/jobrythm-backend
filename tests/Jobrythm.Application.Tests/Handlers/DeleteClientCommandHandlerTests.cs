using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Clients.Commands.DeleteClient;
using Jobrythm.Domain.Entities;
using Jobrythm.Infrastructure.Persistence;
using Jobrythm.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class DeleteClientCommandHandlerTests
{
    private readonly JobrythmDbContext _context;
    private readonly DeleteClientCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public DeleteClientCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<JobrythmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new JobrythmDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var clientRepo = new ClientRepository(_context);
        _handler = new DeleteClientCommandHandler(clientRepo, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteClient_WhenValid()
    {
        // Arrange
        var client = new Client { Id = Guid.NewGuid(), UserId = "user-1", Name = "Client" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var command = new DeleteClientCommand(client.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _context.Clients.Count().Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenClientHasJobs()
    {
        // Arrange
        var client = new Client { Id = Guid.NewGuid(), UserId = "user-1", Name = "Client" };
        var job = new Job { ClientId = client.Id, UserId = "user-1", Title = "Job" };
        _context.Clients.Add(client);
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var command = new DeleteClientCommand(client.Id);

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ConflictException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
