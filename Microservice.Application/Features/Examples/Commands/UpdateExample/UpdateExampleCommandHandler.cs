using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.Exceptions;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.UpdateExample
{
    public class UpdateExampleCommandHandler(
        IReadRepository<Example> readRepository,
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateExampleCommand, int>
    {
        public async Task<int> Handle(UpdateExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            writeRepository.Update(example);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return example.Id;
        }
    }
}
