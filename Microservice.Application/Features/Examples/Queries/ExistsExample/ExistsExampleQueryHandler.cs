using MediatR;
using Microservice.Application.Contracts.Persistence;

namespace Microservice.Application.Features.Examples.Queries.ExistsExample
{
    public class ExistsExampleQueryHandler(
        IExampleRepository exampleRepository
        ) : IRequestHandler<ExistsExampleQuery, bool>
    {
        private readonly IExampleRepository _exampleRepository = exampleRepository;

        public async Task<bool> Handle(ExistsExampleQuery request, CancellationToken cancellationToken)
        {
            return await _exampleRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);
        }
    }
}
