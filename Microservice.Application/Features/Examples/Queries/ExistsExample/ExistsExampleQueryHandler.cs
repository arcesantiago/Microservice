using MediatR;
using Microservice.Application.Contracts.Persistence;

namespace Microservice.Application.Features.Examples.Queries.ExistsExample
{
    public class ExistsExampleQueryHandler(
        IExampleRepository exampleRepository
        ) : IRequestHandler<ExistsExampleQuery, bool>
    {
        public async Task<bool> Handle(ExistsExampleQuery request, CancellationToken cancellationToken)
        {
            return await exampleRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);
        }
    }
}
