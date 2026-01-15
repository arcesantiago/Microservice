using MediatR;

namespace Microservice.Application.Features.Examples.Commands.DeleteManyExamples
{
    public record DeleteManyExamplesCommand(
        int[] Ids
    ) : IRequest<int>;
}
