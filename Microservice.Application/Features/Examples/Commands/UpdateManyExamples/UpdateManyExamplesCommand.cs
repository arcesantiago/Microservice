using MediatR;
using Microservice.Application.Common.Results;

namespace Microservice.Application.Features.Examples.Commands.UpdateManyExamples
{
    public record UpdateManyExamplesCommand(
        int[] Ids
    ) : IRequest<Result<int>>;
}
