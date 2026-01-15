using MediatR;

namespace Microservice.Application.Features.Examples.Commands.UpdateManyExamples
{
    public record UpdateManyExamplesCommand(
        int[] Ids
    ) : IRequest<int>;
}
