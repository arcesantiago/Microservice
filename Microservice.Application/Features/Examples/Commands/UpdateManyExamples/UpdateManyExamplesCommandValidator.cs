using FluentValidation;

namespace Microservice.Application.Features.Examples.Commands.UpdateManyExamples
{
    /// <summary>
    /// Validator for UpdateManyExamplesCommand
    /// 
    /// Use Case: Validate bulk update command before handler execution
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
    /// - Validates all IDs before any updates
    /// - Prevents empty bulk operations (fail fast)
    /// 
    /// Bulk Operation Optimization:
    /// - Pre-validates all IDs to prevent partial failures
    /// - More efficient than validating during update loop
    /// - Enables AI agents to process entire batch atomically
    /// - Ideal for batch corrections identified by AI analysis
    /// </summary>
    public class UpdateManyExamplesCommandValidator : AbstractValidator<UpdateManyExamplesCommand>
    {
        public UpdateManyExamplesCommandValidator()
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
