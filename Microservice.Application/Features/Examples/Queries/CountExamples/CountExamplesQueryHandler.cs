using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.CountExamples
{
    public class CountExamplesQueryHandler(
                IReadRepository<Example> readRepository
        ) : IRequestHandler<CountExamplesQuery, int>
    {
        public async Task<int> Handle(CountExamplesQuery request, CancellationToken cancellationToken)
        {
            return await readRepository.CountAsync(cancellationToken);
        }
    }
}
