using MediatR;

namespace Microservice.Application.Features.Examples.Commands.DeleteExample
{
    public record DeleteExampleCommand(
        int Id
    ) : IRequest<int>;
}
