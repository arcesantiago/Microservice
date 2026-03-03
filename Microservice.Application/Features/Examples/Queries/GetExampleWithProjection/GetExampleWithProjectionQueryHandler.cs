using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetExampleWithProjection
{
    public class GetExampleWithProjectionQueryHandler(
        IQueryRepository<Example> queryRepository
        ) : IRequestHandler<GetExampleWithProjectionQuery, GetExampleWithProjectionDto?>
    {
        public async Task<GetExampleWithProjectionDto?> Handle(GetExampleWithProjectionQuery request, CancellationToken cancellationToken)
        {
            return await queryRepository.GetAsync(x => new GetExampleWithProjectionDto { Id = x.Id}, x => x.Id == request.Id, cancellationToken);
        }
    }
}
