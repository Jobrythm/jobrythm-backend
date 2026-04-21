using FluentValidation;
using Jobrythm.Application.UseCases.Jobs.Commands.CreateJob;

namespace Jobrythm.Application.UseCases.Jobs.Commands.CreateJob;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
