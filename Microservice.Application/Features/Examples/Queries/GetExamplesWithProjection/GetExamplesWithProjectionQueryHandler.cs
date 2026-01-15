using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesWithProjection
{
    public class GetExamplesWithProjectionQueryHandler(
        IExampleRepository exampleRepository) : IRequestHandler<GetExamplesWithProjectionQuery, IEnumerable<GetExamplesWithProjectionDto>>
    {
        private readonly IExampleRepository _exampleRepository = exampleRepository;

        public async Task<IEnumerable<GetExamplesWithProjectionDto>> Handle(GetExamplesWithProjectionQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Example, GetExamplesWithProjectionDto>> select = x => new GetExamplesWithProjectionDto
            {
                Id = x.Id
            };

            return await _exampleRepository.GetListAsync(select, cancellationToken: cancellationToken);
        }
    }
}
