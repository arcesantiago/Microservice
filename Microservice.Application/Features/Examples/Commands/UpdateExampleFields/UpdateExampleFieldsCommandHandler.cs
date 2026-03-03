using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.Exceptions;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Commands.UpdateExampleFields
{
    public class UpdateExampleFieldsCommandHandler(
        IReadRepository<Example> readRepository,
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateExampleFieldsCommand, int>
    {
        public async Task<int> Handle(UpdateExampleFieldsCommand request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            // Ejemplo: actualizar solo UpdatedAt (aunque en este caso Example no tiene más campos)
            // En una entidad real, se especificarían los campos a actualizar
            Expression<Func<Example, object>>[] propertiesToUpdate = [];

            writeRepository.UpdateFields(example, propertiesToUpdate);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return example.Id;
        }
    }
}
