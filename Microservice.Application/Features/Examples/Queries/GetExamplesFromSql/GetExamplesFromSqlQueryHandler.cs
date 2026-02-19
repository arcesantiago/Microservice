using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesFromSql
{
    public class GetExamplesFromSqlQueryHandler(
        IExampleRepository exampleRepository,
        IMapper mapper
        ) : IRequestHandler<GetExamplesFromSqlQuery, IEnumerable<GetExamplesFromSqlDto>>
    {
        public async Task<IEnumerable<GetExamplesFromSqlDto>> Handle(GetExamplesFromSqlQuery request, CancellationToken cancellationToken)
        {
            // Ejemplo: ejecutar SQL crudo (en producción esto debería ser más seguro)
            FormattableString sql = $"SELECT * FROM \"Examples\" WHERE \"Id\" > 0";

            var examples = await exampleRepository.FromSqlAsync(sql, cancellationToken);

            return mapper.Map<IEnumerable<GetExamplesFromSqlDto>>(examples);
        }
    }
}
