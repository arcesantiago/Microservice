using Microservice.Application.Contracts.Persistence;
using Microservice.Domain.Entities;
using Microservice.Infrastructure.Percistence;

namespace Microservice.Infrastructure.Repositories
{
    public class ExampleRepository : RepositoryBase<Example>, IExampleRepository
    {
        public ExampleRepository(ExampleDbContext context) : base(context)
        {
        }
    }
}