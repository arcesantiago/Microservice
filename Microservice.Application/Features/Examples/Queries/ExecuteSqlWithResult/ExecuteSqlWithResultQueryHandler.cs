using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.ExecuteSqlWithResult
{
    public class ExecuteSqlWithResultQueryHandler(
        ISqlQueryRepository<Example> sqlQueryRepository,
        IMapper mapper
        ) : IRequestHandler<ExecuteSqlWithResultQuery, IReadOnlyList<ExecuteSqlWithResultDto>>
    {
        public async Task<IReadOnlyList<ExecuteSqlWithResultDto>> Handle(ExecuteSqlWithResultQuery request, CancellationToken cancellationToken)
        {
            var examples = await sqlQueryRepository.FromSqlAsync(request.Sql, cancellationToken);
            return mapper.Map<IReadOnlyList<ExecuteSqlWithResultDto>>(examples);
        }
    }
}
