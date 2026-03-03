using MediatR;

namespace Microservice.Application.Features.Examples.Commands.ExecuteStoredProcedure
{
    public record ExecuteStoredProcedureCommand(
        FormattableString Sql
    ) : IRequest<int>;
}
