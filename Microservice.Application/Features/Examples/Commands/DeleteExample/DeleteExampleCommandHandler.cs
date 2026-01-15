using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Exceptions;

namespace Microservice.Application.Features.Examples.Commands.DeleteExample
{
    public class DeleteExampleCommandHandler(
        IExampleUnitOfWork unitOfWork) : IRequestHandler<DeleteExampleCommand, int>
    {
        private readonly IExampleUnitOfWork _unitOfWork = unitOfWork;

        public async Task<int> Handle(DeleteExampleCommand request, CancellationToken cancellationToken)
        {
            var example = await _unitOfWork.Examples.FindAsync(request.Id, cancellationToken);

            if (example == null)
                throw new NotFoundException(nameof(example), request.Id);

            _unitOfWork.Examples.Delete(example);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}
