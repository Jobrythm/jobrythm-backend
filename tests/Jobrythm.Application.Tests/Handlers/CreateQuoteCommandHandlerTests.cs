using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Quotes.Commands.CreateQuote;
using Jobrythm.Application.Tests.Common;
using Jobrythm.Domain.Entities;
using Jobrythm.Infrastructure.Persistence;
using Jobrythm.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class CreateQuoteCommandHandlerTests
{
    private readonly JobrythmDbContext _context;
    private readonly CreateQuoteCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly NumberSequenceService _numberSequenceService;

    public CreateQuoteCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<JobrythmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new JobrythmDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var quoteRepo = new Jobrythm.Infrastructure.Persistence.Repositories.QuoteRepository(_context);
        var jobRepo = new Jobrythm.Infrastructure.Persistence.Repositories.JobRepository(_context);

        _handler = new CreateQuoteCommandHandler(jobRepo, quoteRepo, _currentUserServiceMock.Object, MapperFactory.Create());
    }

    [Fact]
    public async Task Handle_ShouldCreateQuoteAndCalculateTotals_WhenValid()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), UserId = "user-1", Title = "Test Job" };
        var lineItem = new LineItem { JobId = job.Id, Quantity = 2, UnitPrice = 1000, Category = Jobrythm.Domain.Enums.LineItemCategory.Labour };
        job.LineItems.Add(lineItem);
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var command = new CreateQuoteCommand { JobId = job.Id, VatRate = 20 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalNet.Should().Be(2000);
        result.VatAmount.Should().Be(400); // 20% of 2000
        result.TotalGross.Should().Be(2400);
        result.QuoteNumber.Should().Contain("QT-");
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenQuoteAlreadyExists()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), UserId = "user-1", Title = "Test Job" };
        var quote = new Quote { JobId = job.Id, QuoteNumber = "Q1" };
        job.Quote = quote;
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var command = new CreateQuoteCommand { JobId = job.Id };

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ConflictException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_ShouldThrowForbidden_WhenJobDoesNotBelongToUser()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), UserId = "other-user", Title = "Other Job" };
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var command = new CreateQuoteCommand { JobId = job.Id };

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ForbiddenException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
