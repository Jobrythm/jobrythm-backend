using FluentAssertions;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Xunit;

namespace Jobrythm.Domain.Tests;

public class JobDomainTests
{
    [Theory]
    [InlineData(1000, 2000, 50.00)]
    [InlineData(500, 1000, 50.00)]
    [InlineData(0, 1000, 100.00)]
    [InlineData(1000, 1000, 0.00)]
    [InlineData(1200, 1000, -20.00)]
    public void MarginPercent_ShouldBeCorrect(long totalCost, long totalRevenue, decimal expectedMargin)
    {
        // Arrange
        var job = new Job();
        job.LineItems.Add(new LineItem 
        { 
            Quantity = 1, 
            UnitCost = totalCost, 
            UnitPrice = totalRevenue 
        });

        // Act & Assert
        job.MarginPercent.Should().Be(expectedMargin);
    }

    [Fact]
    public void TotalCost_ShouldSumLineItems()
    {
        // Arrange
        var job = new Job();
        job.LineItems.Add(new LineItem { Quantity = 2, UnitCost = 500 }); // 1000
        job.LineItems.Add(new LineItem { Quantity = 1, UnitCost = 1500 }); // 1500

        // Act & Assert
        job.TotalCost.Should().Be(2500);
    }

    [Fact]
    public void TotalRevenue_ShouldSumLineItems()
    {
        // Arrange
        var job = new Job();
        job.LineItems.Add(new LineItem { Quantity = 2, UnitPrice = 1000 }); // 2000
        job.LineItems.Add(new LineItem { Quantity = 3, UnitPrice = 500 }); // 1500

        // Act & Assert
        job.TotalRevenue.Should().Be(3500);
    }

    [Fact]
    public void Margin_WithNoLineItems_ShouldBeZero()
    {
        // Arrange
        var job = new Job();

        // Act & Assert
        job.TotalCost.Should().Be(0);
        job.TotalRevenue.Should().Be(0);
        job.MarginAmount.Should().Be(0);
        job.MarginPercent.Should().Be(0);
    }
}
