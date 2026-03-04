using FluentValidation;

namespace Microservice.Application.Features.Examples.Commands.DeleteManyExamples
{
    /// <summary>
    /// Validator for DeleteManyExamplesCommand
    /// 
    /// Use Case: Validate bulk delete command before handler execution
    /// 
    /// Validation Rules:
    /// - Ids: Cannot be empty (at least one Id required)
    /// - Ids: All IDs must be greater than 0
    /// 
    /// Integration with Result Pattern:
    /// - Invalid commands return Result<int>.Failure() instead of exception
    /// - Validation errors are included in Result.Error property
    /// - AI agents can access validation errors without try-catch
    /// 
    /// Pipeline Behavior:
    /// - Automatically invoked by ValidationBehaviour
    /// - Validates all IDs before any deletion
    /// - Prevents empty bulk operations (fail fast)
    /// 
    /// Bulk Operation Optimization:
    /// - Pre-validates all IDs to prevent partial failures
    /// - More efficient than validating during deletion loop
    /// - Enables AI agents to process in single batch
    /// </summary>
    public class DeleteManyExamplesCommandValidator : AbstractValidator<DeleteManyExamplesCommand>
    {
        public DeleteManyExamplesCommandValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty()
                .WithMessage("Ids cannot be empty")
                .WithErrorCode("IdsEmpty")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.Ids)
                .Must(ids => ids.All(id => id > 0))
                .WithMessage("All Ids must be greater than 0")
                .WithErrorCode("InvalidId")
                .WithSeverity(Severity.Error);
        }
    }
}
