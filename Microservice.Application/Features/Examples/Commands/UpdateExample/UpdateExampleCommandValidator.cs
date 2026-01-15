using FluentValidation;

namespace Microservice.Application.Features.Examples.Commands.UpdateExample
{
    public class UpdateExampleCommandValidator : AbstractValidator<UpdateExampleCommand>
    {
        public UpdateExampleCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
