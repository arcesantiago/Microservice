using MediatR;

namespace Microservice.Application.Features.Examples.Commands.UpdateExample
{
    public record UpdateExampleCommand(
        int Id
    ) : IRequest<int>;
}
