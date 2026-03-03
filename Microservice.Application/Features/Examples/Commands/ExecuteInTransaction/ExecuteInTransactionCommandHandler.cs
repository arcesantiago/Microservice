using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.ExecuteInTransaction
{
    public class ExecuteInTransactionCommandHandler(
        ISqlRepository<Example> sqlRepository
        ) : IRequestHandler<ExecuteInTransactionCommand, int>
    {
        public async Task<int> Handle(ExecuteInTransactionCommand request, CancellationToken cancellationToken)
        {
            return await sqlRepository.ExecuteInTransactionAsync(
                async (repository) =>
                {
                    // Ejemplo: Ejecutar operaciones dentro de una transacción
                    // Si cualquiera falla, todo se revierte automáticamente
                    var result = await repository.ExecuteSqlAsync(
                        $"INSERT INTO Examples (Description) VALUES ({request.Description})", 
                        cancellationToken);
                    return result;
                },
                cancellationToken);
        }
    }
}
