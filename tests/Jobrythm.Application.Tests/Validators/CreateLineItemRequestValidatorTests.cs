using FluentAssertions;
using Jobrythm.Application.UseCases.LineItems.Commands.CreateLineItem;
using Xunit;

namespace Jobrythm.Application.Tests.Validators;

public class CreateLineItemCommandValidatorTests
{
    private readonly CreateLineItemCommandValidator _validator;

    public CreateLineItemCommandValidatorTests()
    {
        _validator = new CreateLineItemCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_QuantityIsZero()
    {
        var command = new CreateLineItemCommand { Quantity = 0 };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Quantity");
    }

    [Fact]
    public void Should_HaveError_When_CategoryIsInvalid()
    {
        var command = new CreateLineItemCommand { Category = (Jobrythm.Domain.Enums.LineItemCategory)999 };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Category");
    }

    [Fact]
    public void Should_BeValid_When_CommandIsCorrect()
    {
        var command = new CreateLineItemCommand
        {
            JobId = Guid.NewGuid(),
            Description = "Item",
            Quantity = 1,
            Category = Jobrythm.Domain.Enums.LineItemCategory.Labour
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
