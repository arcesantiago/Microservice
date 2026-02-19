using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExampleWithProjection
{
    public class GetExampleWithProjectionQueryHandler(
        IExampleRepository exampleRepository
        ) : IRequestHandler<GetExampleWithProjectionQuery, GetExampleWithProjectionDto?>
    {
        public async Task<GetExampleWithProjectionDto?> Handle(GetExampleWithProjectionQuery request, CancellationToken cancellationToken)
        {
            return await exampleRepository.GetAsync(x => new GetExampleWithProjectionDto { Id = x.Id}, x => x.Id == request.Id, cancellationToken);
        }
    }
}
