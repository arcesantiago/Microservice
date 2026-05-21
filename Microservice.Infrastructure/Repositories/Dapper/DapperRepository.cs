// Microservice.Infrastructure/Persistence/Dapper/DapperRepository.cs

using Dapper;
using Microservice.Application.Contracts.Persistence.Dapper;
using Microservice.Application.Models;
using Microservice.Domain.Common;

public class DapperRepository<T>(IDbConnectionFactory connectionFactory)
    : IDapperRepository<T> where T : BaseDomainModel
{
    // Cada entidad define su tabla. No hay magia de convención.
    protected string TableName { get; } = string.Empty;

    // ──── READ ────

    public async Task<T?> FindAsync(int id, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var sql = $"SELECT * FROM {TableName} WHERE id = @Id";
        return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
    }

    public async Task<T?> GetEntityAsync(QueryParameters<T> parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        var cols = SqlBuilder.BuildColumnList(parameters.Columns);
        var sql = $"SELECT {cols} FROM {TableName} {where} LIMIT 1";
        return await conn.QuerySingleOrDefaultAsync<T>(sql, dbParams);
    }

    public async Task<IReadOnlyList<T>> GetListAsync(
        QueryParameters<T>? parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        parameters ??= QueryParameters<T>.Empty;
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        var orderBy = SqlBuilder.BuildOrderBy(parameters);
        var cols = SqlBuilder.BuildColumnList(parameters.Columns);
        var sql = $"SELECT {cols} FROM {TableName} {where} {orderBy}";
        var result = await conn.QueryAsync<T>(sql, dbParams);
        return result.ToList().AsReadOnly();
    }

    public async Task<PagedResult<T>> GetListPaginatedAsync(
        int currentPage, int pageSize,
        QueryParameters<T>? parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        parameters ??= QueryParameters<T>.Empty;
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        var orderBy = SqlBuilder.BuildOrderBy(parameters);
        var pagination = SqlBuilder.BuildPagination(currentPage, pageSize);

        // Dos queries en un solo roundtrip con QueryMultiple
        var sql = $"""
            SELECT COUNT(*) FROM {TableName} {where};
            SELECT * FROM {TableName} {where} {orderBy} {pagination};
            """;

        using var multi = await conn.QueryMultipleAsync(sql, dbParams);
        var total = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<T>()).ToList();

        return new PagedResult<T>(items, total, currentPage, pageSize);
    }

    public async Task<bool> ExistsAsync(QueryParameters<T> parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        var sql = $"SELECT EXISTS(SELECT 1 FROM {TableName} {where})";
        return await conn.ExecuteScalarAsync<bool>(sql, dbParams);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        return await conn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {TableName}");
    }

    public async Task<IReadOnlyList<TResult>> GetProjectedListAsync<TResult>(
        QueryParameters<T>? parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        parameters ??= QueryParameters<T>.Empty;
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        var cols = SqlBuilder.BuildColumnList(parameters.Columns);
        var sql = $"SELECT {cols} FROM {TableName} {where}";
        var result = await conn.QueryAsync<TResult>(sql, dbParams);
        return result.ToList().AsReadOnly();
    }

    public async Task<TResult?> GetProjectedEntityAsync<TResult>(
        QueryParameters<T> parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        var cols = SqlBuilder.BuildColumnList(parameters.Columns);
        var sql = $"SELECT {cols} FROM {TableName} {where} LIMIT 1";
        return await conn.QuerySingleOrDefaultAsync<TResult>(sql, dbParams);
    }

    // ──── WRITE ────

    //public Task<T> AddAsync(T entity, CancellationToken ct);
    //public Task<T> UpdateAsync(T entity, CancellationToken ct);

    public async Task<T> UpdateFieldsAsync(
        T entity, IReadOnlyList<string> columnsToUpdate, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var sets = columnsToUpdate.Select(c =>
        {
            if (!SqlBuilder.IsValidColumnName(c))
                throw new ArgumentException($"Invalid column: {c}");
            return $"{c} = @{c}";
        });
        var sql = $"UPDATE {TableName} SET {string.Join(", ", sets)} WHERE id = @Id";
        var dbParams = new DynamicParameters(entity);
        await conn.ExecuteAsync(sql, dbParams);
        return entity;
    }

    public async Task<int> UpdateManyAsync(
        QueryParameters<T> parameters,
        Dictionary<string, object?> fieldsToUpdate, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var (where, whereParams) = SqlBuilder.BuildWhere(parameters);
        var sets = fieldsToUpdate.Keys.Select(c => $"{c} = @set_{c}");
        var sql = $"UPDATE {TableName} SET {string.Join(", ", sets)} {where}";
        var dbParams = new DynamicParameters(whereParams);
        foreach (var (k, v) in fieldsToUpdate) dbParams.Add($"set_{k}", v);
        return await conn.ExecuteAsync(sql, dbParams);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        await conn.ExecuteAsync($"DELETE FROM {TableName} WHERE id = @Id", new { entity.Id });
    }

    public async Task<int> DeleteManyAsync(QueryParameters<T> parameters, CancellationToken ct)
    {
        using var conn = await connectionFactory.CreateAsync(ct);
        var (where, dbParams) = SqlBuilder.BuildWhere(parameters);
        return await conn.ExecuteAsync($"DELETE FROM {TableName} {where}", dbParams);
    }

    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}