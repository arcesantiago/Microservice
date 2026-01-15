using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Exceptions;
using Microservice.Application.Features.Examples.Commands.UpdateExampleFields;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace ModularMonolith.Modules.Examples.Core.Features.Examples.Commands.UpdateExampleFields
{
    public class UpdateExampleFieldsCommandHandler(
        IExampleUnitOfWork unitOfWork) : IRequestHandler<UpdateExampleFieldsCommand, int>
    {
        private readonly IExampleUnitOfWork _unitOfWork = unitOfWork;

        public async Task<int> Handle(UpdateExampleFieldsCommand request, CancellationToken cancellationToken)
        {
            var example = await _unitOfWork.Examples.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            // Ejemplo: actualizar solo UpdatedAt (aunque en este caso Example no tiene más campos)
            // En una entidad real, se especificarían los campos a actualizar
            Expression<Func<Example, object>>[] propertiesToUpdate = Array.Empty<Expression<Func<Example, object>>>();

            _unitOfWork.Examples.UpdateFields(example, propertiesToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return example.Id;
        }
    }
}
