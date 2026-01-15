using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Queries.GetExampleByPredicate
{
    public class GetExampleByPredicateQueryHandler(
        IExampleRepository exampleRepository,
        IMapper mapper) : IRequestHandler<GetExampleByPredicateQuery, GetExampleByPredicateDto?>
    {
        private readonly IExampleRepository _exampleRepository = exampleRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<GetExampleByPredicateDto?> Handle(GetExampleByPredicateQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Example, bool>> predicate = x => x.Id == request.Id;

            var example = await _exampleRepository.GetEntityAsync(predicate, cancellationToken: cancellationToken);

            if (example == null)
                return null;

            return _mapper.Map<GetExampleByPredicateDto>(example);
        }
    }
}
