using MediatR;

namespace Microservice.Application.Features.Examples.Commands.ExecuteSql
{
    public record ExecuteSqlCommand(
        FormattableString Sql
    ) : IRequest<int>;
}
