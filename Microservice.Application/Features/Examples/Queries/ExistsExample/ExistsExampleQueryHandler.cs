using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.ExistsExample
{
    public class ExistsExampleQueryHandler(
        IReadRepository<Example> readRepository
        ) : IRequestHandler<ExistsExampleQuery, bool>
    {
        public async Task<bool> Handle(ExistsExampleQuery request, CancellationToken cancellationToken)
        {
            return await readRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);
        }
    }
}
