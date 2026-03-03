using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesWithProjection
{
    public class GetExamplesWithProjectionQueryHandler(
        IQueryRepository<Example> queryRepository
        ) : IRequestHandler<GetExamplesWithProjectionQuery, IEnumerable<GetExamplesWithProjectionDto>>
    {
        public async Task<IEnumerable<GetExamplesWithProjectionDto>> Handle(GetExamplesWithProjectionQuery request, CancellationToken cancellationToken)
        {

            return await queryRepository.GetListAsync(x => new GetExamplesWithProjectionDto { Id = x.Id}, cancellationToken: cancellationToken);
        }
    }
}
