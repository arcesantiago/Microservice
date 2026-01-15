using MediatR;

namespace Microservice.Application.Features.Examples.Commands.UpdateExampleFields
{
    public record UpdateExampleFieldsCommand(
        int Id
    ) : IRequest<int>;
}
