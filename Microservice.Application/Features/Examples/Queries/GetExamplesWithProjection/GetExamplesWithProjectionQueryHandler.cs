using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesWithProjection
{
    public class GetExamplesWithProjectionQueryHandler(
        IExampleRepository exampleRepository
        ) : IRequestHandler<GetExamplesWithProjectionQuery, IEnumerable<GetExamplesWithProjectionDto>>
    {
        public async Task<IEnumerable<GetExamplesWithProjectionDto>> Handle(GetExamplesWithProjectionQuery request, CancellationToken cancellationToken)
        {

            return await exampleRepository.GetListAsync(x => new GetExamplesWithProjectionDto { Id = x.Id}, cancellationToken: cancellationToken);
        }
    }
}
