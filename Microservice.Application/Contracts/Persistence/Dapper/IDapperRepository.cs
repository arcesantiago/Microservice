// Microservice.Application/Contracts/Persistence/Dapper/IDapperRepository.cs

using Microservice.Application.Models;
using Microservice.Domain.Common;

namespace Microservice.Application.Contracts.Persistence.Dapper
{
    public interface IDapperRepository<T> :
        IDapperReadRepository<T>,
        IDapperWriteRepository<T>
        where T : BaseDomainModel
    { }

    /// <summary>
    /// Equivalente a IReadRepository{T} sin IQueryable ni Expression trees.
    /// Los filtros se expresan como QueryParameters{T} — Infrastructure los
    /// traduce a SQL parametrizado seguro.
    /// </summary>
    public interface IDapperReadRepository<T> where T : BaseDomainModel
    {
        Task<T?> FindAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<T?> GetEntityAsync(
            QueryParameters<T> parameters,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetListAsync(
            QueryParameters<T>? parameters = null,
            CancellationToken cancellationToken = default);

        Task<PagedResult<T>> GetListPaginatedAsync(
            int currentPage,
            int pageSize,
            QueryParameters<T>? parameters = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            QueryParameters<T> parameters,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            CancellationToken cancellationToken = default);

        // Equivalente a IQueryRepository<T>: proyección a otro tipo
        Task<IReadOnlyList<TResult>> GetProjectedListAsync<TResult>(
            QueryParameters<T>? parameters = null,
            CancellationToken cancellationToken = default);

        Task<TResult?> GetProjectedEntityAsync<TResult>(
            QueryParameters<T> parameters,
            CancellationToken cancellationToken = default);
    }

    public interface IDapperWriteRepository<T> where T : BaseDomainModel
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        // Equivalente a UpdateFields — actualiza solo las columnas indicadas
        Task<T> UpdateFieldsAsync(
            T entity,
            IReadOnlyList<string> columnsToUpdate,
            CancellationToken cancellationToken = default);

        // Equivalente a UpdateManyAsync — bulk update por filtro
        Task<int> UpdateManyAsync(
            QueryParameters<T> parameters,
            Dictionary<string, object?> fieldsToUpdate,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task<int> DeleteManyAsync(
            QueryParameters<T> parameters,
            CancellationToken cancellationToken = default);
    }
}