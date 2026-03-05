using FluentValidation;

namespace Microservice.Application.Features.Examples.Commands.CreateExample
{
    /// <summary>
    /// Validator for CreateExampleCommand
    /// 
    /// Use Case: Validate incoming create command before handler execution
    /// 
    /// Validation Rules:
    /// - Name: Required, max 200 characters
    /// - Description: Optional, max 1000 characters
    /// 
    /// Integration with Result Pattern:
    /// - Invalid commands return Result<int>.Failure() instead of exception
    /// - Validation errors are included in Result.Error property
    /// - AI agents can access validation errors without try-catch
    /// 
    /// Pipeline Behavior:
    /// - Automatically invoked by ValidationBehaviour
    /// - Executes before handler reaches business logic
    /// - Supports functional error handling
    /// </summary>
    public class CreateExampleCommandValidator : AbstractValidator<CreateExampleCommand>
    {
        public CreateExampleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(200)
                .WithMessage("Name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
