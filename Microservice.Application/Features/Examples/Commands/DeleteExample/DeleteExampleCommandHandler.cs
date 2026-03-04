using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.DeleteExample
{
    public class DeleteExampleCommandHandler(
        IReadRepository<Example> readRepository,
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<DeleteExampleCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(DeleteExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                return Result<int>.Failure($"Ejemplo con id {request.Id} no encontrado");

            writeRepository.Delete(example);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(request.Id);
        }
    }
}
