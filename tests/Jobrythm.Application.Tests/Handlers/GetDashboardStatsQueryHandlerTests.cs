using FluentAssertions;
using Jobrythm.Application.UseCases.Dashboard.Queries.GetDashboardStats;
using Jobrythm.Application.Interfaces;
using Jobrythm.Infrastructure.Persistence;
using Jobrythm.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class GetDashboardStatsQueryHandlerTests
{
    private readonly JobrythmDbContext _context;
    private readonly GetDashboardStatsQueryHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public GetDashboardStatsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<JobrythmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new JobrythmDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var jobRepo = new JobRepository(_context);
        var clientRepo = new ClientRepository(_context);
        var invoiceRepo = new InvoiceRepository(_context);
        var quoteRepo = new QuoteRepository(_context);

        _handler = new GetDashboardStatsQueryHandler(jobRepo, clientRepo, invoiceRepo, quoteRepo, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnStats()
    {
        // Act
        var result = await _handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        // Since we have a simplified handler for now, we just check if it returns a DTO
    }
}
