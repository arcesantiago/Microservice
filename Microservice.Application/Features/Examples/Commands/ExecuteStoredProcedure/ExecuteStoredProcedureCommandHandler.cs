using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.ExecuteStoredProcedure
{
    public class ExecuteStoredProcedureCommandHandler(
        ISqlCommandRepository<Example> sqlCommandRepository
        ) : IRequestHandler<ExecuteStoredProcedureCommand, int>
    {
        public async Task<int> Handle(ExecuteStoredProcedureCommand request, CancellationToken cancellationToken)
        {
            return await sqlCommandRepository.ExecuteStoredProcedureAsync(request.Sql, cancellationToken);
        }
    }
}
