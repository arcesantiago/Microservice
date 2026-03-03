using MediatR;

namespace Microservice.Application.Features.Examples.Commands.ExecuteInTransaction
{
    public record ExecuteInTransactionCommand(
        string Description
    ) : IRequest<int>;
}
