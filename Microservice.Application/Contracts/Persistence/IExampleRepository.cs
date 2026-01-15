using Microservice.Application.Contracts.Persistence.Read;
using Microservice.Application.Contracts.Persistence.Write;
using Microservice.Domain.Entities;

namespace Microservice.Application.Contracts.Persistence
{
    public interface IExampleRepository : IReadRepository<Example>, IWriteRepository<Example>, IQueryRepository<Example>
    {
    }
}
