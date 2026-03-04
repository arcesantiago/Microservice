using MediatR;
using Microservice.Application.Common.Results;

namespace Microservice.Application.Features.Examples.Queries.ExistsExample
{
    public record ExistsExampleQuery(
        int Id
    ) : IRequest<Result<bool>>;
}
