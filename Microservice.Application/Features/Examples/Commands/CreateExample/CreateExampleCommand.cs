using MediatR;
using Microservice.Application.Common.Results;

namespace Microservice.Application.Features.Examples.Commands.CreateExample
{
    public record CreateExampleCommand(
        int Id
    ) : IRequest<Result<int>>;
}
