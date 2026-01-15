using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Commands.DeleteManyExamples
{
    public class DeleteManyExamplesCommandHandler(
        IExampleUnitOfWork unitOfWork) : IRequestHandler<DeleteManyExamplesCommand, int>
    {
        private readonly IExampleUnitOfWork _unitOfWork = unitOfWork;

        public async Task<int> Handle(DeleteManyExamplesCommand request, CancellationToken cancellationToken)
        {
            Expression<Func<Example, bool>> predicate =
                x => request.Ids.Contains(x.Id);

            var deletedCount = await _unitOfWork.Examples.DeleteManyAsync(predicate, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return deletedCount;
        }
    }
}
