using FluentValidation;

namespace Microservice.Application.Features.Examples.Commands.DeleteManyExamples
{
    public class DeleteManyExamplesCommandValidator : AbstractValidator<DeleteManyExamplesCommand>
    {
        public DeleteManyExamplesCommandValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("Ids cannot be empty.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All Ids must be greater than 0.");
        }
    }
}
