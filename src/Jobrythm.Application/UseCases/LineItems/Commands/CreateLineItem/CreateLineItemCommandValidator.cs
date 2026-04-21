using FluentValidation;
using Jobrythm.Application.UseCases.LineItems.Commands.CreateLineItem;

namespace Jobrythm.Application.UseCases.LineItems.Commands.CreateLineItem;

public class CreateLineItemCommandValidator : AbstractValidator<CreateLineItemCommand>
{
    public CreateLineItemCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Category).IsInEnum();
    }
}
