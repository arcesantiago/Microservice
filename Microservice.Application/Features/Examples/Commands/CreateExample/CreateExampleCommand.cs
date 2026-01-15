using MediatR;

namespace Microservice.Application.Features.Examples.Commands.CreateExample
{
    public record CreateExampleCommand(
        int Id
    ) : IRequest<int>;
}
