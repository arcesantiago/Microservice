using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.UpdateExample
{
    /// <summary>
    /// Use Case: Update an existing Example record with new values.
    /// 
    /// When to use:
    /// - When receiving a PUT request to modify a complete resource
    /// - When AI agents generate suggestions to update existing records
    /// - When synchronizing data changes from external systems
    /// - For workflow transitions where entity state must change
    /// 
    /// Responsibilities:
    /// - Retrieve the existing entity by ID
    /// - Validate entity existence before modification
    /// - Persist changes through the write repository
    /// - Commit changes via Unit of Work pattern
    /// 
    /// AI Agent Integration:
    /// - AI agents can use this to:
    ///   * Update records based on learning outcomes
    ///   * Apply automated corrections or enhancements
    ///   * Reflect changes from external AI service predictions
    /// 
    /// Error Handling:
    /// - Returns a failure result if the entity is not found (non-exception flow)
    /// </summary>
    public class UpdateExampleCommandHandler(
        IReadRepository<Example> readRepository,
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateExampleCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(UpdateExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                return Result<int>.Failure($"Ejemplo con id {request.Id} no encontrado");

            writeRepository.Update(example);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(example.Id);
        }
    }
}
