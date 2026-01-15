using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Queries.GetExampleWithProjection
{
    public class GetExampleWithProjectionQueryHandler(
        IExampleRepository exampleRepository) : IRequestHandler<GetExampleWithProjectionQuery, GetExampleWithProjectionDto?>
    {
        private readonly IExampleRepository _exampleRepository = exampleRepository;

        public async Task<GetExampleWithProjectionDto?> Handle(GetExampleWithProjectionQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Example, GetExampleWithProjectionDto>> select = x => new GetExampleWithProjectionDto
            {
                Id = x.Id
            };

            Expression<Func<Example, bool>> predicate = x => x.Id == request.Id;

            return await _exampleRepository.GetAsync(select, predicate, cancellationToken);
        }
    }
}
