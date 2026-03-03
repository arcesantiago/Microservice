using MediatR;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.ExecuteSqlWithResult
{
    public record ExecuteSqlWithResultQuery(
        FormattableString Sql
    ) : IRequest<IReadOnlyList<ExecuteSqlWithResultDto>>;
}
