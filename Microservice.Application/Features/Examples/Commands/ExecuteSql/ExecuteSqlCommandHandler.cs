using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.ExecuteSql
{
    public class ExecuteSqlCommandHandler(
        ISqlCommandRepository<Example> sqlCommandRepository
        ) : IRequestHandler<ExecuteSqlCommand, int>
    {
        public async Task<int> Handle(ExecuteSqlCommand request, CancellationToken cancellationToken)
        {
            return await sqlCommandRepository.ExecuteSqlAsync(request.Sql, cancellationToken);
        }
    }
}
