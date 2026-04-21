using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Clients.Commands.CreateClient;
using Jobrythm.Application.Tests.Common;
using Jobrythm.Domain.Entities;
using Jobrythm.Infrastructure.Persistence;
using Jobrythm.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class CreateClientCommandHandlerTests
{
    private readonly JobrythmDbContext _context;
    private readonly CreateClientCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public CreateClientCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<JobrythmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new JobrythmDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var clientRepo = new ClientRepository(_context);
        _handler = new CreateClientCommandHandler(clientRepo, _currentUserServiceMock.Object, MapperFactory.Create());
    }

    [Fact]
    public async Task Handle_ShouldCreateClient_WhenValid()
    {
        // Arrange
        var command = new CreateClientCommand
        {
            Name = "New Client",
            Email = "client@example.com"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Client");
        _context.Clients.Count().Should().Be(1);
    }
}
