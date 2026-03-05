using MediatR;
using Microservice.Application.Common.Results;

namespace Microservice.Application.Features.Examples.Commands.UpdateExampleFields
{
    public record UpdateExampleFieldsCommand(
        int Id,
        string? Name,
        string? Description
    ) : IRequest<Result<int>>;
}
