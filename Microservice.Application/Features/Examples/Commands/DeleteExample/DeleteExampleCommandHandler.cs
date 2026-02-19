using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Exceptions;

namespace Microservice.Application.Features.Examples.Commands.DeleteExample
{
    public class DeleteExampleCommandHandler(
        IExampleRepository exampleRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<DeleteExampleCommand, int>
    {
        public async Task<int> Handle(DeleteExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await exampleRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            exampleRepository.Delete(example);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}
