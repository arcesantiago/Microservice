using MediatR;

namespace Microservice.Application.Features.Examples.Queries.ExistsExample
{
    public record ExistsExampleQuery(
        int Id
    ) : IRequest<bool>;
}
