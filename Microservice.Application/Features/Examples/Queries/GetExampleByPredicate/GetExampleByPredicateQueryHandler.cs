using AutoMapper;
using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Queries.GetExampleByPredicate
{
    public class GetExampleByPredicateQueryHandler(
        IReadRepository<Example> readRepository,
        IMapper mapper
        ) : IRequestHandler<GetExampleByPredicateQuery, Result<GetExampleByPredicateDto>>
    {
        public async Task<Result<GetExampleByPredicateDto>> Handle(GetExampleByPredicateQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Example, bool>> predicate = x => x.Id == request.Id;

            var example = await readRepository.GetEntityAsync(predicate, cancellationToken: cancellationToken);

            if (example == null)
                return Result<GetExampleByPredicateDto>.Failure("Ejemplo no encontrado");

            return Result<GetExampleByPredicateDto>.Success(mapper.Map<GetExampleByPredicateDto>(example));
        }
    }
}
