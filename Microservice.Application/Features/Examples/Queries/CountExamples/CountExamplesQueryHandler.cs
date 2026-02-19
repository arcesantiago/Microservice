using MediatR;
using Microservice.Application.Contracts.Persistence;

namespace Microservice.Application.Features.Examples.Queries.CountExamples
{
    public class CountExamplesQueryHandler(
        IExampleRepository exampleRepository
        ) : IRequestHandler<CountExamplesQuery, int>
    {
        public async Task<int> Handle(CountExamplesQuery request, CancellationToken cancellationToken)
        {
            return await exampleRepository.CountAsync(cancellationToken);
        }
    }
}
