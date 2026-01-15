using MediatR;

namespace Microservice.Application.Features.Examples.Queries.CountExamples
{
    public record CountExamplesQuery : IRequest<int>;
}
