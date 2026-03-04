using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Commands.UpdateExampleFields
{
    public class UpdateExampleFieldsCommandHandler(
        IReadRepository<Example> readRepository,
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateExampleFieldsCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(UpdateExampleFieldsCommand request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                return Result<int>.Failure($"Ejemplo con id {request.Id} no encontrado");

            Expression<Func<Example, object>>[] propertiesToUpdate = [];

            writeRepository.UpdateFields(example, propertiesToUpdate);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(example.Id);
        }
    }
}
