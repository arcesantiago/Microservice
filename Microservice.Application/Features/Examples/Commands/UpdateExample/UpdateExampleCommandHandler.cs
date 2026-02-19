using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Exceptions;

namespace Microservice.Application.Features.Examples.Commands.UpdateExample
{
    public class UpdateExampleCommandHandler(
        IExampleRepository exampleRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateExampleCommand, int>
    {
        public async Task<int> Handle(UpdateExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await exampleRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            exampleRepository.Update(example);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return example.Id;
        }
    }
}
