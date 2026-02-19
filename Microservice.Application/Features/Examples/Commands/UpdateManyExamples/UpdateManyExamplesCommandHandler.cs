using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.UpdateManyExamples
{
    public class UpdateManyExamplesCommandHandler(
        IExampleRepository exampleRepository,
        IUnitOfWork unitOfWork
        ) : IRequestHandler<UpdateManyExamplesCommand, int>
    {
        public async Task<int> Handle(UpdateManyExamplesCommand request, CancellationToken cancellationToken)
        {
            Func<IQueryable<Example>, IQueryable<Example>> filter = query => query.Where(x => request.Ids.Contains(x.Id));

            Func<IQueryable<Example>, Task<int>> updateAction = async query =>
            {

                foreach (var example in query)
                {
                    // En este ejemplo, no hay campos que actualizar, pero se marca como modificado
                    exampleRepository.Update(example);
                }
                return query.Count();
            };

            var updatedCount = await exampleRepository.UpdateManyAsync(filter, updateAction);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return updatedCount;
        }
    }
}
