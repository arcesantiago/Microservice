using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Domain.Entities;
using System.Linq.Expressions;

namespace Microservice.Application.Features.Examples.Commands.DeleteManyExamples
{
    public class DeleteManyExamplesCommandHandler(
        IWriteRepository<Example> writeRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<DeleteManyExamplesCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(DeleteManyExamplesCommand request, CancellationToken cancellationToken)
        {
            Expression<Func<Example, bool>> predicate =
                x => request.Ids.Contains(x.Id);

            var deletedCount = await writeRepository.DeleteManyAsync(predicate, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(deletedCount);
        }
    }
}
