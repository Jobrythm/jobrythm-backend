using FluentAssertions;
using Jobrythm.Application.UseCases.Jobs.Commands.CreateJob;
using Xunit;

namespace Jobrythm.Application.Tests.Validators;

public class CreateJobCommandValidatorTests
{
    private readonly CreateJobCommandValidator _validator;

    public CreateJobCommandValidatorTests()
    {
        _validator = new CreateJobCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_TitleIsEmpty()
    {
        var command = new CreateJobCommand { Title = "" };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Title");
    }

    [Fact]
    public void Should_BeValid_When_CommandIsCorrect()
    {
        var command = new CreateJobCommand
        {
            ClientId = Guid.NewGuid(),
            Title = "Job Title"
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
