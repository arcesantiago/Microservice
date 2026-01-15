using Microservice.Application.Contracts.Persistence;
using Microservice.Infrastructure.Percistence;

namespace Microservice.Infrastructure.Repositories
{
    public class ExampleUnitOfWork : UnitOfWorkBase<ExampleDbContext>, IExampleUnitOfWork
    {
        public IExampleRepository Examples { get; }
        public ExampleUnitOfWork(
            ExampleDbContext context,
            IExampleRepository exampleRepository
        ) : base(context)
        {
            Examples = exampleRepository;
        }
    }
}