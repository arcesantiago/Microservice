using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.Exceptions;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.DeleteExample
{
    public class DeleteExampleCommandHandler(
        IReadRepository<Example> readRepository,
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<DeleteExampleCommand, int>
    {
        public async Task<int> Handle(DeleteExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            writeRepository.Delete(example);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}
