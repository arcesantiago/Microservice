namespace Microservice.Application.Contracts.Persistence
{
    public interface IExampleUnitOfWork : IDisposable
    {
        IExampleRepository Examples { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}