using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesFromSql
{
    /// <summary>
    /// Handler que demuestra uso de ISqlQueryRepository para SELECT raw
    /// .NET 10 + C# 14
    /// </summary>
    public class GetExamplesFromSqlQueryHandler(
        ISqlQueryRepository<Example> sqlQueryRepository,
        IMapper mapper
        ) : IRequestHandler<GetExamplesFromSqlQuery, IEnumerable<GetExamplesFromSqlDto>>
    {
        public async Task<IEnumerable<GetExamplesFromSqlDto>> Handle(
            GetExamplesFromSqlQuery request, 
            CancellationToken cancellationToken)
        {
            // ✅ Usar ISqlQueryRepository para SELECT raw
            // Esto es más eficiente para reportes complejos con agregaciones
            FormattableString sql = $"SELECT * FROM \"Examples\" WHERE \"Id\" > 0";

            var examples = await sqlQueryRepository.FromSqlAsync(sql, cancellationToken);

            return mapper.Map<IEnumerable<GetExamplesFromSqlDto>>(examples);
        }
    }
}
