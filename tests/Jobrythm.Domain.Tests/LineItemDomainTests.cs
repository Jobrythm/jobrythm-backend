using FluentAssertions;
using Jobrythm.Domain.Entities;
using Xunit;

namespace Jobrythm.Domain.Tests;

public class LineItemDomainTests
{
    [Theory]
    [InlineData(2, 500, 1000)]
    [InlineData(0, 500, 0)]
    [InlineData(1.5, 1000, 1500)]
    [InlineData(10, 0, 0)]
    public void TotalCost_ShouldBeCorrect(decimal quantity, long unitCost, long expectedTotal)
    {
        var item = new LineItem { Quantity = quantity, UnitCost = unitCost };
        item.TotalCost.Should().Be(expectedTotal);
    }

    [Theory]
    [InlineData(2, 1000, 2000)]
    [InlineData(0, 1000, 0)]
    [InlineData(1.5, 2000, 3000)]
    [InlineData(10, 0, 0)]
    public void TotalPrice_ShouldBeCorrect(decimal quantity, long unitPrice, long expectedTotal)
    {
        var item = new LineItem { Quantity = quantity, UnitPrice = unitPrice };
        item.TotalPrice.Should().Be(expectedTotal);
    }

    [Theory]
    [InlineData(1000, 2000, 50.00)]
    [InlineData(500, 1000, 50.00)]
    [InlineData(800, 1000, 20.00)]
    [InlineData(1000, 1000, 0.00)]
    public void MarginPercent_ShouldBeCorrect(long unitCost, long unitPrice, decimal expectedMargin)
    {
        var item = new LineItem { Quantity = 1, UnitCost = unitCost, UnitPrice = unitPrice };
        item.MarginPercent.Should().Be(expectedMargin);
    }

    [Fact]
    public void MarginPercent_WithZeroPrice_ShouldBeZero()
    {
        var item = new LineItem { Quantity = 1, UnitCost = 1000, UnitPrice = 0 };
        item.MarginPercent.Should().Be(0);
    }
}
