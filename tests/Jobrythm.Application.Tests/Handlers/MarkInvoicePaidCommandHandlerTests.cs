using FluentAssertions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Invoices.Commands.MarkPaid;
using Jobrythm.Application.Tests.Common;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Jobrythm.Infrastructure.Persistence;
using Jobrythm.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Jobrythm.Application.Tests.Handlers;

public class MarkInvoicePaidCommandHandlerTests
{
    private readonly JobrythmDbContext _context;
    private readonly MarkInvoicePaidCommandHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public MarkInvoicePaidCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<JobrythmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new JobrythmDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var invoiceRepo = new InvoiceRepository(_context);
        _handler = new MarkInvoicePaidCommandHandler(invoiceRepo, _currentUserServiceMock.Object, MapperFactory.Create());
    }

    [Fact]
    public async Task Handle_ShouldMarkAsPaid_WhenValid()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), UserId = "user-1", Title = "Job" };
        var invoice = new Invoice { Id = Guid.NewGuid(), Job = job, JobId = job.Id, Status = InvoiceStatus.Sent };
        _context.Jobs.Add(job);
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var command = new MarkInvoicePaidCommand(invoice.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(InvoiceStatus.Paid);
        result.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowForbidden_WhenInvoiceDoesNotBelongToUser()
    {
        // Arrange
        var job = new Job { Id = Guid.NewGuid(), UserId = "other-user", Title = "Other Job" };
        var invoice = new Invoice { Id = Guid.NewGuid(), Job = job, JobId = job.Id, Status = InvoiceStatus.Sent };
        _context.Jobs.Add(job);
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var command = new MarkInvoicePaidCommand(invoice.Id);

        // Act & Assert
        await Assert.ThrowsAsync<Jobrythm.Application.Exceptions.ForbiddenException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
