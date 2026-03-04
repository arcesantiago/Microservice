using MediatR;
using Microservice.Application.Common.Results;

namespace Microservice.Application.Features.Examples.Commands.UpdateExample
{
    public record UpdateExampleCommand(
        int Id
    ) : IRequest<Result<int>>;
}
